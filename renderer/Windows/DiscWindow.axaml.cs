using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using NAudio.Wave;
using SquareSmash.objects;
using SquareSmash.objects.components;
using SquareSmash.objects.score;
using SquareSmash.utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
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

        private readonly Stopwatch stopwatch = new();
        public bool GameRestart { get; private set; } = false;
        private int CurrentLevel = 1;
        public Level Level { get; private set; }

        public ScoreBoard ScoreBoard { get; private set; }
        public QuadBatchRenderer GLRenderer
        {
            get => GLView.Renderer;
        }

        public DiscWindow()
        {
            InitializeComponent();
            Instance = this;
            KeyDown += OnKeyDown!;
            Paddle = new();
            Level = new("assets.levels.level_1.json");
            ScoreBoard = ScoreBoard.Load();
            stopwatch.Start();
            musicPlayer.Init(new Mp3FileReader(AssetUtil.OpenEmbeddedFile("assets.sounds.music.mp3")));
            //musicPlayer.Play();
            GC.Collect();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (GameRestart)
            {
                if (e.Key == Key.Enter)
                {
                    GameRestart = false;
                    DisplayText.Text = "";
                    Level = new("assets.levels.level_" + Convert.ToString(CurrentLevel) + ".json");
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
                {
                    DisplayText.Text = "Press Space To Start!\n " + ScoreBoard.ToString();
                }
                else
                    DisplayText.Text = "";
            }
            if (Paddle.IsDead())
            {
                Paddle.ResetPaddle();
                LastScore = Level.GetBall().GetScore();
                if (LastScore > ScoreBoard.entry[0].Score)
                {
                    Action<int, string> callback = FinishNewUser;
                    var popupWindow = new PopUpWindow(LastScore, callback);
                    var task = popupWindow.ShowDialog(this);
                }
                GameRestart = true;
            }
            float DeltaTime = (float)Instance.stopwatch.Elapsed.TotalMilliseconds;
            Instance.stopwatch.Restart();
            Instance.Level.OnUpdate(DeltaTime);
            Instance.Paddle.OnUpdate(DeltaTime);
            GLView.Render(context);
            base.Render(context);
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
        }

        protected void FinishNewUser(int LastScoreData, string UserNameData)
        {
            ScoreBoard.addScore(new(UserNameData, LastScoreData));
            ScoreBoard.Save(ScoreBoard);
        }



        protected override void OnClosing(CancelEventArgs e)
        {
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
            Level = new Level("assets.levels.level_" + Convert.ToString(CurrentLevel) + ".json");
        }
    }
}
