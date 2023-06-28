using Avalonia.Controls;
using Avalonia.Interactivity;
using DiscOut.Util;
using System;
namespace DiscOut.Avalonia
{
    public partial class PopUpWindow : Window
    {
        private readonly Action<int, string> Callback;
        private readonly int Score;

        public PopUpWindow()
        {
            Score = 0;
            Callback = (int n, string s) => { };
            InitializeComponent();
            TextInput.Text = "";
        }

        public PopUpWindow(int score, Action<int, string> callback)
        {
            Callback = callback;
            Score = score;
            InitializeComponent();
            TextInput.Text = "";
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            TextInput.Focus();
            SoundUtils.PlaySound(SoundUtils.CLICK_SOUND);
            if (TextInput.Text.Length >= 1 && TextInput.Text.Length <= 10)
            {
                Callback.Invoke(Score, TextInput.Text);
                Close();
                return;
            }
            TextInput.Text = "must be between 1 & 10 chars";
        }
    }
}