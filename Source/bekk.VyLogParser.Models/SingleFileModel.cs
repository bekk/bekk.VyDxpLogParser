using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace bekk.VyLogParser.Models;

public class SingleFileModel : ResponseModel
{
    [Required(ErrorMessage = "Please enter file name")]
    public string FileName { get; set; } = null!;

    [Required(ErrorMessage = "Please select file")]
    public IFormFile File { get; set; } = null!;
}