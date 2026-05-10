namespace TechMove.Services
{
    public class FileValidationService
    {
        public bool IsPdfFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            var extension = Path.GetExtension(fileName).ToLower();

            return extension == ".pdf";
        }
    }
}