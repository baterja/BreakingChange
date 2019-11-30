using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace BreakingChange.Core.Extensions
{
    internal static class TypeSymbolExtensions
    {
        public static IEnumerable<IMethodSymbol> GetPublicMethods(this ITypeSymbol typeSymbol)
        {
            return typeSymbol
                .GetPublicSymbols()
                .OfType<IMethodSymbol>();
        }

        public static IEnumerable<ISymbol> GetPublicSymbols(this ITypeSymbol typeSymbol)
        {
            return typeSymbol
                .GetMembers()
                .Where(symbol => symbol.DeclaredAccessibility == Accessibility.Public);
        }
    }
}
