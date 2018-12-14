using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace QuizApp.Extension
{
    public static class ListExtension 
    {
        public static void ShuffleList<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static List<T> ChangeModel<U, T>(this IList<U> list, Func<U, int, T> FactoryMethod)
        {
            var outputList = new List<T>();
            var index = 0;
            foreach (var item in list)
                outputList.Add(FactoryMethod(item, ++index));
            return outputList;
        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random random;
        public static Random Random => random ?? (random = new Random(unchecked((int)DateTime.Now.Ticks)));
    }
}