using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace BreakingChange.Core.Comparers
{
    internal class TypeSymbolEqualityComparer : IEqualityComparer<INamedTypeSymbol>
    {
        public bool Equals(INamedTypeSymbol x, INamedTypeSymbol y)
        {
            return x.ToString() == y.ToString();
        }

        public int GetHashCode(INamedTypeSymbol obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
