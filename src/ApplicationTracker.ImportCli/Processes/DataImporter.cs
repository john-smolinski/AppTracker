using ClosedXML.Excel;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTracker.ImportCli.Processes
{
    [ExcludeFromCodeCoverage]
    public class DataImporter()
    {
        public string ImportApplication(IXLWorksheet workSheet)
        {
            ArgumentNullException.ThrowIfNull(workSheet);

            throw new NotImplementedException();
        }
    }
}
