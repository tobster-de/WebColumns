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
using System.Windows.Media.Imaging;
using System.Diagnostics;

using WebColumns.Logic;

namespace WebColumns
{
    public partial class BoardControl : UserControl
    {
        private static String BACKGROUND = "images/board_background_red.png";
        public static int TILESIZE = 60;

        private Dictionary<Element, Image> _elementMap = new Dictionary<Element, Image>();

        private Board _board;

        public Board Board
        {
            get { return _board; }
            private set { _board = value; }
        }

        public BoardControl()
        {
            InitializeComponent();

            BackGoundImages();
            _board = new Board();
            //_board.OnElementsAdded += new ElementsAdded(BoardElementsAdded);
            //_board.OnElementsRemoved += new ElementsRemoved(BoardElementsRemoved);
            this.Loaded += new RoutedEventHandler(delegate(object sender, RoutedEventArgs e)
                {
                    Dispatcher.BeginInvoke(delegate()
                    {
                        _board.Init();
                    });
                });
        }

        public void TestImage()
        {
            Debug.WriteLine("TestImage");
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("images/elemBlue.png", UriKind.Relative));
            image.Width = 30;
            image.Height = 30;
            image.SetValue(Canvas.LeftProperty, (double)90);
            image.SetValue(Canvas.TopProperty, (double)30);
            canvas.Children.Add(image);
            Debug.WriteLine("/TestImage");
        }


        void BoardElementsRemoved(List<Element> elements)
        {
            canvas.Dispatcher.BeginInvoke(delegate()
            {
                foreach (Element elem in elements)
                {
                    if (!_elementMap.ContainsKey(elem)) continue;

                    Image image = _elementMap[elem];
                    if (image != null && canvas.Children.Contains(image))
                        canvas.Children.Remove(image);
                }
            });
        }

        void BoardElementsAdded(List<Element> elements)
        {
            canvas.Dispatcher.BeginInvoke(delegate()
            {
                foreach (Element elem in elements)
                {
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(String.Format("images/elem{0}.png", elem.Color.ToString()), UriKind.Relative));
                    image.Width = 30;
                    image.Height = 30;
                    image.SetValue(Canvas.LeftProperty, elem.Position.X);
                    image.SetValue(Canvas.TopProperty, elem.Position.Y);
                    _elementMap.Add(elem, image);

                    canvas.Children.Add(image);
                }

            });
        }

        private void BackGoundImages()
        {
            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(new Uri(BACKGROUND, UriKind.Relative));
            int mi = (int)Math.Ceiling(Width / TILESIZE);
            int mj = (int)Math.Ceiling(Height / TILESIZE);
            for (int i = 0; i < mi; i++)
                for (int j = 0; j < mj; j++)
                {
                    Rectangle r = new Rectangle();
                    r.Width = TILESIZE;
                    r.Height = TILESIZE;
                    r.Fill = image;
                    r.SetValue(Canvas.LeftProperty, (double)i * TILESIZE);
                    r.SetValue(Canvas.TopProperty, (double)j * TILESIZE);
                    canvas.Children.Add(r);
                }
            RectangleGeometry rg = new RectangleGeometry();
            rg.Rect = new Rect(new Point(), new Size(Width, Height));
            this.Clip = rg;
        }

        //public BitmapSource GetTileImageUsingArray(int height, int width, BitmapSource tileSource)
        //{
        //    WriteableBitmap source = new WriteableBitmap(tileSource);
        //    WriteableBitmap final = new WriteableBitmap(width, height);

        //    int repeatX = (int)Math.Floor(width / tileSource.PixelWidth);
        //    int restX = (int)Math.Min(width % tileSource.PixelWidth, width);

        //    int repeatY = (int)Math.Floor(height / tileSource.PixelHeight);
        //    int restY = (int)Math.Min(height % tileSource.PixelHeight, height);

        //    int cursor = 0;
        //    int tileRowLength = width * source.PixelHeight;
        //    int[] tileRow = new int[tileRowLength];
        //    for (int i = 0; i <= source.PixelHeight - 1; i++)
        //    {
        //        int startIndex = i * source.PixelWidth;
        //        for (int x = 0; x < repeatX; x++)
        //        {
        //            Array.Copy(source.Pixels, startIndex, tileRow, cursor, source.PixelWidth);
        //            cursor += source.PixelWidth;
        //        }
        //        Array.Copy(source.Pixels, startIndex, tileRow, cursor, restX);
        //        cursor += restX;
        //    }

        //    cursor = 0;
        //    for (int y = 0; y < repeatY; y++)
        //    {
        //        Array.Copy(tileRow, 0, final.Pixels, cursor, tileRowLength);
        //        cursor += tileRowLength;
        //    }
        //    Array.Copy(tileRow, 0, final.Pixels, cursor, width * restY);

        //    final.Invalidate();
        //    return final;
        //}
    }
}

/*
    <Grid x:Name="LayoutRoot" ShowGridLines="False" Background="Black">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>
    </Grid>
*/