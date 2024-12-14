using ApplicationTracker.ImportCli.Helpers;
using ClosedXML.Excel;
using System.Text;

namespace ApplicationTracker.ImportCli.Processes
{
    public class DataImporter
    {

        public string ImportEntities(IXLWorksheet workSheet)
        {
            ArgumentNullException.ThrowIfNull(workSheet);

            return string.Empty;
        }
    }
}
