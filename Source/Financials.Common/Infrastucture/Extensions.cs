using System.Reactive;
using System.Reactive.Linq;
using System;

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
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
