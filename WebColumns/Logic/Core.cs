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
using System.Windows.Browser;

namespace WebColumns.Logic
{
    public class Core
    {
        private static Core _instance = null;
        private Menu _menu = null;

        private string _playerName;
        private int _playerID;

        /// <summary>
        /// Singleton-Instanz
        /// </summary>
        public static Core Instance
        {
            get
            {
                if (_instance == null) _instance = new Core();
                return _instance;
            }
        }

        /// <summary>
        /// Setzt oder liest das Menu-Control-Objekt
        /// </summary>
        public Menu Menu
        {
            get
            {
                if (_menu == null) _menu = new Menu();
                return _menu;
            }
            set
            {
                if (_menu != null) _menu = value;
            }
        }

        private Core() { }

        private void SetCookie(string key, string value)
        {
            // Expire in 7 days
            DateTime expireDate = DateTime.Now + TimeSpan.FromDays(7);

            string newCookie = key + "=" + value + ";expires=" + expireDate.ToString("R");
            HtmlPage.Document.SetProperty("cookie", newCookie);
        }

        private string GetCookie(string key)
        {
            string[] cookies = HtmlPage.Document.Cookies.Split(';');

            foreach (string cookie in cookies)
            {
                string[] keyValue = cookie.Split('=');
                if (keyValue.Length == 2 && keyValue[0].ToString() == key) return keyValue[1];
            }
            return null;
        }
    }
}
