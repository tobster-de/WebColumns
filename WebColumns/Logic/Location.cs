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

namespace WebColumns.Logic
{
    public class Location
    {
        int _x;

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        int _y;

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Location(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Location)) return false;
            Location other = (Location)obj;
            return other._x == _x && other._y == _y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return _x.ToString() + "/" + _y.ToString();
        }
    }
}
