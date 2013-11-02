using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class ExtensionsHelper
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        public static T SelectRandom<T>(this List<T> list)
        {
            int count = list.Count;

            //Random r = new Random();

            lock (syncLock)
            {
                return list[random.Next(count)];
            }
        }

    }
}
