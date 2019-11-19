using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BreakingChange.Core.Extensions
{
    internal static class TypeSymbolExtensions
    {
        public static IEnumerable<IMethodSymbol> GetPublicMethods(this ITypeSymbol typeSymbol)
        {
            return typeSymbol
                .GetMembers()
                .Where(symbol => symbol.DeclaredAccessibility == Accessibility.Public)
                .OfType<IMethodSymbol>();
        }
    }
}
