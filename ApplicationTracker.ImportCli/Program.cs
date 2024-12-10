using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.ImportCli.CommandLine;
using ClosedXML.Excel;
using CommandLine;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTracker.ImportCli
{
    public class Program
    {
        public static int ProcessArguments(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    options =>
                    {
                        if(!options.ValidateRequired())
                        {
                            Console.WriteLine("Error: File option missing");
                            return 1;
                        }

                        if(!options.ValidateFilePath())
                        {
                            Console.WriteLine("Error: File is missing or invalid");
                            return 1;
                        }
                        if (!options.Report && !options.Execute)
                        {
                            Console.WriteLine("Error: Either --report (-r) or --execute (-x) must be provided.");
                            return 1;
                        }
                        if (options.Report && options.Execute)
                        {
                            Console.WriteLine("Error: Only one of --report (-r) or --execute (-x) can be provided at the same time");
                            return 1;
                        }
                        if (options.Execute && !(options.JobTitles || options.Locations || options.Organizations || options.Sources || options.WorkEnvironments || options.All))
                        {
                            Console.WriteLine("Error: When using --execute (-x), at least one of --titles (-t), --locations (-l), --organizations (-o), --sources (-s), --environments (-e), or --all (-a) must be specified.");
                            return 1;
                        }

                        if (options.Report && !(options.JobTitles || options.Locations || options.Organizations || options.Sources || options.WorkEnvironments || options.All))
                        {
                            options.All = true;
                            Console.WriteLine("Info: Defaulting to --all (-a) for the report operation.");
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

        public static void Main(string[] args)
        {
            var exitCode = ProcessArguments(args);
            Environment.Exit(exitCode);
        }
    
        public static void ExecuteOptions(Options options)
        {
            // validation passed need to load first sheet of workbook (hard coded)
            var worksheet = new XLWorkbook(options.FilePath).Worksheet(1);

            // TODO hard coding connection string temporarily 
            var context = new TrackerDbContext(new DbContextOptionsBuilder<TrackerDbContext>()
                .UseSqlServer("Server=localhost; Database=Tracker; Integrated Security=True; TrustServerCertificate=True;").Options);

            if (options.Report)
            {
                var reporter = new ReportGenerator(context);

                Console.WriteLine("Generating report...");
                if (options.All)
                {
                    Console.WriteLine("Reporting on all entities.");
                }
                else
                {
                    if (options.JobTitles)
                    {
                        Console.WriteLine("Reporting on Job Titles.");
                        var report = reporter.GenerateReport<JobTitle>(worksheet);
                        Console.WriteLine(report);
                    }
                    if (options.Organizations)
                    {
                        Console.WriteLine("Reporting on Organizations.");
                        var report = reporter.GenerateReport<Organization>(worksheet);
                        Console.WriteLine(report);
                    }
                    if (options.Sources)
                    {
                        Console.WriteLine("Reporting on Sources.");
                        var report = reporter.GenerateReport<Source>(worksheet);
                        Console.WriteLine(report);
                    }
                    if (options.WorkEnvironments)
                    {
                        Console.WriteLine("Reporting on Work Environments.");
                        var report = reporter.GenerateReport<WorkEnvironment>(worksheet);
                        Console.WriteLine(report);
                    }
                    // edge cases
                    if (options.Locations)
                    {
                        Console.WriteLine("Reporting on Locations.");
                    }
                }
            }

            if (options.Execute)
            {
                Console.WriteLine("Executing migrations...");
                if (options.All)
                {
                    Console.WriteLine("Executing on all entities.");
                    // add exectuion 
                }
                if (options.JobTitles)
                {
                    Console.WriteLine("Executing on Job Titles.");
                    // add exectuion 
                }
                if (options.Locations)
                {
                    Console.WriteLine("Executing on Locations.");
                    // add exectuion 
                }
                if (options.Organizations)
                {
                    Console.WriteLine("Executing on Organizations.");
                    // add exectuion 
                }
                if (options.Sources)
                {
                    Console.WriteLine("Executing on Sources.");
                    // add exectuion 
                }
                if (options.WorkEnvironments)
                {
                    Console.WriteLine("Executing on Work Environments.");
                    // add exectuion 
                }
            }
        }
    }
}
