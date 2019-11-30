using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace BreakingChange.Core.Extensions
{
    internal static class SolutionExtensions
    {
        public static async Task<IEnumerable<Compilation>> GetCompilationsAsync(this Solution solution)
        {
            var compilationsTasks = solution.Projects.Select(project => project.GetCompilationAsync());
            var compilations = await Task.WhenAll(compilationsTasks).ConfigureAwait(false);
            var nonNullCompilations = compilations.Where(compilation => compilation != null);
            return nonNullCompilations;
        }
    }
}
