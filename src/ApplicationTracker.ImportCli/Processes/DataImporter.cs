using ApplicationTracker.ImportCli.Helpers;
using ClosedXML.Excel;
using System.Text;

namespace ApplicationTracker.ImportCli.Processes
{
    public class DataImporter()
    {
        public string ImportApplication(IXLWorksheet workSheet)
        {
            ArgumentNullException.ThrowIfNull(workSheet);

            throw new NotImplementedException();
        }
    }
}
