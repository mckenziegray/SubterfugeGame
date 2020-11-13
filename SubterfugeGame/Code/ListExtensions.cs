using System;
using System.Collections.Generic;

namespace SubterfugeGame.Code
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            for (int i = list.Count; i > 0; i--)
            {
                int r = random.Next(0, i);
                T temp = list[0];
                list[0] = list[r];
                list[r] = temp;
            }
        }
    }
}
