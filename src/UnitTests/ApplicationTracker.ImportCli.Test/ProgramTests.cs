namespace ApplicationTracker.ImportCli.Test
{
    [TestFixture]
    public class ProgramTests
    {
        private StringWriter _consoleOutput;
        private const string _validFilePath = "testfile.xls";

        [SetUp]
        public void SetUp()
        {
            // capture console output
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
            
            // valid file for testing
            File.WriteAllText(_validFilePath, "Sample content");
        }

        [TearDown]
        public void TearDown()
        {
            _consoleOutput.Dispose();
            if (File.Exists(_validFilePath))
            {
                File.Delete(_validFilePath);
            }
        }

        [Test]
        public void ProcessArguments_ShouldReturnError_WhenFileDoesNotExist()
        {
            // Arrange
            var args = new string[] { "-f", "nonexistent.txt" };
            
            // Act 
            var exitCode = Program.ProcessArguments(args);

            Assert.Multiple(() =>
            {
                Assert.That(exitCode, Is.EqualTo(1));
                Assert.That(_consoleOutput.ToString(), Does.StartWith("Error: File not found"));
            });
        }
    }
}
