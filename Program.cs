using System;
using System.IO;

namespace RIT.CSCI._251_Assignment1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2) {
                printHelp();
            } else if (args[0] == "-s") {
                runSequential(args[1]);
            } else if (args[0] == "-p") {
            
            } else if (args[0] == "-b") {
            
            } else {
                printHelp();
            }
        }

        static bool runSequential(String path) {
            String[] allfiles = DirectoryInfo.GetFiles(path, "*.*", SearchOption.AllDirectories);

            try {
                foreach (String f in allFiles) {
                    Console.WriteLine(f);
                }
            } catch (Exception e) {
                Console.WriteLine("Error in runSequential: {0}", e.ToString());
            } finally {}

            return true;
        }

        static void printHelp() {
            Console.WriteLine("Usage : du [−s ] [−p ] [−b ] <path>");
            Console.WriteLine("Summarize disk usage of the set of FILES, recursively for directories");
            Console.WriteLine();
            Console.WriteLine("You MUST specify one of the parameters, -s, -p, or -b");
            Console.WriteLine("−s\tRun in single threaded mode");
            Console.WriteLine("−p\tRun in parallel mode (uses all available processors");
            Console.WriteLine("−b\tRun in both parallel and single threaded mode.");
            Console.WriteLine("  \tRuns parallel followed by sequential mode");
        }
    }
}
