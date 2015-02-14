using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System;
using Financials.Common.Infrastucture;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class Extensions
    {
        public static IObservable<Unit> ToUnit<T>(this IObservable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source.Select(_=>Unit.Default);
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (string.IsNullOrEmpty(source)) return false;
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action )
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");
          
            foreach (var item in source)
            {
                action(item);
            }
        }

    }
}
