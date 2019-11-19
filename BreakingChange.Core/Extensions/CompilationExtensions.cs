using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BreakingChange.Core.Extensions
{
    internal static class CompilationExtensions
    {
        public static async Task<IEnumerable<INamedTypeSymbol>> GetPublicTypeSymbolsAsync(this Compilation compilation)
        {
            var syntaxTrees = compilation.SyntaxTrees;
            var syntaxTreesRootsTasks = syntaxTrees.Select(syntaxTree => syntaxTree.GetRootAsync());
            var syntaxTreesRoots = await Task.WhenAll(syntaxTreesRootsTasks).ConfigureAwait(false);

            var publicTypeSymbols = syntaxTreesRoots
                .SelectMany(syntaxTreeRoot => syntaxTreeRoot.DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>())
                .Where(typeDeclaration => typeDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
                .Select(publicTypeDeclaration => compilation.GetSemanticModel(publicTypeDeclaration.SyntaxTree).GetDeclaredSymbol(publicTypeDeclaration))
                .ToList();

            return publicTypeSymbols;
        }
    }
}
