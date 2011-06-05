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
using System.Threading;

namespace WebColumns.Logic
{
    public class Board
    {
        /// <summary>
        /// Event, wenn ein neues Vorschautripel angezeigt werden soll
        /// </summary>
        public event NewPreviewAvailable OnNewPreviewAvailable;

        /// <summary>
        /// Event, wenn sich die Punktzahl ändert
        /// </summary>
        public event ScoreChanged OnScoreChanged;

        /// <summary>
        /// Event, wenn neue Elemente hinzukommen
        /// </summary>
        public event ElementsAdded OnElementsAdded;

        /// <summary>
        /// Event, wenn Elemente entfernt werden
        /// </summary>
        public event ElementsRemoved OnElementsRemoved;

        private List<Element> _elements = new List<Element>();
        private List<Element> _moveElements = new List<Element>();
        private List<Element> _checkElements = new List<Element>();
        private Triple _currentTriple;
        private Triple _previewTriple;
        private BoardMode _mode;

        private int _score = 0;
        private int _linkedCount = 0;
        private int _level = 0;

        private Timer _timer;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Board()
        {
        }

        #region Public Methoden

        /// <summary>
        /// Spielstart: Spielfeld initialisieren, Timer starten
        /// </summary>
        public void Init()
        {
            _mode = BoardMode.ElementMove;
            _previewTriple = Triple.GenerateRandomTriple();
            GenerateNewTriple();

            _timer = new Timer(new System.Threading.TimerCallback(TimerEvent), null, 0, 100);
        }

        /// <summary>
        /// Timer-Intervall verändern
        /// </summary>
        /// <param name="interval">Neues Intervall</param>
        public void ChangeTimer(int interval)
        {
            _timer.Change(interval, interval);
        }

        /// <summary>
        /// Timer stoppen
        /// </summary>
        public void StopTimer()
        {
            _timer.Change(0, Timeout.Infinite);
        }

        #endregion

        #region Private Methoden

        /// <summary>
        /// Timer-Eventhandler (Zustandsautomat)
        /// </summary>
        /// <param name="o"></param>
        private void TimerEvent(object o)
        {
            switch (_mode)
            {
                case BoardMode.ElementMove:
                case BoardMode.CheckingMove:
                    MoveElements(0, 1);
                    break;
                case BoardMode.LinkChecking:
                    StopTimer();
                    CheckElementLinks();
                    ChangeTimer(100);
                    break;
            }
        }

        /// <summary>
        /// Prüft, ob Verbindungen gleicher Elemente erzielt wurden
        /// </summary>
        private void CheckElementLinks()
        {
            List<Element> linked = new List<Element>();

            int[] dx = new int[] { -1, +1, +0, +0, -1, +1, -1, +1 };
            int[] dy = new int[] { +0, +0, -1, +1, -1, -1, +1, +1 };

            foreach (Element elem in _checkElements)
            {
                Point loc = elem.Location;
                int lx = (int)loc.X; int ly = (int)loc.Y;
                for (int i = 0; i < 7; i++)
                {
                    List<Element> match = new List<Element>();
                    int count = RecursiveCheck(elem.Color, lx, ly, dx[i], dy[i], ref match);
                    if (count >= 2)
                    {
                        linked.AddRange(match);
                        if (!linked.Contains(elem)) linked.Add(elem);
                    }
                }
            }
            if (linked.Count > 0)
            {
                if (OnElementsRemoved != null) OnElementsRemoved(linked);
                foreach (Element elem in linked)
                {
                    _elements.Remove(elem);
                }
            }
            _checkElements.Clear();
            // moveElements füllen mit Elementen mit Freiraum darunter
            _moveElements.Clear();
            foreach (Element elem in _elements)
            {
                Point loc = elem.Location;
                int cx = (int)loc.X; int cy = (int)loc.Y;
                if (!LocationBlocked((int)elem.Location.X, (int)elem.Location.Y + 1))
                {
                    for (int i = cy; i >= 0; i--)
                    {
                        foreach (Element ontop in _elements)
                        {
                            Point otloc = elem.Location;
                            int ox = (int)otloc.X; int oy = (int)otloc.Y;
                            if (ox == cx && oy == i) _moveElements.Add(ontop);
                        }
                    }
                }
            }
            _mode = BoardMode.CheckingMove;
        }

        private int RecursiveCheck(ElementColor color, int lx, int ly, int dx, int dy, ref List<Element> match)
        {
            foreach (Element elem in _elements)
            {
                Point loc = elem.Location;
                int cx = (int)loc.X; int cy = (int)loc.Y;
                if (elem.Color == color && cx == lx + dx && cy == ly + dy)
                {
                    match.Add(elem);
                    return RecursiveCheck(color, cx, cy, dx, dy, ref match) + 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Bewegt Elemente
        /// </summary>
        public void MoveElements(int dx, int dy)
        {
            if (_mode != BoardMode.ElementMove) return;
            for (int i = _moveElements.Count - 1; i >= 0; i--)
            {
                Element elem = _moveElements[i];
                elem.Move(dx, dy);
                if (elem.Position.Y % BoardControl.TILESIZE == 0 && LocationBlocked((int)elem.Location.X, (int)elem.Location.Y + 1))
                {
                    _checkElements.Add(elem);
                    _moveElements.RemoveAt(i);
                }
            }
            if (_moveElements.Count == 0)
            {
                if (_mode == BoardMode.ElementMove)
                    _mode = BoardMode.LinkChecking;
                else if (_mode == BoardMode.CheckingMove)
                {
                    _mode = BoardMode.ElementMove;
                    GenerateNewTriple();
                }
            }
        }

        /// <summary>
        /// Vorschau-Tripel übernehmen und neues generieren
        /// </summary>
        private void GenerateNewTriple()
        {
            _currentTriple = _previewTriple;
            _previewTriple = Triple.GenerateRandomTriple();
            _moveElements.Clear();  // unnötig?
            _moveElements.AddRange(_currentTriple.Elements);
            if (OnNewPreviewAvailable != null) OnNewPreviewAvailable(_previewTriple);
            if (OnElementsAdded != null) OnElementsAdded(_moveElements);
        }

        /// <summary>
        /// Prüft, ob der angegebene Sektor bereits blockiert ist
        /// </summary>
        /// <param name="lx">X-Koordinate des Sektors</param>
        /// <param name="ly">Y-Koordinate des Sektors</param>
        /// <returns>True/False</returns>
        private bool LocationBlocked(int lx, int ly)
        {
            foreach (Element elem in _elements)
            {
                Point loc = elem.Location;
                if (loc.X == lx && loc.Y == ly) return true;
            }
            return false;
        }

        #endregion

    }
}
