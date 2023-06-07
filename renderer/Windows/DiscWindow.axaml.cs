using Avalonia.Controls;
using SquareSmash.objects.components;
using SquareSmash.objects.score;
using SquareSmash.objects;
using System.Diagnostics;
using SquareSmash.renderer.Windows.Controls;
using Avalonia.Media;
using Avalonia;
using System;
using System.Threading;
using Avalonia.Input;
using System.ComponentModel;
using NAudio.Wave;
using SquareSmash.utils;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace SquareSmash.renderer.Windows
{
    public partial class DiscWindow : Window
    {
        private static readonly WaveOutEvent musicPlayer = new();
        private int LastScore = 0;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static DiscWindow Instance { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Paddle Paddle { get; private set; }

        public float GLHeight
        {
            get => ((1.0f - ((float)Height / (float)Width)) * 2.0f) - 1.0f;
        }

        private readonly Stopwatch stopwatch = new();
        public bool GameRestart { get; private set; } = false;
        private int CurrentLevel = 1;
        public Level Level { get; private set; }

        protected readonly CancellationTokenSource Cancellation = new CancellationTokenSource();
        private Thread UpdateThread;
        public ScoreBoard ScoreBoard { get; private set; }
        public QuadBatchRenderer GLRenderer
        {
            get => GLView.Renderer;
        }

        protected void Update()
        {
            while (!Instance.Cancellation.IsCancellationRequested)
            {
                //Instance.pauseEvent.WaitOne();
                float DeltaTime = (float)Instance.stopwatch.Elapsed.TotalMilliseconds;
                Instance.stopwatch.Restart();
                var t = Task.Run(() => Instance.Level.OnUpdate(this, DeltaTime));
                Instance.Paddle.OnUpdate(DeltaTime);
                t.Wait();
            }
        }

        public DiscWindow()
        {
            InitializeComponent();
            Instance = this;
            KeyDown += OnKeyDown!;
            Paddle = new();
            Level = new(this, "assets.levels.level_1.json");
            ScoreBoard = ScoreBoard.Load();
            Console.WriteLine(ScoreBoard.ToString());
            stopwatch.Start();
            musicPlayer.Init(new Mp3FileReader(AssetUtil.OpenEmbeddedFile("assets.sounds.music.mp3")));
            UpdateThread = new Thread(() => { Instance.Update(); });
            UpdateThread.Start();
            musicPlayer.Play();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (GameRestart)
            {
                if (e.Key == Key.Enter)
                {
                    GameRestart = false;
                    DisplayText.Text = "";
                    Level = new(this, "assets.levels.level_" + Convert.ToString(CurrentLevel) + ".json");
                }
                else
                    return;
            }
            else
            {
                Paddle.OnKeyDown(sender, e);
                Level.GetBall().OnKeyDown(sender, e);
            }
        }

        public override void Render(DrawingContext context)
        {
            if (GameRestart)
                DisplayText.Text = "        Final Score: " + Convert.ToString(LastScore) + "\nPress Enter To Restart";
            else
            {
                if (!Level.GetBall().IsAlive())
                    DisplayText.Text = "Press Space To Start";
                else
                    DisplayText.Text = "";
            }
            if (Paddle.IsDead())
            {
                Paddle.ResetPaddle();
                LastScore = Level.GetBall().GetScore();
                /*if (LastScore > ScoreBoard.entry[0].Score)
                {
                    ShowDialog();
                }*/
                GameRestart = true;
            }
            GLView.Render(context);
            base.Render(context);
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
        }



        protected override void OnClosing(CancelEventArgs e)
        {
            Cancellation.Cancel();
            UpdateThread.Join();
            musicPlayer.Stop();
            musicPlayer.Dispose();
            stopwatch.Stop();
            Renderer.Dispose();
            ScoreBoard.Save(ScoreBoard);
            base.OnClosing(e);
        }

        public void LevelWon()
        {
            CurrentLevel++;
            Level = new Level(this, "assets.levels.level_" + Convert.ToString(CurrentLevel) + ".json");
        }
    }
}
