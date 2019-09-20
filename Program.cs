/*
 * Colin Reilly
 * Project 1
 * CSCI.251.03 - Conc of Par and Dist Systems
 * Rochester Institute of Technology
 * 9/20/19
*/

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RIT.CSCI._251_Assignment1 {
    class Program {
        private static object LockObject = new object();

        static void Main(string[] args) {
            Stopwatch sw = new Stopwatch();

            if (args.Length != 2) {
                printHelp();
            } else if (args[0] == "-s") {
                sw.Start();
                var data = runSequential(new DirectoryInfo(args[1]));
                sw.Stop();

                printData(data, sw, "Sequential");
            } else if (args[0] == "-p") {
                sw.Start();
                var data = runParallel(new DirectoryInfo(args[1]));
                sw.Stop();

                printData(data, sw, "Parallel");
            } else if (args[0] == "-b") {
                sw.Start();
                var data = runParallel(new DirectoryInfo(args[1]));
                sw.Stop();
                printData(data, sw, "Parallel");

                Console.WriteLine();

                sw.Start();
                data = runSequential(new DirectoryInfo(args[1]));
                sw.Stop();
                printData(data, sw, "Sequential");
            } else {
                printHelp();
            }
        }

        static void printData(Dictionary<string, long> data, Stopwatch sw, string type) {
            Console.WriteLine("{0} Calculated in: {1}.{2}s", type, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds);
            Console.WriteLine("{0:n0} folders, {1:n0} files, {2:n0} bytes",
                data["numFolders"],
                data["numFiles"],
                data["size"]);
        }

        static Dictionary<string, long> runParallel(DirectoryInfo dirInfo) {
            long size = 0;
            long numFolders = 0;
            long numFiles = 0;

            try {
                // Add size of each file in directory to size.
                Parallel.ForEach(dirInfo.GetFiles(), (file) => {
                    try {
                        lock (LockObject) {
                            numFiles++;
                        }

                        lock(LockObject) {
                            size += file.Length;
                        }
                    } catch (System.UnauthorizedAccessException) { }  // Can't get files
                });

                // Recursively call runSequential and add their returns to size.
                Parallel.ForEach(dirInfo.GetDirectories(), (dir) => {
                    lock (LockObject) {
                        numFolders++;                    
                    }

                    var data = runSequential(dir);

                    lock (LockObject) {
                        size += data["size"];
                        numFolders += data["numFolders"];
                        numFiles += data["numFiles"];
                    }
                });
            } catch (System.UnauthorizedAccessException) { }  // Can't get directories
            catch (Exception e) { Console.WriteLine("Error in runSequential: {0}", e.ToString()); }

            return new Dictionary<string, long> {
                {"size", size},
                {"numFolders", numFolders},
                {"numFiles", numFiles},
            };
        }

        static Dictionary<string, long> runSequential(DirectoryInfo dirInfo) {
            long size = 0;
            long numFolders = 0;
            long numFiles = 0;

            try {
                // Add size of each file in directory to size.
                foreach (FileInfo file in dirInfo.GetFiles()) {
                    try {
                        numFiles++;

                        size += file.Length;
                    } catch (System.UnauthorizedAccessException) { }  // Can't get files
                }

                // Recursively call runSequential and add their returns to size.
                foreach (var dir in dirInfo.GetDirectories()) {
                    numFolders++;

                    var data = runSequential(dir);

                    size += data["size"];
                    numFolders += data["numFolders"];
                    numFiles += data["numFiles"];
                }
            } catch (System.UnauthorizedAccessException) { }  // Can't get directories
            catch (Exception e) { Console.WriteLine("Error in runSequential: {0}", e.ToString()); }

            return new Dictionary<string, long> {
                {"size", size},
                {"numFolders", numFolders},
                {"numFiles", numFiles},
            };
        }

        static void printHelp() {
            Console.WriteLine("Usage : du [−s ] [−p ] [−b ] <path>");
            Console.WriteLine("Summarize disk usage of the set of FILES, recursively for directories.");
            Console.WriteLine();
            Console.WriteLine("You MUST specify one of the parameters, -s, -p, or -b");
            Console.WriteLine("−s\tRun in single threaded mode");
            Console.WriteLine("−p\tRun in parallel mode (uses all available processors)");
            Console.WriteLine("−b\tRun in both parallel and single threaded mode.");
            Console.WriteLine("  \tRuns parallel followed by sequential mode");
        }
    }
}
