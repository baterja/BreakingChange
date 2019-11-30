using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreakingChange.Core.Comparers;
using BreakingChange.Core.Extensions;
using Microsoft.CodeAnalysis;

namespace BreakingChange.Core.Analyzers
{
    internal class Analyzer
    {
        // TODO as extension
        public async Task Analyze(Project newerProject, Project olderProject)
        {
            var (newerCompilation, olderCompilation) = await (newerProject, olderProject).ForBoth(project => project.GetCompilationAsync()).ConfigureAwait(false);
            // TODO if null then error
            await this.Analyze(new[] { newerCompilation }, new[] { olderCompilation }).ConfigureAwait(false);
        }

        // TODO as extension
        public async Task Analyze(Solution newerSolution, Solution olderSolution)
        {
            var (newerCompilations, olderCompilations) = await (newerSolution, olderSolution).ForBoth(solution => solution.GetCompilationsAsync()).ConfigureAwait(false);
            await this.Analyze(newerCompilations, olderCompilations).ConfigureAwait(false);
        }

        public async Task Analyze(IEnumerable<Compilation> newerCompilations, IEnumerable<Compilation> olderCompilations)
        {
            var (newerPublicTypeSymbols, olderPublicTypeSymbols) = await (newerCompilations, olderCompilations).ForBoth(compilation => compilation.GetPublicTypeSymbolsAsync()).ConfigureAwait(false);

            var typeSymbolsDiff = Differ.GetDiff(newerPublicTypeSymbols, olderPublicTypeSymbols, new TypeSymbolEqualityComparer());
            this.LogTypeSymbolsDiff(typeSymbolsDiff);

            var matchingTypes = typeSymbolsDiff.Matching;
            var matchingTypesPublicMethods = matchingTypes
                .Select(matchingTypes => matchingTypes.ForBoth(typeSymbol => typeSymbol.GetPublicMethods().ToList()))
                .ToList();

            var matchingTypesOrdinaryMethods = matchingTypesPublicMethods.Select(publicTypes => publicTypes.ForBoth(methodSymbols => methodSymbols.OfKind(MethodKind.Ordinary)));
            var ordinaryMethodEqualityComparer = new MethodSymbolEqualityComparer();
            var ordinaryMethodsDiffs = matchingTypesOrdinaryMethods.Select(publicTypes =>
            {
                return Differ.GetDiff(publicTypes.newerResult, publicTypes.olderResult, ordinaryMethodEqualityComparer);
            });

            foreach (var methodDiff in ordinaryMethodsDiffs)
            {
                this.LogMethodDiff(methodDiff);
            }

            //var matchingOrdinaryMethods = ordinaryMethodsDiffs.Select(diff => diff.Matching);
            //var matchingOrdinaryMethodsDetailedComparisons = matchingOrdinaryMethods.Select(matchingOrdinaryMethods =>
            //{
            //    return matchingOrdinaryMethods.Select(match => match.Newer.)
            //});
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
