namespace bekk.VyLogParser.Models;

public class ResponseModel
{
    public ResponseModel()
    {
        Message = string.Empty;
    }

    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public bool IsResponse { get; set; }
}