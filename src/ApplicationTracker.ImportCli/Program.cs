using ApplicationTracker.ImportCli.CommandLine;
using ApplicationTracker.ImportCli.Processes;
using ClosedXML.Excel;
using CommandLine;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTracker.ImportCli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var exitCode = ProcessArguments(args);
            Environment.Exit(exitCode);
        }

        public static int ProcessArguments(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    options =>
                    {
                        if(!ValidateOptions(options))
                        {
                            Console.WriteLine("Error: File option missing");
                            return 1;
                        }

                        ExecuteOptions(options);
                        return 0;
                    },
                    errors =>
                    {
                        Console.WriteLine("Error parsing arguments. Use --help for usage information.");
                        return 1;
                    });
        }

        private static bool ValidateOptions(Options options)
        {
            if (!options.ValidateRequired())
            {
                Console.WriteLine("Error: File option missing.");
                return false;
            }

            if (!options.ValidateFilePath())
            {
                Console.WriteLine("Error: File is missing or invalid.");
                return false;
            }
            return true;
        }

        public static void ExecuteOptions(Options options)
        {
            // intentional hard coding 
            var worksheet = new XLWorkbook(options.FilePath).Worksheet(1);

        }
    }
}
