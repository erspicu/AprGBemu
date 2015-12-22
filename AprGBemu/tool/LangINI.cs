using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace AprGBemu
{
    public class LangINI
    {

        static string LangFile = Application.StartupPath + "/AprGBemuLang.ini";
        static List<string> lines = File.ReadAllLines(LangFile).ToList();
        static List<string> langs = new List<string>();
        public static Dictionary<string, string> lang_map = new Dictionary<string, string>();
        public static Dictionary<string, Dictionary<string, string>> lang_table = new Dictionary<string, Dictionary<string, string>>();
        static bool finished = false;

        public static void init()
        {
            if (finished)
                return;

            Dictionary<string, string> items = new Dictionary<string, string>();
            string lang = "";

            bool start = false;
            foreach (string i in lines)
            {
                string l = i.Replace("\r", "").Replace("\n", "");

                if (start == true)
                {

                    List<string> keyvalue = i.Split(new char[] { '=' }).ToList();

                    if (keyvalue.Count == 2)
                    {
                        lang_table[lang].Add(keyvalue[0], keyvalue[1]);
                        if (keyvalue[0] == "lang")
                            lang_map.Add(lang, keyvalue[1]);
                    }

                    if (l.StartsWith("[") && l.EndsWith("]"))
                        start = false;
                }
                if ((l.StartsWith("[") && l.EndsWith("]")) && start != true)
                {
                    start = true;
                    lang = l.Replace("[", "").Replace("]", "");
                    lang_table.Add(lang, new Dictionary<string, string>());
                    langs.Add(lang);
                }
            }
            finished = true;
        }
    }
}
