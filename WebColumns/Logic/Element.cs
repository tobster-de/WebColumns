using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace WebColumns.Logic
{
    public class Element
    {
        private ElementColor _color;
        private Point _position;
        //private Image _image;

        ///// <summary>
        ///// Anzeigebild
        ///// </summary>
        //public Image Image
        //{
        //    get { return _image; }
        //    private set { _image = value; }
        //}

        /// <summary>
        /// Farbe des Elements
        /// </summary>
        public ElementColor Color
        {
            get { return _color; }
            private set
            {
                _color = value;
            }
        }

        /// <summary>
        /// Position des Elements (in Pixeln)
        /// </summary>
        public Point Position
        {
            get { return _position; }
            private set { _position = value; }
        }

        /// <summary>
        /// Position des Elements (in Sektoren)
        /// </summary>
        public Point Location
        {
            get
            {
                return new Point((_position.X + BoardControl.TILESIZE / 2) / BoardControl.TILESIZE,
                    (_position.Y + BoardControl.TILESIZE / 2) / BoardControl.TILESIZE);
            }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="color">Farbe des Elements</param>
        /// <param name="x">Position X-Koordinate</param>
        /// <param name="y">Position Y-Koodinate</param>
        public Element(ElementColor color, int x, int y)
        {
            _position = new Point(x, y);
            Color = color;
        }

        /// <summary>
        /// Bewegen
        /// </summary>
        /// <param name="dx">Änderung der X-Koordinate</param>
        /// <param name="dy">Änderung der Y-Koordinate</param>
        internal void Move(int dx, int dy)
        {
            _position.X += dx;
            _position.Y += dy;
            //_image.SetValue(Canvas.LeftProperty, _position.X);
            //_image.SetValue(Canvas.TopProperty, _position.Y);
        }
    }
}
