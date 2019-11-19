using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace BreakingChange.Core.Comparers
{
    internal class MethodSymbolEqualityComparer : IEqualityComparer<IMethodSymbol>
    {
        public bool Equals(IMethodSymbol x, IMethodSymbol y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(IMethodSymbol obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
