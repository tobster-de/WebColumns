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

using WebColumns.Logic;

namespace WebColumns
{
    public partial class Menu : UserControl
    {

        private static String[] IMAGES = { "images/image1.png", "images/image2.png", "images/image3.png" };    // images
        private static double MARGIN = 50;			// Margin between images
        private static double IMAGE_WIDTH = 48;	    // Image width
        private static double IMAGE_HEIGHT = 48;	// Image height
        private static double MAX_SCALE = 3;		// Max scale 
        private static double MULTIPLIER = 45;		// Control the effectiveness of the mouse

        private List<Image> _images = new List<Image>();		// Store the added images

        public Menu()
        {
            Core.Instance.Menu = this;
            InitializeComponent();
            addImages();

            // start the mouse event handler
            this.MouseMove += new MouseEventHandler(handleMouseMove);
            //this.MouseLeftButtonUp += new MouseButtonEventHandler(handleMouseLeftButtonUp);
        }

        #region Handlers

        void handleMouseMove(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < _images.Count; i++)
            {
                Image image = _images[i];

                double dx = Math.Abs(e.GetPosition(this).X - ((double)image.GetValue(Canvas.LeftProperty) + image.Width / 2));
                double dy = Math.Abs(e.GetPosition(this).Y - ((double)image.GetValue(Canvas.TopProperty) + image.Height / 2));
                double dist = Math.Sqrt(dx * dx + dy * dy);

                // compute the scale of each image according to the mouse position
                double imageScale = MAX_SCALE - Math.Min(MAX_SCALE - 1, dist / MULTIPLIER);
                // resize the image
                resizeImage(image, IMAGE_WIDTH * imageScale, IMAGE_HEIGHT * imageScale, i, IMAGES.Length);

                // sort the children according to the scale
                image.SetValue(Canvas.ZIndexProperty, (int)Math.Round(IMAGE_WIDTH * imageScale));
            }
        }

        void handleMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < _images.Count; i++)
            {
                Image image = _images[i];

                double dx = Math.Abs(e.GetPosition(this).X - ((double)image.GetValue(Canvas.LeftProperty) + image.Width / 2));
                double dy = Math.Abs(e.GetPosition(this).Y - ((double)image.GetValue(Canvas.TopProperty) + image.Height / 2));
                double dist = Math.Sqrt(dx * dx + dy * dy);

                if (dist < IMAGE_WIDTH)
                {
                    //(this.Parent as PageSwitch).Navigate(new GamePage());
                }
            }            
        }

        void handleMenuImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (this.Parent as PageSwitch).Navigate(new GamePage());
        }

        private void openRegisterWindow(object sender, RoutedEventArgs e)
        {
            RegisterWindow rw = new RegisterWindow();
            rw.Show();
        }

        private void openLoginWindow(object sender, RoutedEventArgs e)
        {
            LoginWindow lw = new LoginWindow();
            lw.Show();
        }

        #endregion

        #region Public Methods
        #endregion

        #region  Private Methods

        // add the images to the stage
        private void addImages()
        {
            for (int i = 0; i < IMAGES.Length; i++)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(IMAGES[i], UriKind.Relative));

                // resize the image
                resizeImage(image, IMAGE_WIDTH, IMAGE_HEIGHT, i, IMAGES.Length);

                LayoutRoot.Children.Add(image);
                image.MouseLeftButtonUp += new MouseButtonEventHandler(handleMenuImage_MouseLeftButtonUp);
                _images.Add(image);
            }
        }

        // resize the image
        private void resizeImage(Image image, double imageWidth, double imageHeight, int index, int total)
        {
            image.Width = imageWidth;
            image.Height = imageHeight;

            image.SetValue(Canvas.TopProperty, Height / 2 - image.Height / 2);
            image.SetValue(Canvas.LeftProperty, Width / 2 + (index - (total - 1) / 2) * (MARGIN + IMAGE_WIDTH) - image.Width / 2);
        }

        #endregion

    }
}
