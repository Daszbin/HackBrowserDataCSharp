using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeUtil
{
    public static class TypeUtils
    {
        public static List<K> Keys<K, V>(Dictionary<K, V> dict)
        {
            return dict.Keys.ToList();
        }

        public static bool IntToBool(long a)
        {
            return a switch
            {
                0 or -1 => false,
                _ => true,
            };
        }

        public static List<T> Reverse<T>(List<T> list)
        {
            List<T> reversed = new(list.Count);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                reversed.Add(list[i]);
            }
            return reversed;
        }

        public static DateTime TimeStamp(long stamp)
        {
            DateTime s = DateTimeOffset.FromUnixTimeSeconds(stamp).LocalDateTime;
            return s.Year > 9999 ? new DateTime(9999, 12, 13, 23, 59, 59) : s;
        }

        public static DateTime TimeEpoch(long epoch)
        {
            long maxTime = 99633311740000000;
            if (epoch > maxTime)
            {
                return new DateTime(2049, 1, 1, 1, 1, 1, 1);
            }
            DateTime t = new(1601, 1, 1, 0, 0, 0, DateTimeKind.Local);
            TimeSpan d = TimeSpan.FromTicks(epoch);
            for (int i = 0; i < 1000; i++)
            {
                t = t.Add(d);
            }
            return t;
        }
    }
}
