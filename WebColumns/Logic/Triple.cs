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
using System.Collections.Generic;

namespace WebColumns.Logic
{
    public class Triple
    {
        private static Random _random = new Random();
        private List<Element> _elements = new List<Element>(3);

        /// <summary>
        /// Liest die enthaltenen Elemente aus
        /// </summary>
        public List<Element> Elements
        {
            get { return _elements; }
        }

        /// <summary>
        /// Indexzugriff auf Elemente
        /// </summary>
        /// <param name="index">Index des gewünschten Elements</param>
        /// <returns>Element an Index</returns>
        public Element this[int index]
        {
            get
            {
                if (index < 0 || index > 2) throw new ArgumentOutOfRangeException("index");
                return _elements[index];
            }
        }

        /// <summary>
        /// Erzeuge ein neues zufälliges Tripel
        /// </summary>
        /// <returns></returns>
        public static Triple GenerateRandomTriple()
        {
            Triple t = new Triple();
            for (int i = 0; i < 3; i++)
            {
                ElementColor col = (ElementColor)_random.Next(5);
                t._elements.Add(new Element(col, 3, i));
            }
            return t;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        private Triple() { }

        ///// <summary>
        ///// Reihenfolge der Elemente ändern
        ///// </summary>
        //public void Toggle()
        //{
        //    _elements.Add(_elements[0]);
        //    _elements.RemoveAt(0);
        //}

        /// <summary>
        /// Bewegen
        /// </summary>
        /// <param name="dx">Änderung der X-Koordinate</param>
        /// <param name="dy">Änderung der Y-Koordinate</param>
        public void Move(int dx, int dy)
        {
            foreach (Element e in _elements)
            {
                e.Move(dx, dy);
            }
        }

    }
}
