using bekk.VyLogParser.Models;
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

        public static void CleanupOldArchives(string rootFolder)
        {
            var rootDirectory = new DirectoryInfo(rootFolder);
            if (!rootDirectory.Exists) return;

            foreach (var directoryInfo in rootDirectory.GetDirectories())
            {
                if (directoryInfo.CreationTime > DateTime.Now.AddMonths(-6)) continue;
                directoryInfo.Delete(true);
            }
        }

        public static IEnumerable<FileOnDiskModel> GetLogFilesOnDisk()
        {
            const string workFolder = "wwwroot\\Upload";
            var rootFolder = Path.Combine(Directory.GetCurrentDirectory(), workFolder);
            var rootDirectory = new DirectoryInfo(rootFolder);

            if (!rootDirectory.Exists) yield return new FileOnDiskModel();

            var logFiles = rootDirectory.GetFiles("*.zip", SearchOption.AllDirectories);

            foreach (var logFile in logFiles)
            {
                var parsedFileInfo = ParseFileInfo(logFile);

                yield return new FileOnDiskModel
                {
                    Name = parsedFileInfo.Name,
                    Url = ExtractPathAndFileName(logFile),
                    LogStartDate = parsedFileInfo.MinDate
                };
            }
        }
        
        private static LogFileProperties ParseFileInfo(FileSystemInfo fileInfo)
        {
            var temp = fileInfo.Name.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            var minDateString = temp[1];
            var maxDateString = temp[2];
            var minDate = DateTime.Parse(minDateString.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries).First());
            var maxDate = DateTime.Parse(maxDateString.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries).First());
            
            return new LogFileProperties
            {
                Name = $"From: {minDate:dd.MM.yyyy} To: {maxDate:dd.MM.yyyy}",
                MinDate = minDate,
                MaxDate = maxDate
            };
        }

        private static string ExtractPathAndFileName(FileInfo x)
        {
            var path = x.DirectoryName!.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return $"/Upload/{path.Last()}/{x.Name}";
        }
    }

    public class LogFileProperties
    {
        public string Name { get; set; } = null!;
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
    }
}
