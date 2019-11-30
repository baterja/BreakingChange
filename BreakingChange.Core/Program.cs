using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BreakingChange.Core.Analyzers;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace BreakingChange.Core
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            }

            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            var instance = visualStudioInstances.Length == 1
                ? visualStudioInstances[0]
                : SelectVisualStudioInstance(visualStudioInstances);

            Console.WriteLine($"Using MSBuild at '{instance.MSBuildPath}' to load projects.");

            MSBuildLocator.RegisterInstance(instance);

            using var workspace = MSBuildWorkspace.Create();
            workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);

            //var olderSolutionPath = args[0];
            //Console.WriteLine($"Loading older solution '{olderSolutionPath}'");
            //var olderSolution = await workspace.OpenSolutionAsync(olderSolutionPath, new ConsoleProgressReporter()).ConfigureAwait(false);
            //Console.WriteLine($"Finished loading solution '{olderSolutionPath}'");

            //var newerSolutionPath = args[1];
            //Console.WriteLine($"Loading newer solution '{newerSolutionPath}'");
            //var newerSolution = await workspace.OpenSolutionAsync(newerSolutionPath, new ConsoleProgressReporter()).ConfigureAwait(false);
            //Console.WriteLine($"Finished loading solution '{newerSolutionPath}'");

            var olderProjectPath = args[0];
            Console.WriteLine($"Loading older project '{olderProjectPath}'");
            var olderProject = await workspace.OpenProjectAsync(olderProjectPath, new ConsoleProgressReporter()).ConfigureAwait(false);
            Console.WriteLine($"Finished loading project '{olderProjectPath}'");

            var newerProjectPath = args[1];
            Console.WriteLine($"Loading newer project '{newerProjectPath}'");
            var newerProject = await workspace.OpenProjectAsync(newerProjectPath, new ConsoleProgressReporter()).ConfigureAwait(false);
            Console.WriteLine($"Finished loading project '{newerProjectPath}'");

            try
            {
                var analyzer = new Analyzer();
                await analyzer.Analyze(newerProject, olderProject).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }

        private static VisualStudioInstance SelectVisualStudioInstance(VisualStudioInstance[] visualStudioInstances)
        {
            Console.WriteLine("Multiple installs of MSBuild detected please select one:");
            for (int i = 0; i < visualStudioInstances.Length; i++)
            {
                Console.WriteLine($"Instance {i + 1}");
                Console.WriteLine($"    Name: {visualStudioInstances[i].Name}");
                Console.WriteLine($"    Version: {visualStudioInstances[i].Version}");
                Console.WriteLine($"    MSBuild Path: {visualStudioInstances[i].MSBuildPath}");
            }

            while (true)
            {
                var userResponse = Console.ReadLine();
                if (int.TryParse(userResponse, out int instanceNumber) &&
                    instanceNumber > 0 &&
                    instanceNumber <= visualStudioInstances.Length)
                {
                    return visualStudioInstances[instanceNumber - 1];
                }

                Console.WriteLine("Input not accepted, try again.");
            }
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress loadProgress)
            {
                var projectDisplay = Path.GetFileName(loadProgress.FilePath);
                if (loadProgress.TargetFramework != null)
                {
                    projectDisplay += $" ({loadProgress.TargetFramework})";
                }

                Console.WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
            }
        }
    }
}
