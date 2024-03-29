﻿using bekk.VyLogParser.Library;
using bekk.VyLogParser.Models;
using Microsoft.AspNetCore.Mvc;

namespace bekk.VyLogParser.WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadController : Controller
{
    private readonly ILogger<UploadController> _logger;

    public UploadController(ILogger<UploadController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "UploadFile")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            var workFolder = $"wwwroot\\Upload\\{DateTime.Now.Ticks}";
            var rootFolder = Path.Combine(Directory.GetCurrentDirectory(), workFolder);
            var fileNameOnly = await FileHelper.WriteFile(file, rootFolder);
            var fileName = new FileInfo(Path.Combine(rootFolder, fileNameOnly));
            var arguments = new Arguments
            {
                ClearOutputDirectory = false,
                OutputDirectory = fileName.Directory!,
                ZipFile = fileName
            };
            var logItems = LogReader.Execute(arguments);
            var result = LogHelper.Execute(arguments, logItems);

            result.ResultAsLogItemsFile = FileHelper.ConvertPhysicalFileToWebFile(result.ResultAsLogItemsFile, workFolder);
            result.ResultsAsJsonFile = FileHelper.ConvertPhysicalFileToWebFile(result.ResultsAsJsonFile, workFolder);
            result.ResultAsSummaryFile = FileHelper.ConvertPhysicalFileToWebFile(result.ResultAsSummaryFile, workFolder);

            Task.Run(() => { FileHelper.CleanupOldArchives(rootFolder); }).FireAndForget();

            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "En error has occurred when uploading file");
            return BadRequest(e.Message);
        }
    }

}