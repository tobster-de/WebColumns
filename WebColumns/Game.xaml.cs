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

        private Timer _keyTimer;

        private List<Image> _previewImages = new List<Image>();

        public GamePage()
        {
            InitializeComponent();

            //boardControl.Board.OnScoreChanged += new Logic.ScoreChanged(Board_OnScoreChanged);
            boardControl.Board.OnNewPreviewAvailable += new Logic.NewPreviewAvailable(Board_OnNewPreviewAvailable);

            this.KeyDown += new KeyEventHandler(GamePage_KeyDown);
            this.KeyUp += new KeyEventHandler(GamePage_KeyUp);
            _keyTimer = new Timer(TimerEvent, null, 25, 25);

            this.Loaded += new RoutedEventHandler(delegate(object sender, RoutedEventArgs e)
                {
                    // workaround, um keyevents zu erhalten (vgl. ContentControl)
                    this.Focus();
                });
        }

        void Board_OnNewPreviewAvailable(Triple triple)
        {
            //Debug.WriteLine(String.Format("Preview: {0} {1} {2}",
            //    new object[] { triple[0].Color, triple[1].Color, triple[2].Color }));
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
        }

        void Board_OnScoreChanged(int score, int elements, int level)
        {
            Dispatcher.BeginInvoke(delegate()
            {
                label_Score.Content = score;
                label_Elements.Content = elements;
                label_Level.Content = level;
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
                boardControl.TestImage();
            }
        }

        void GamePage_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Down);
            if (e.Handled)
            {
                if (e.Key == Key.Left) _left = true;
                if (e.Key == Key.Right) _right = true;
                if (e.Key == Key.Down) _down = true;
            }
        }

        public void TimerEvent(object o)
        {
            boardControl.Dispatcher.BeginInvoke(delegate()
            {
                if (_left) boardControl.Board.MoveElements(-1, 0);
                if (_right) boardControl.Board.MoveElements(+1, 0);
                if (_down) boardControl.Board.MoveElements(0, +1);
            });
        }
    }
}
