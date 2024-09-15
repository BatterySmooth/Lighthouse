using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Lighthouse.Tower.Logging
{
  public static class Logger
  {
    private static BlockingCollection<string> m_Queue = new();

    static Logger()
    {
      var thread = new Thread(
        () =>
        {
          while (true) Console.WriteLine($"[{DateTime.Now:s}] {m_Queue.Take()}");
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
      Console.WriteLine($"[{DateTime.Now:s}] {value}");
    }
  }
}