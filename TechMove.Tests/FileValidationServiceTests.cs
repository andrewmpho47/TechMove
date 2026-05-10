using TechMove.Services;

namespace TechMove.Tests
{
    public class FileValidationServiceTests
    {
        [Fact]
        public void IsPdfFile_ShouldReturnTrue_WhenFileIsPdf()
        {
            var service = new FileValidationService();

            var result = service.IsPdfFile("signed-agreement.pdf");

            Assert.True(result);
        }

        [Fact]
        public void IsPdfFile_ShouldReturnFalse_WhenFileIsExe()
        {
            var service = new FileValidationService();

            var result = service.IsPdfFile("virus.exe");

            Assert.False(result);
        }

        [Fact]
        public void IsPdfFile_ShouldReturnFalse_WhenFileNameIsEmpty()
        {
            var service = new FileValidationService();

            var result = service.IsPdfFile("");

            Assert.False(result);
        }
    }
}