using ApplicationTracker.Data.Dtos;
using ApplicationTracker.ImportCli.CommandLine;
using ApplicationTracker.ImportCli.Processes;
using ClosedXML.Excel;
using CommandLine;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace ApplicationTracker.ImportCli
{
    public class Program
    {
        private static string apiUrl = "http://localhost:5000/api/applications";
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

            // get the data from the worksheet
            var applicationDtos = GetApplicationDtos(worksheet);

            foreach (var dto in applicationDtos)
            {
                PostData(dto);
            }

        }

        public static void PostData(ApplicationDto dto)
        {
            // post the data to the API
            var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            Console.WriteLine($"Posting data: {dto.ApplicaitionDate} - {dto.Organization.Name} - {dto.JobTitle.Name}");
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


        /// <summary>
        /// Extract the application data from the worksheet
        /// </summary>
        /// <param name="worksheet">worksheet containing application log</param>
        /// <returns>list of applications</returns>
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

                dto.ApplicaitionDate = DateOnly.FromDateTime(appDate.GetDateTime());
                dto.Source = new SourceDto { Name = worksheet.Cell(i, 2).GetString() };
                dto.Organization = new OrganizationDto { Name = worksheet.Cell(i, 3).GetString() };
                dto.JobTitle = new JobTitleDto { Name = worksheet.Cell(i, 4).GetString() };
                dto.WorkEnvironment = new WorkEnvironmentDto { Name = worksheet.Cell(i, 5).GetString() };

                applicationDtos.Add(dto);
            }
            return applicationDtos;
        }
    }
}
