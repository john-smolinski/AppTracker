using CommandLine;
using ApplicationTracker.ImportCli.CommandLine;

namespace ApplicationTracker.ImportCli
{

    internal class Program
    {
        internal static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    // drop out if we're not reporting or executing migrations
                    if (!options.Report && !options.Execute)
                    {
                        Console.WriteLine("Error: Either --report (-r) or --execute (-x) must be provided.");
                        Environment.Exit(1);
                    }
                    if (options.Report && options.Execute) 
                    {
                        Console.WriteLine("Error: Only one of --report (-r) or --execute (-x) can be provided at same time");
                        Environment.Exit(1);
                    }

                    // drop out if execute was passed without identifying the entities
                    if (options.Execute)
                    {
                        if (!(options.JobTitles || options.Locations || options.Organizations ||
                              options.Sources || options.WorkEnvironments || options.All))
                        {
                            Console.WriteLine("Error: When using --execute (-x), at least one of --titles (-t), --locations (-l), --organizations (-o), --sources (-s), --environments (-e), or --all (-a) must be specified.");
                            Environment.Exit(1);
                        }
                    }
                    // report config
                    if (options.Report)
                    {
                        if (!(options.JobTitles || options.Locations || options.Organizations ||
                              options.Sources || options.WorkEnvironments || options.All))
                        {
                            // default to 'all' if no specific entity options are provided
                            options.All = true; 
                            Console.WriteLine("Info: Defaulting to --all (-a) for the report operation.");
                        }
                    }

                    ExecuteOptions(options);
                })
                .WithNotParsed(errors =>
                {
                    Console.WriteLine("Error parsing arguments. Use --help for usage information.");
                    Environment.Exit(1);
                });
        }

        internal static void ExecuteOptions(Options options)
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
