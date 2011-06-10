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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WebColumns.Logic
{
    public class Board
    {
        const int SIZEX = 7;
        const int SIZEY = 14;

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

        /// <summary>
        /// Event, wenn Elemente bewegt werden
        /// </summary>
        public event ElementsMoved OnElementsMoved;

        private List<Element> _elements = new List<Element>();
        private List<Element> _moveElements = new List<Element>();
        //private List<Element> _checkElements = new List<Element>();
        private Triple _currentTriple;
        private Triple _previewTriple;
        private BoardMode _mode;

        private int _score = 0;             // Punktezahl
        private int _linkedCount = 0;       // Anzahl eleminierte Elemente
        private int _level = 0;             // Spielstufe (_linked div 50)
        private int _linkChain = 0;         // Stufe der Kettenreaktion 

        private Timer _timer;


        /// <summary>
        /// Aktueller Arbeitsmodus
        /// </summary>
        public BoardMode Mode
        {
            get { return _mode; }
            private set { _mode = value; }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Board() { }

        #region Public Methoden

        /// <summary>
        /// Spielstart: Spielfeld initialisieren, Timer starten
        /// </summary>
        public void Init()
        {
            _mode = BoardMode.ElementMove;
            _previewTriple = Triple.GenerateRandomTriple();
            GenerateNewTriple();

            _timer = new Timer(new System.Threading.TimerCallback(TimerEvent), null, 0, 500);
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
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Aktuelles Tripel togglen
        /// </summary>
        /// 
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ToggleTriple()
        {
            Debug.WriteLine("toggle");
            Location loc = _currentTriple[0].Location;
            _currentTriple[0].Location = _currentTriple[1].Location;
            _currentTriple[1].Location = _currentTriple[2].Location;
            _currentTriple[2].Location = loc;
            if (OnElementsMoved != null) OnElementsMoved(_currentTriple.Elements);
        }

        /// <summary>
        /// Aktuelles Tripel ein Feld nach unten bewegen
        /// </summary>
        public void DropTriple()
        {
            if (_mode != BoardMode.ElementMove) return;
            MoveElements(0, 1);
        }

        /// <summary>
        /// Aktuelles Tripel ein Feld nach links bewesen
        /// </summary>
        public void MoveTripleLeft()
        {
            if (_mode != BoardMode.ElementMove) return;
            MoveElements(-1, 0);
        }

        /// <summary>
        /// Aktuelles Tripel ein Feld nach rechts bewegen
        /// </summary>
        public void MoveTripleRight()
        {
            if (_mode != BoardMode.ElementMove) return;
            MoveElements(1, 0);
        }

        #endregion

        #region Private Methoden

        /// <summary>
        /// Timer-Eventhandler (Zustandsautomat)
        /// </summary>
        /// <param name="o"></param>
        private void TimerEvent(object o)
        {
            Debug.WriteLine("timer " + DateTime.Now.Millisecond + " " + _mode);
            switch (_mode)
            {
                case BoardMode.ElementMove:
                    //case BoardMode.CheckingMove:
                    MoveElements(0, 1);
                    break;
                case BoardMode.LinkChecking:
                    StopTimer();
                    CheckElementLinks();
                    ChangeTimer(1000);
                    break;
            }
        }

        /// <summary>
        /// Prüft, ob Verbindungen gleicher Elemente erzielt wurden
        /// </summary>
        private void CheckElementLinks()
        {
            List<Element> linked = new List<Element>();
            List<int> checkCols = new List<int>();

            int[] dx = new int[] { -1, 0, 1, 1 };
            int[] dy = new int[] { 1, 1, 1, 0 };

            foreach (Element elem in _elements)
            {
                // alle möglichen Richtungen ausgehen von diesem Element überprüfen 
                for (int i = 0; i < dx.Length; i++)
                {
                    List<Element> match = new List<Element>();
                    int count = RecursiveCheck(elem, dx[i], dy[i], ref match);
                    if (count >= 2)
                    {
                        match.Add(elem);
                        foreach (Element m in match)
                            if (!linked.Contains(m)) linked.Add(m);
                    }
                }
            }
            if (linked.Count > 0)
            {
                // verbundene Elemente eleminieren
                List<Element> moved = new List<Element>();
                _linkChain++;
                if (OnElementsRemoved != null) OnElementsRemoved(linked);
                foreach (Element elem in linked)
                {
                    _elements.Remove(elem);
                    if (!checkCols.Contains(elem.Location.X)) checkCols.Add(elem.Location.X);
                }
                // darüber liegende Elemente nachrücken
                foreach (int x in checkCols)
                {
                    for (int y = SIZEY - 1; y >= 0; y--)
                    {
                        Element elem = ElementAtLocation(x, y);
                        if (elem != null)
                        {
                            int j = y;
                            while (!LocationBlocked(elem.Location.X, elem.Location.Y + 1))
                            {
                                elem.Move(0, 1);
                                if (!moved.Contains(elem)) moved.Add(elem);
                            }
                        }
                    }
                }
                if (moved.Count > 0 && OnElementsMoved != null) OnElementsMoved(moved);

                //Punktewertung
                int roundscore = (linked.Count - 2) * 30 * (_linkChain) * (_level + 1);
                _score += roundscore;
                if (_linkedCount / 50 < (_linkedCount + linked.Count) / 50) _level++;
                _linkedCount += linked.Count;
                if (OnScoreChanged != null) OnScoreChanged(roundscore, _score, _linkedCount, _level);
            }
            else
            {
                _mode = BoardMode.ElementMove;
                GenerateNewTriple();
            }
        }

        private int RecursiveCheck(Element e, int dx, int dy, ref List<Element> match)
        {
            if (e.Location.X + dx < 0 || e.Location.X + dx >= SIZEX
                || e.Location.X + dy < 0 || e.Location.Y + dy >= SIZEY) return 0;
            foreach (Element elem in _elements)
            {
                Location loc = elem.Location;
                if (elem.Color == e.Color && loc.X == e.Location.X + dx && loc.Y == e.Location.Y + dy)
                {
                    if (!match.Contains(elem)) match.Add(elem);
                    return RecursiveCheck(elem, dx, dy, ref match) + 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Bewegt Elemente
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void MoveElements(int dx, int dy)
        {
            _moveElements.Sort(new Comparison<Element>(delegate(Element a, Element b)
            {
                return a.Location.Y - b.Location.Y;
            }));
            List<Element> _moved = new List<Element>(_moveElements);
            for (int i = _moveElements.Count - 1; i >= 0; i--)
            {
                Element elem = _moveElements[i];
                Debug.WriteLine(elem);
                elem.Move(dx, dy);
                if (LocationBlocked(elem.Location.X, elem.Location.Y + 1))
                {
                    //_checkElements.Add(elem);
                    _elements.Add(elem);
                    _moveElements.RemoveAt(i);
                }
            }
            if (OnElementsMoved != null && _moved.Count > 0) OnElementsMoved(_moved);
            if (_moveElements.Count == 0)
            {
                if (_mode == BoardMode.ElementMove)
                    _mode = BoardMode.LinkChecking;
                //else if (_mode == BoardMode.CheckingMove)
                //{
                //    _mode = BoardMode.ElementMove;
                //    GenerateNewTriple();
                //}
            }
        }

        /// <summary>
        /// Vorschau-Tripel übernehmen und Neues generieren
        /// </summary>
        private void GenerateNewTriple()
        {
            Debug.WriteLine("generate triple");
            _linkChain = 0;
            _currentTriple = _previewTriple;
            _previewTriple = Triple.GenerateRandomTriple();
            _moveElements.Clear();  // unnötig?
            _moveElements.AddRange(_currentTriple.Elements);
            if (OnNewPreviewAvailable != null) OnNewPreviewAvailable(_previewTriple);
            if (OnElementsAdded != null) OnElementsAdded(_moveElements);
        }

        /// <summary>
        /// Bestimmt das Element an einer bestimmten Stelle
        /// </summary>
        /// <param name="loc">Stelle an der das Element bestimmt werden soll</param>
        /// <returns>Element an gegebener Stelle oder Null</returns>
        private Element ElementAtLocation(int lx, int ly)
        {
            if (lx < 0 || lx >= SIZEX || ly >= SIZEY) return null;
            foreach (Element elem in _elements)
            {
                Location loc = elem.Location;
                if (loc.X == lx && loc.Y == ly) return elem;
            }
            return null;
        }

        /// <summary>
        /// Prüft, ob der angegebene Sektor bereits blockiert ist
        /// </summary>
        /// <param name="lx">X-Koordinate des Sektors</param>
        /// <param name="ly">Y-Koordinate des Sektors</param>
        /// <returns>True/False</returns>
        private bool LocationBlocked(int lx, int ly)
        {
            if (lx < 0 || lx >= SIZEX || ly >= SIZEY) return true;
            foreach (Element elem in _elements)
            {
                Location loc = elem.Location;
                if (loc.X == lx && loc.Y == ly) return true;
            }
            return false;
        }

        #endregion

    }
}
