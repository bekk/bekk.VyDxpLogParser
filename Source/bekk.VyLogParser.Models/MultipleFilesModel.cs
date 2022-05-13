using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace bekk.VyLogParser.Models;

public class MultipleFilesModel : ResponseModel
{
    public MultipleFilesModel()
    {
        Files = new List<IFormFile>();
    }

    [Required(ErrorMessage = "Please select files")]
    public List<IFormFile> Files { get; set; }
}