using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

using WebColumns.Logic;
using System.Windows.Media.Imaging;

namespace WebColumns
{
    public partial class GamePage : UserControl
    {
        private bool _left = false;
        private bool _right = false;
        private bool _down = false;
        private bool _up = false;
        private bool _toggle = false;

        private Timer _keyTimer;

        private List<Image> _previewImages = new List<Image>();

        public GamePage()
        {
            InitializeComponent();

            boardControl.Board.OnScoreChanged += new Logic.ScoreChanged(Board_OnScoreChanged);
            boardControl.Board.OnNewPreviewAvailable += new Logic.NewPreviewAvailable(Board_OnNewPreviewAvailable);

            this.KeyDown += new KeyEventHandler(GamePage_KeyDown);
            this.KeyUp += new KeyEventHandler(GamePage_KeyUp);
            _keyTimer = new Timer(TimerEvent, null, 100, 100);

            this.Loaded += new RoutedEventHandler(delegate(object sender, RoutedEventArgs e)
                {
                    this.Focus();       // workaround, um keyevents zu erhalten (vgl. ContentControl)
                    boardControl.Board.Init();
                });
        }

        void Board_OnNewPreviewAvailable(Triple triple)
        {
            //Debug.WriteLine(String.Format("Preview: {0} {1} {2}",
            //    new object[] { triple[0].Color, triple[1].Color, triple[2].Color }));
            Dispatcher.BeginInvoke(delegate()
            {
                if (_previewImages.Count > 0)
                {
                    for (int i = _previewImages.Count - 1; i >= 0; i--)
                        canvas_preview.Children.Remove(_previewImages[i]);
                    _previewImages.Clear();
                }
                for (int i = 0; i < 3; i++)
                {
                    Element elem = triple[i];
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(String.Format("images/elem{0}.png", elem.Color.ToString()), UriKind.Relative));
                    image.Width = 30;
                    image.Height = 30;
                    image.SetValue(Canvas.LeftProperty, 0.0);
                    image.SetValue(Canvas.TopProperty, i * 30.0);
                    _previewImages.Add(image);
                    canvas_preview.Children.Add(image);
                }
            });
        }

        void Board_OnScoreChanged(int roundscore, int score, int elements, int level)
        {
            Dispatcher.BeginInvoke(delegate()
            {
                label_Score.Content = score.ToString("00000000");
                label_Elements.Content = elements.ToString("0000");
                label_Level.Content = level.ToString();
            });
        }

        void GamePage_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Up);
            if (e.Key == Key.Left) _left = false;
            if (e.Key == Key.Right) _right = false;
            if (e.Key == Key.Down) _down = false;
            if (e.Key == Key.Up)
            {
                if (_toggle && boardControl.Board.Mode == BoardMode.ElementMove) boardControl.Board.ToggleTriple();
                _toggle = false;
                _up = false;
            }
        }

        void GamePage_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Up);
            if (e.Handled)
            {
                if (e.Key == Key.Left) _left = true;
                if (e.Key == Key.Right) _right = true;
                if (e.Key == Key.Down) _down = true;
                if (e.Key == Key.Up)
                {
                    _toggle = true;
                    _up = true;
                }
            }
        }

        public void TimerEvent(object o)
        {
            boardControl.Dispatcher.BeginInvoke(delegate()
            {
                if (boardControl.Board.Mode != BoardMode.ElementMove) return;
                if (_left) boardControl.Board.MoveTripleLeft();
                if (_right) boardControl.Board.MoveTripleRight();
                if (_down) boardControl.Board.DropTriple();
                if (_up)
                {
                    boardControl.Board.ToggleTriple();
                    _toggle = false;
                }
            });
        }

        private void button_Pause_Click(object sender, RoutedEventArgs e)
        {
            boardControl.Board.StopTimer();
            button_Pause.IsEnabled = false;
            button_Resume.IsEnabled = true;
        }

        private void button_Resume_Click(object sender, RoutedEventArgs e)
        {
            boardControl.Board.ChangeTimer(500);
            button_Pause.IsEnabled = true;
            button_Resume.IsEnabled = false;
        }
    }
}
