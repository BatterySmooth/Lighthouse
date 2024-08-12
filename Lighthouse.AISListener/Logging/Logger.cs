using System.Collections.Concurrent;

namespace Lighthouse.AISListener.Logging;

public static class Logger
{
  private static BlockingCollection<string> m_Queue = new();

  static Logger()
  {
    var thread = new Thread(
      () =>
      {
        while (true) Console.WriteLine(m_Queue.Take());
      });
    thread.IsBackground = true;
    thread.Start();
  }

  public static void LogAsync(string value)
  {
    m_Queue.Add(value);
  }
  public static void LogSync(string value)
  {
    Console.WriteLine(value);
  }
}