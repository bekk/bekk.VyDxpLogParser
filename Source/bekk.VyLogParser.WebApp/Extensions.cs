namespace bekk.VyLogParser.WebApp;

public static class Extensions
{
    public static async void FireAndForget(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception)
        {
            // log errors
        }
    }
}