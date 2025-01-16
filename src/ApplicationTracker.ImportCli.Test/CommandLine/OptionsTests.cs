using ApplicationTracker.ImportCli.CommandLine;

namespace ApplicationTracker.ImportCli.Test.CommandLine
{
    [TestFixture]
    public class OptionsTests
    {
        private StringWriter _consoleOutput;
        private const string ValidFilePath = "validfile.txt";

        [SetUp]
        public void Setup()
        {
            // caputre console output
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);

            // valid file for testing
            File.WriteAllText(ValidFilePath, "Sample content");
        }

        [TearDown]
        public void TearDown()
        {
            _consoleOutput.Dispose();   
            // clean up after tests
            if (File.Exists(ValidFilePath))
            {
                File.Delete(ValidFilePath);
            }
        }

        [Test]
        public void ValidateRequired_ShouldReturnFalse_WhenFilePathIsMissing()
        {
            // Setup
            var options = new Options { FilePath = string.Empty };

            // Act
            var result = options.ValidateRequired();

            // Assert  Error: The required option '--file (-f)' is missing.
            Assert.Multiple(() =>
            { 
                Assert.That(result, Is.False);
                Assert.That(_consoleOutput.ToString().TrimEnd(), Is.EqualTo("Error: The required option '--file (-f)' is missing."));
            });
        }

        [Test]
        public void ValidateRequired_ShouldReturnTrue_WhenFilePathIsProvided()
        {
            // Setup
            var options = new Options { FilePath = "somefile.txt" };

            // Act 
            var result = options.ValidateRequired();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateFilePath_ShouldReturnFalse_WhenFilePathIsEmpty()
        {
            // Setup
            var options = new Options { FilePath = string.Empty };

            // Act
            var result = options.ValidateFilePath();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(_consoleOutput.ToString().TrimEnd(), Is.EqualTo("Error: File path is empty."));
            });
        }

        [Test]
        public void ValidateFilePath_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            // Setup
            var options = new Options { FilePath = "nonexistentfile.txt" };

            // Act
            var result = options.ValidateFilePath();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(_consoleOutput.ToString(), Does.StartWith("Error: File not found"));
            });
        }

        [Test]
        public void ValidateFilePath_ShouldReturnTrue_WhenFileExists()
        {
            // Setup 
            var options = new Options { FilePath = ValidFilePath };

            // Act 
            var result = options.ValidateFilePath();

            // Assert
            Assert.That(result, Is.True);
        }
    }
}
