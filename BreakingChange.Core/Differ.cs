using System.Collections.Generic;
using System.Linq;

namespace BreakingChange.Core
{
    internal class Differ
    {
        private enum DiffResult
        {
            Added,
            Removed,
            Matching,
        }

        public static Diff<T> GetDiff<T>(IEnumerable<T> newerElements, IEnumerable<T> olderElements, IEqualityComparer<T> comparer)
           where T : class
        {
            var diffStream = GetDiffStream(newerElements, olderElements, comparer);
            var diffGroups = diffStream
                .ToLookup(match =>
                {
                    if (match.NewerMatch != null)
                    {
                        if (match.OlderMatch != null)
                        {
                            return DiffResult.Matching;
                        }

                        return DiffResult.Added;
                    }

                    return DiffResult.Removed;
                });
            var added = diffGroups[DiffResult.Added].Select(match => match.NewerMatch!);
            var removed = diffGroups[DiffResult.Removed].Select(match => match.OlderMatch!);
            var matching = diffGroups[DiffResult.Matching].Cast<(T, T)>();

            return new Diff<T>(added, removed, matching);
        }

        private static IEnumerable<(T? OlderMatch, T? NewerMatch)> GetDiffStream<T>(IEnumerable<T> newerElements, IEnumerable<T> olderElements, IEqualityComparer<T> comparer)
           where T : class
        {
            var olderElementsList = olderElements.ToList();

            foreach (var newerElement in newerElements)
            {
                var olderMatchingElement = olderElementsList.FirstOrDefault(olderElement => comparer.Equals(olderElement, newerElement));
                if (olderMatchingElement == default)
                {
                    yield return (null, newerElement);
                }
                else
                {
                    yield return (olderMatchingElement, newerElement);
                    olderElementsList.Remove(olderMatchingElement);
                }
            }

            foreach (var olderNotMatchedElement in olderElementsList)
            {
                yield return (olderNotMatchedElement, null);
            }
        }
    }
}
