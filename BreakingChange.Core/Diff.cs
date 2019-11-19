using System;
using System.Collections.Generic;
using System.Linq;

namespace BreakingChange.Core
{
    internal class Diff<T>
    {
        public Diff(
            IEnumerable<T> added,
            IEnumerable<T> removed,
            IEnumerable<(T Newer, T Older)> matching)
        {
            if (added is null)
            {
                throw new ArgumentNullException(nameof(added));
            }

            if (removed is null)
            {
                throw new ArgumentNullException(nameof(removed));
            }

            if (matching is null)
            {
                throw new ArgumentNullException(nameof(matching));
            }

            this.Added = added.ToList();
            this.Removed = removed.ToList();
            this.Matching = matching.ToList();
        }

        public IReadOnlyList<T> Added { get; }

        public IReadOnlyList<T> Removed { get; }

        public IReadOnlyList<(T Newer, T Older)> Matching { get; }
    }
}
