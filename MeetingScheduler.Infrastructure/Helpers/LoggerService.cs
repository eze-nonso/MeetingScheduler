using MeetingScheduler.Core.Interfaces.Helpers;
using MeetingScheduler.Core.Models.Constants;

namespace MeetingScheduler.Infrastructure.Helpers;

public class LoggerService : ILoggerService
{
    private static object mutex = default!;

    public LoggerService()
    {
        mutex = new object();
    }
    public void LogError(Exception exception, string message)
    {
        try
        {
            string directory = AppSettings.Logpath;
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string filepath = directory + @"\" + DateTime.Now.Date.ToString("dd-MMM-yyyy") + ".txt";
            lock (mutex)
            {
                File.AppendAllText(filepath, "Event Time: " + DateTime.Now.ToString() + " | Message: " + message + " | Exception: " + exception + Environment.NewLine);

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not log info to file: {ex.Message}");
        }
    }

    public void LogInfo(string message)
    {
        try
        {
            string directory = AppSettings.Logpath;
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string filepath = directory + @"\" + DateTime.Now.Date.ToString("dd-MMM-yyyy") + ".txt";
            lock (mutex)
            {
                File.AppendAllText(filepath, "Event Time: " + DateTime.Now.ToString() + " | Message: " + message + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not log info to file: {ex.Message}");
        }
    }
}
