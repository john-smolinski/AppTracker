using ApplicationTracker.ImportCli.CommandLine;

namespace ApplicationTracker.ImportCli.Test
{
    [TestFixture]
    public class ProgramTests
    {
        private StringWriter _consoleOutput;

        [SetUp]
        public void SetUp()
        {
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
        }

        [TearDown]
        public void TearDown()
        {
            _consoleOutput.Dispose();
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
                Assert.That(_consoleOutput.ToString(), Contains.Substring("Error: Either --report (-r) or --execute (-x) must be provided."));
            });
        }

        [Test]
        public void ProcessArguments_ReturnsError_WhenBothReportAndExecuteProvided()
        {
            // Setup
            var args = new[] { "--report", "--execute" };

            // Act
            var exitCode = Program.ProcessArguments(args);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exitCode, Is.EqualTo(1));
                Assert.That(_consoleOutput.ToString(), Contains.Substring("Error: Only one of --report (-r) or --execute (-x) can be provided at the same time"));
            });
        }

        [Test]
        public void ProcessArguments_DefaultsToAll_WhenReportAndNoSpecificEntityOptionsProvided()
        {
            // Setup
            var args = new[] { "--report" };

            // Act
            var exitCode = Program.ProcessArguments(args);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exitCode, Is.EqualTo(0));
                Assert.That(_consoleOutput.ToString(), Contains.Substring("Info: Defaulting to --all (-a) for the report operation."));
            });
        }

        [Test]
        public void ProcessArguments_CallsExecuteOptions_ForValidArguments()
        {
            // Setup
            var args = new[] { "--execute", "--titles" };

            // Act
            var exitCode = Program.ProcessArguments(args);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exitCode, Is.EqualTo(0));
                Assert.That(_consoleOutput.ToString(), Contains.Substring("Executing migrations..."));
                Assert.That(_consoleOutput.ToString(), Contains.Substring("Executing on Job Titles."));
            });
        }

        [Test]
        public void ExecuteOptions_PrintsReportingMessages()
        {
            // Setup
            var options = new Options
            {
                Report = true,
                JobTitles = true
            };

            // Act
            Program.ExecuteOptions(options);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_consoleOutput.ToString(), Contains.Substring("Generating report..."));
                Assert.That(_consoleOutput.ToString(), Contains.Substring("Reporting on Job Titles."));
            });
        }
    }
}
