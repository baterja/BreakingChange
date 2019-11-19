using System.Collections.Generic;
using System.Linq;

namespace BreakingChange.Core
{
    internal class Differ
    {
        public static Diff<T> GetDiff<T>(IEnumerable<T> newerElements, IEnumerable<T> olderElements, IEqualityComparer<T> comparer)
           where T : class
        {
            var added = new List<T>();
            var matching = new List<(T Newer, T Older)>();

            var olderElementsList = olderElements.ToList();

            foreach (var newerElement in newerElements)
            {
                var olderMatchingElement = olderElementsList.FirstOrDefault(olderElement => comparer.Equals(olderElement, newerElement));
                if (olderMatchingElement == default)
                {
                    added.Add(newerElement);
                }
                else
                {
                    matching.Add((newerElement, olderMatchingElement));
                }
            }

            var removed = olderElementsList.Where(olderElement => !matching.Any(matchingElements => comparer.Equals(matchingElements.Older, olderElement)));

            return new Diff<T>(added, removed, matching);
        }
    }
}
