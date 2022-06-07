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

        public static List<FileOnDiskModel> GetLogFilesOnDisk()
        {
            const string workFolder = "wwwroot\\Upload";
            var rootFolder = Path.Combine(Directory.GetCurrentDirectory(), workFolder);
            var rootDirectory = new DirectoryInfo(rootFolder);
            if (!rootDirectory.Exists) return new List<FileOnDiskModel>();

            var logFiles = rootDirectory.GetFiles("*.zip", SearchOption.AllDirectories);

            return logFiles.Select(x => new FileOnDiskModel
            {
                Name = x.Name,
                Url = ExtractPathAndFileName(x),
                LogStartDate = ParseDate(x)
            }).ToList();
        }

        private static DateTime ParseDate(FileSystemInfo fileInfo)
        {
            var temp = fileInfo.Name.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var dateTimePart = temp.Skip(1).Take(1).First();
            var datePart = dateTimePart.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return DateTime.Parse(datePart.First());
        }

        private static string ExtractPathAndFileName(FileInfo x)
        {
            var path = x.DirectoryName!.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return $"/Upload/{path.Last()}/{x.Name}";
        }
    }
}
