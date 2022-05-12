namespace bekk.VyLogParser.ConsoleApp;

internal static class ConsoleHelper
{
    internal static string? GetArgument(this IEnumerable<string> args, string name)
    {
        try
        {
            var item = args.FirstOrDefault(x => x.StartsWith($"-{name}="));
            var argumentValue = item?.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            return argumentValue;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}