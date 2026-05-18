using System;
using System.Collections.Generic;
using System.Linq;

namespace BoyAfraidOfGhosts.Helpers
{
    public static class FunctionalHelpers
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
            where T : class
        {
            return source.Where(item => item != null);
        }

        public static Optional<T> FirstOrNone<T>(
            this IEnumerable<T> source,
            Func<T, bool> predicate)
        {
            var result = source.FirstOrDefault(predicate);
            if (result == null)
                return Optional<T>.None();
            return Optional<T>.Some(result);
        }

        public static IEnumerable<TResult> Map<T, TResult>(
            this IEnumerable<T> source,
            Func<T, TResult> selector)
        {
            return source.Select(selector);
        }

        public static T Apply<T>(this T value, Action<T> action)
        {
            action(value);
            return value;
        }

        public static TResult Pipe<T, TResult>(
            this T value,
            Func<T, TResult> function)
        {
            return function(value);
        }
    }
}