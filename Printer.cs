namespace ConsoleSSG;

public class Printer
{
    public void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Something went wrong: {message}");
        Console.ResetColor();
    }

    public void WriteLineColorUnderline(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.WriteLine(new string('-', message.Length));
        Console.ResetColor();
    }

    public void WriteLineColor(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void WriteColor(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }

    public void PrintBox(string message, ConsoleColor color = ConsoleColor.Cyan, int padding = 2)
    {
        int width = message.Length + (padding * 2);
        
        // Top bar
        WriteLineColor("╔" + new string('═', width) + "╗", color);
        WriteLineColor("║" + new string(' ', padding) + message + new string(' ', padding) + "║", color);
        WriteLineColor("╚" + new string('═', width) + "╝", color);

        Console.WriteLine();
    }
}