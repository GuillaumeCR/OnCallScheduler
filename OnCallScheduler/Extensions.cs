using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnCallScheduler
{
    public static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list, int start, int end)
        {
            shuffle(list, start, end);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            shuffle(list, 0, list.Count - 1);
        }

        private static void shuffle<T>(IList<T> list, int start, int end)
        {
            if (start < 0 || start > list.Count - 2)
            {
                throw new ArgumentException("start " + start + " doesn't follow he rule 0 <= start <= list.Count() - 3");
            }

            if (end < 1 || end >= list.Count)
            {
                throw new ArgumentException("end " + end + " doesn't follow the rule 2 < end < list.Count()");
            }

            Random rng = new Random();
            int n = end - start;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1) + start;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
