using ApplicationTracker.ImportCli.CommandLine;
using NUnit.Framework.Constraints;

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
        public void ProcessArguments_ShouldReturnError_WhenFilePathIsMissing()
        {
            // Setup 
            var args = new string[] { "--report" };

            // Act
            var exitCode = Program.ProcessArguments(args);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exitCode, Is.EqualTo(1));
                Assert.That(_consoleOutput.ToString().TrimEnd(), Is.EqualTo("Error parsing arguments. Use --help for usage information."));
            });
        }

        [Test]
        public void ProcessArguments_ShouldReturnError_WhenFileDoesNotExist()
        {
            // Setup
            var args = new string[] { "-f", "nonexistent.txt", "--report" };
            
            // Act 
            var exitCode = Program.ProcessArguments(args);

            Assert.Multiple(() =>
            {
                Assert.That(exitCode, Is.EqualTo(1));
                Assert.That(_consoleOutput.ToString(), Does.StartWith("Error: File not found"));
            });
        }

        [Test]
        public void ProcessArguments_ReturnsError_WhenNoArgumentsProvided()
        {
            // Setup
            var args = new string[0];

            // Act
            var exitCode = Program.ProcessArguments(args);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exitCode, Is.EqualTo(1));
                Assert.That(_consoleOutput.ToString(), Contains.Substring("Error parsing arguments. Use --help for usage information"));
            });
        }

        [Test]
        public void ProcessArguments_ReturnsError_WhenBothReportAndExecuteProvided()
        {
            // Setup
            var args = new[] { "-f", _validFilePath,   "--report", "--execute" };

            // Act
            var exitCode = Program.ProcessArguments(args);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exitCode, Is.EqualTo(1));
                Assert.That(_consoleOutput.ToString(), Contains.Substring("Error: Only one of --report (-r) or --execute (-x) can be provided at the same time"));
            });
        }
    }
}
