using Microsoft.AspNetCore.Http;

namespace bekk.VyLogParser.Library
{
    public static class FileHelper
    {
        public static async Task<string> WriteFile(IFormFile file, string rootFolder)
        {
            try
            {
                //var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                //var fileName = DateTime.Now.Ticks + extension;

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
    }
}
