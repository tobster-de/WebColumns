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
        private Location _location;

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
        /// Position des Elements (in Sektoren)
        /// </summary>
        public Location Location
        {
            get
            {
                return new Location(_location.X, _location.Y);
            }
            set
            {
                _location = new Location(value.X, value.Y);
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
            _location = new Location(x, y);
            Color = color;
        }

        /// <summary>
        /// Bewegen
        /// </summary>
        /// <param name="dx">Änderung der X-Koordinate</param>
        /// <param name="dy">Änderung der Y-Koordinate</param>
        internal void Move(int dx, int dy)
        {
            int cx = (int)_location.X + dx;
            int cy = (int)_location.Y + dy;
            if (cx < 0 || cx > 6 || cy > 14) return;
            _location.X += dx;
            _location.Y += dy;
        }

        public override string ToString()
        {
            return Color.ToString() + " Element at " + (int)Location.X + "/" + (int)Location.Y;
        }
    }
}
