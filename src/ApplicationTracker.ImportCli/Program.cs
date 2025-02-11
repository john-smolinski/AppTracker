using ApplicationTracker.Data.Dtos;
using ApplicationTracker.ImportCli.CommandLine;
using ClosedXML.Excel;
using CommandLine;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace ApplicationTracker.ImportCli
{
    public class Program
    {
        private static string? apiUrl;
        public static void Main(string[] args)
        {
            LoadConfiguration();
            var exitCode =  ProcessArguments(args);
            Environment.Exit(exitCode);
        }

        private static void LoadConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            apiUrl = configuration["ApiUrl"] ?? throw new InvalidOperationException("ApiUrl is missing in appsettings.json");
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
            var worksheet = new XLWorkbook(options.FilePath).Worksheet(1);
            var applicationDtos = GetApplicationDtos(worksheet);

            foreach (var dto in applicationDtos)
            {
                PostData(dto);
            }

        }

        public static void PostData(ApplicationDto dto)
        {
            var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            
            Console.WriteLine($"Posting data: {dto.ApplicationDate} - {dto.Organization.Name} - {dto.JobTitle.Name}");
            var response = client.PostAsync(apiUrl, content).Result;
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error posting data: {response.StatusCode}");
            }
            else
            {
                Console.WriteLine("Data posted successfully");
            }
        }

        private static List<ApplicationDto> GetApplicationDtos(IXLWorksheet worksheet)
        {
            var lastRowUsed = worksheet.LastRowUsed();
            if (lastRowUsed == null)
            {
                return new List<ApplicationDto>();
            }
            var rowCount = lastRowUsed.RowNumber();
            List<ApplicationDto> applicationDtos = new List<ApplicationDto>();

            for (int i = 2; i <= rowCount; i++)
            {
                var dto = new ApplicationDto();
                var appDate = worksheet.Cell(i, 1).Value;

                dto.ApplicationDate = DateOnly.FromDateTime(appDate.GetDateTime());
                dto.Source = new SourceDto { Name = worksheet.Cell(i, 2).GetString() };
                dto.Organization = new OrganizationDto { Name = worksheet.Cell(i, 3).GetString() };
                dto.JobTitle = new JobTitleDto { Name = worksheet.Cell(i, 4).GetString() };
                dto.WorkEnvironment = new WorkEnvironmentDto { Name = worksheet.Cell(i, 5).GetString() };
                if (!string.IsNullOrWhiteSpace(worksheet.Cell(i, 6).GetString()))
                {
                    dto.City = worksheet.Cell(i, 6).GetString();
                }
                if (!string.IsNullOrWhiteSpace(worksheet.Cell(i, 7).GetString()))
                {
                    dto.State = worksheet.Cell(i, 7).GetString();
                }
                applicationDtos.Add(dto);
            }
            return applicationDtos;
        }
    }
}
