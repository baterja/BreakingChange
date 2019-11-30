using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BreakingChange.Core.Extensions
{
    internal static class MethodSymbolExtensions
    {
        public static IEnumerable<IMethodSymbol> OfKind(this IEnumerable<IMethodSymbol> methodSymbols, MethodKind kind)
        {
            return methodSymbols.Where(methodSymbol => methodSymbol.MethodKind == kind);
        }
    }
}
