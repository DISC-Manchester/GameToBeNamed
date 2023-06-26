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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SquareSmash.renderer.Windows
{
    public partial class DiscWindow : Window
    {
        private HashSet<Key> keys = new HashSet<Key>();
        private static readonly WaveOutEvent MusicPlayer = new();
        private readonly ScoreBoard ScoreBoard;
        private int CurrentLevel = 1;
        private bool GameRestart = false;

        private readonly DispatcherTimer ticker = new() { Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60) };

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
            MusicPlayer.Init(new Mp3FileReader(AssetUtil.OpenEmbeddedFile("assets.sounds.music.mp3")));
            MusicPlayer.Volume = 0.25f;
            //MusicPlayer.Play();
            ticker.Tick += delegate { tick(); };
            ticker.IsEnabled = true;
            GC.Collect(2,GCCollectionMode.Aggressive,true,true);
            GC.WaitForPendingFinalizers();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            keys.Add(e.Key);
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            keys.Remove(e.Key);
            base.OnKeyUp(e);
        }

        protected void tick()
        {
            if (GameRestart)
            {
                if (keys.Contains(Key.Enter))
                {
                    SoundUtils.PlaySound(SoundUtils.CLICK_SOUND);
                    GameRestart = false;
                    DisplayText.Text = "";
                    Level = new("assets.levels.level_" + Convert.ToString(CurrentLevel) + ".json");
                    keys.Clear();
                }
                DisplayText.Text = "Press Enter To Restart";
            }
            else
            {
                Paddle.OnKeyDown(keys);
                Level.GetBall().OnKeyDown(keys);
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
            Level.OnUpdate();
            Paddle.OnUpdate();
        }

        public override void Render(DrawingContext context)
        {
           
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
            ticker.IsEnabled = false;
            SoundUtils.CleanUp();
            MusicPlayer.Stop();
            MusicPlayer.Dispose();
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
