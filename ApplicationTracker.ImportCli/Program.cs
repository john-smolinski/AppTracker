using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.ImportCli.CommandLine;
using ClosedXML.Excel;
using CommandLine;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

            if (!options.Report && !options.Execute)
            {
                Console.WriteLine("Error: Either --report (-r) or --execute (-x) must be provided.");
                return false;
            }

            if (options.Report && options.Execute)
            {
                Console.WriteLine("Error: Only one of --report (-r) or --execute (-x) can be provided at the same time.");
                return false;
            }

            if (options.Execute && !(options.JobTitles || options.Locations || options.Organizations || options.Sources || options.WorkEnvironments || options.All))
            {
                Console.WriteLine("Error: When using --execute (-x), at least one of --titles (-t), --locations (-l), --organizations (-o), --sources (-s), --environments (-e), or --all (-a) must be specified.");
                return false;
            }

            if (options.Report && !(options.JobTitles || options.Locations || options.Organizations || options.Sources || options.WorkEnvironments || options.All))
            {
                options.All = true;
                Console.WriteLine("Info: Defaulting to --all (-a) for the report operation.");
            }

            return true;
        }

        public static void ExecuteOptions(Options options)
        {
            // intentional hard coding 
            var worksheet = new XLWorkbook(options.FilePath).Worksheet(1);

            // TODO hard coding connection string temporarily 
            var context = new TrackerDbContext(new DbContextOptionsBuilder<TrackerDbContext>()
                .UseSqlServer("Server=localhost; Database=Tracker; Integrated Security=True; TrustServerCertificate=True;").Options);

            if (options.Report)
            {
                ExecuteReport(options, context, worksheet);
            }

            if (options.Execute)
            {
                ExecuteMigrations(options, context, worksheet);
            }
        }
        private static void ExecuteReport(Options options, TrackerDbContext context, IXLWorksheet worksheet)
        {
            var reporter = new ReportGenerator(context);
            Console.WriteLine("Generating report...");

            if (options.All)
            {
                Console.WriteLine("Reporting on all entities.");
                // TODO: Implement report generation for all entities
            }
            else
            {
                ProcessEntityReports(options, reporter, worksheet);
            }
        }
        private static void ExecuteMigrations(Options options, TrackerDbContext context, IXLWorksheet worksheet)
        {
            var importer = new ExcelDataImporter(context);
            Console.WriteLine("Executing migrations...");

            if (options.All)
            {
                Console.WriteLine("Executing on all entities.");
                // TODO: Implement execution for all entities
            }
            else
            {
                ProcessEntityMigrations(options, importer, worksheet);
            }
        }

        private static void ProcessEntityReports(Options options, ReportGenerator reporter, IXLWorksheet worksheet)
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

            // TODO: Handle edge cases, such as reporting on Locations
        }

        private static void ProcessEntityMigrations(Options options, ExcelDataImporter importer, IXLWorksheet worksheet)
        {
            if (options.JobTitles)
            {
                Console.WriteLine("Executing on Job Titles.");
                var report = importer.ImportEntities<JobTitle>(worksheet);
                Console.WriteLine(report);
            }

            if (options.Organizations)
            {
                Console.WriteLine("Executing on Organizations.");
                var report = importer.ImportEntities<Organization>(worksheet);
                Console.WriteLine(report);
            }

            if (options.Sources)
            {
                Console.WriteLine("Executing on Sources.");
                var report = importer.ImportEntities<Source>(worksheet);
                Console.WriteLine(report);
            }

            if (options.WorkEnvironments)
            {
                Console.WriteLine("Executing on Work Environments.");
                var report = importer.ImportEntities<WorkEnvironment>(worksheet);
                Console.WriteLine(report);
            }

            // TODO: Handle edge cases, such as executing on Locations
        }

    }
}
