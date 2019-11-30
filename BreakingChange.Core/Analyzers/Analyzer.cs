using System;
using System.Linq;
using System.Threading.Tasks;
using BreakingChange.Core.Comparers;
using BreakingChange.Core.Extensions;
using Microsoft.CodeAnalysis;

namespace BreakingChange.Core.Analyzers
{
    internal class Analyzer
    {
        public async Task Analyze(Solution newerSolution, Solution olderSolution)
        {
            var solutionsCompilations = await (newerSolution, olderSolution).ForBoth(solution => solution.GetCompilationsAsync()).ConfigureAwait(false);
            var (newerPublicTypeSymbols, olderPublicTypeSymbols) = await solutionsCompilations.ForBoth(compilation => compilation.GetPublicTypeSymbolsAsync()).ConfigureAwait(false);

            var typeSymbolsDiff = Differ.GetDiff(newerPublicTypeSymbols, olderPublicTypeSymbols, new TypeSymbolEqualityComparer());
            this.LogTypeSymbolsDiff(typeSymbolsDiff);

            var methodsDiffs = typeSymbolsDiff.Matching.Select(matchingTypes =>
            {
                var newerPublicMethods = matchingTypes.Newer.GetPublicMethods();
                var olderPublicMethods = matchingTypes.Older.GetPublicMethods();
                return Differ.GetDiff(newerPublicMethods, olderPublicMethods, new MethodSymbolEqualityComparer());
            });

            foreach (var methodDiff in methodsDiffs)
            {
                this.LogMethodDiff(methodDiff);
            }
        }

        private void LogTypeSymbolsDiff(Diff<INamedTypeSymbol> typeSymbolsDiff)
        {
            foreach (var typeSymbol in typeSymbolsDiff.Added)
            {
                Console.WriteLine($"Added {typeSymbol.TypeKind} {typeSymbol.ToString()}");
            }

            foreach (var typeSymbol in typeSymbolsDiff.Removed)
            {
                Console.WriteLine($"Removed {typeSymbol.TypeKind} {typeSymbol.ToString()}");
            }
        }

        private void LogMethodDiff(Diff<IMethodSymbol> methodDiff)
        {
            foreach (var method in methodDiff.Added)
            {
                Console.WriteLine($"Added {method.MethodKind} {method.Name}");
            }

            foreach (var method in methodDiff.Removed)
            {
                Console.WriteLine($"Removed {method.MethodKind} {method.Name}");
            }
        }
    }
}
