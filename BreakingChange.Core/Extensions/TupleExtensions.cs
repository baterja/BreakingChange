using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreakingChange.Core.Extensions
{
    internal static class TupleExtensions
    {
        public static void Deconstruct<T>(this T[] objects, out T first, out T second)
        {
            if (objects.Length != 2)
            {
                throw new Exception("wtf!"); // TODO
            }

            (first, second) = (objects[0], objects[1]);
        }

        public static (TOutput newerResult, TOutput olderResult) ForBoth<TInput, TOutput>(this (TInput newer, TInput older) tuple, Func<TInput, TOutput> func)
        {
            return (func(tuple.newer), func(tuple.older));
        }

        public static (IEnumerable<TOutput> newerResult, IEnumerable<TOutput> olderResult) ForBoth<TInput, TOutput>(this (IEnumerable<TInput> newer, IEnumerable<TInput> older) tuple, Func<TInput, TOutput> func)
        {
            return (tuple.newer.Select(func), tuple.older.Select(func));
        }

        public static async Task<(TOutput newerResult, TOutput olderResult)> ForBoth<TInput, TOutput>(this (TInput newer, TInput older) tuple, Func<TInput, Task<TOutput>> func)
        {
            var (newerResult, olderResult) = await Task.WhenAll(func(tuple.newer), func(tuple.older)).ConfigureAwait(false);
            return (newerResult, olderResult);
        }

        public static async Task<(IEnumerable<TOutput> newerResult, IEnumerable<TOutput> olderResult)> ForBoth<TInput, TOutput>(this (IEnumerable<TInput> newer, IEnumerable<TInput> older) tuple, Func<TInput, Task<TOutput>> func)
        {
            var (newerTasks, olderTasks) = tuple.ForBoth(element => element.Select(func).ToList());
            var newerResult = await Task.WhenAll(newerTasks).ConfigureAwait(false);
            var olderResult = await Task.WhenAll(olderTasks).ConfigureAwait(false);
            return (newerResult, olderResult);
        }

        public static async Task<(IEnumerable<TOutput> newerResult, IEnumerable<TOutput> olderResult)> ForBoth<TInput, TOutput>(this (IEnumerable<TInput> newer, IEnumerable<TInput> older) tuple, Func<TInput, Task<IEnumerable<TOutput>>> func)
        {
            var (newerResults, olderResults) = await tuple.ForBoth(async element => await Task.WhenAll(element.Select(func)).ConfigureAwait(false)).ConfigureAwait(false);
            return (newerResults.SelectMany(x => x), olderResults.SelectMany(x => x));
        }
    }
}
