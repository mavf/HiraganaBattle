using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Helpers.Control
{
    public class KeysQueue
    {
        private List<string> list = new List<string>(3);

        public string Text
        {
            get
            {
                string s = string.Empty;

                list.ForEach(x => s += x);

                return s;
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public void Add(string s)
        {
            list.Add(s);
        }

        public void Clear()
        {
            list.Clear();
        }


    }
}
