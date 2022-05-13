using Microsoft.AspNetCore.Http;

namespace bekk.VyLogParser.Library
{
    public static class FileHelper
    {
        public static async Task<string> WriteFile(IFormFile file, string rootFolder)
        {
            try
            {
                if (!Directory.Exists(rootFolder)) Directory.CreateDirectory(rootFolder);

                var path = Path.Combine(rootFolder, file.FileName);

                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return file.FileName;
            }
            catch (Exception)
            {
                //log error
            }

            return string.Empty;
        }
        public static string ConvertPhysicalFileToWebFile(string physicalFile, string workFolder)
        {
            var fileInfo = new FileInfo(physicalFile);
            var downloadFolder = workFolder.Replace("\\", "/").Replace("wwwroot/", string.Empty);
            var webFile = $"/{downloadFolder}/{fileInfo.Name}";

            return webFile;
        }
    }
}
