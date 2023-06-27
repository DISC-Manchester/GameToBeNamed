using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using DiscOut.GameObjects.Dynamic;
using DiscOut.GameObjects.World;
using DiscOut.GameObjects.World.Score;
using DiscOut.Util;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace DiscOut.Avalonia
{
    public partial class DiscWindow : Window
    {
        private readonly HashSet<Key> keys = new HashSet<Key>();
        private static readonly WaveOutEvent MusicPlayer = new WaveOutEvent();
        private readonly ScoreBoard ScoreBoard;
        private int CurrentLevel = 1;
        private bool GameRestart = false;
        private readonly DispatcherTimer ticker = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60) };
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static DiscWindow Instance { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal Paddle Paddle { get; } = new Paddle();
        internal Level Level { get; private set; }
        public DiscWindow()
        {
            InitializeComponent();
            Instance = this;
            Level = new Level("assets.levels.level_1.json");
            ScoreBoard = ScoreBoard.Load();
            MusicPlayer.Init(new Mp3FileReader(AssetUtil.OpenEmbeddedFile("assets.sounds.music.mp3")));
            MusicPlayer.Volume = 0.25f;
            MusicPlayer.Play();
            ticker.Tick += delegate
            {
                if (GameRestart)
                {
                    if (keys.Contains(Key.Enter))
                    {
                        SoundUtils.PlaySound(SoundUtils.CLICK_SOUND);
                        GameRestart = false;
                        DisplayText.Text = "";
                        Level = new Level("assets.levels.level_" + Convert.ToString(CurrentLevel) + ".json");
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
            };
            ticker.IsEnabled = true;
            Level.OnUpdate();
            Paddle.OnUpdate();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, true, true);
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

        public override void Render(DrawingContext context)
        {
            GLView.Render(context);
            base.Render(context);
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.MaxValue);
        }

        protected void FinishNewUser(int LastScoreData, string UserNameData)
        {
            ScoreBoard.AddScore(new ScoreEntry(UserNameData, LastScoreData));
            ScoreBoard.Save(ScoreBoard);
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
        }
    }
}
