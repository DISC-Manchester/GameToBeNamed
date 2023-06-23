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
        private static readonly WaveOutEvent MusicPlayer = new();
        private readonly Stopwatch stopwatch = new();
        private readonly ScoreBoard ScoreBoard;
        private int CurrentLevel = 1;
        private bool GameRestart = false;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static DiscWindow Instance { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Paddle Paddle { get; } = new();
        public Level Level { get; private set; }

        public DiscWindow()
        {
            InitializeComponent();
            Instance = this;
            Level = new("assets.levels.level_1.json");
            ScoreBoard = ScoreBoard.Load();
            stopwatch.Start();
            MusicPlayer.Init(new Mp3FileReader(AssetUtil.OpenEmbeddedFile("assets.sounds.music.mp3")));
            //MusicPlayer.Play();
            GC.Collect(2,GCCollectionMode.Aggressive,true,true);
            GC.WaitForPendingFinalizers();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (GameRestart)
            {
                if (e.Key == Key.Enter)
                {
                    SoundUtils.PlaySound(SoundUtils.CLICK_SOUND);
                    GameRestart = false;
                    DisplayText.Text = "";
                    Level = new("assets.levels.level_" + Convert.ToString(CurrentLevel) + ".json");
                }
            }
            else
            {
                Paddle.OnKeyDown(e.Key);
                Level.GetBall().OnKeyDown(e.Key);
            }
            base.OnKeyDown(e);
        }

        public override void Render(DrawingContext context)
        {
            if (GameRestart)
                DisplayText.Text = "Press Enter To Restart";
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
                int Score = Level.GetBall().GetScore();
                if (Score > ScoreBoard.entry[0].Score)
                {
                    Action<int, string> callback = FinishNewUser;
                    var popupWindow = new PopUpWindow(Score, callback);
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
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            GC.WaitForPendingFinalizers();
        }



        protected override void OnClosing(CancelEventArgs e)
        {
            SoundUtils.CleanUp();
            MusicPlayer.Stop();
            MusicPlayer.Dispose();
            stopwatch.Stop();
            Renderer.Dispose();
            ScoreBoard.Save(ScoreBoard);
            base.OnClosing(e);
        }

        public void LevelWon()
        {
            CurrentLevel++;
            Level = new Level("assets.levels.level_" + Convert.ToString(CurrentLevel) + ".json");
            GC.Collect(2, GCCollectionMode.Aggressive, true, true);
            GC.WaitForPendingFinalizers();
        }
    }
}
