using CommandLine;
using ApplicationTracker.ImportCli.CommandLine;

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
            if (options.Report)
            {
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
                    }
                    if (options.Locations)
                    {
                        Console.WriteLine("Reporting on Locations.");
                    }
                    if (options.Organizations)
                    {
                        Console.WriteLine("Reporting on Organizations.");
                    }
                    if (options.Sources)
                    {
                        Console.WriteLine("Reporting on Sources.");
                    }
                    if (options.WorkEnvironments)
                    {
                        Console.WriteLine("Reporting on Work Environments.");
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
