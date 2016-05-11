using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRSC
{
  public static class Log
  {

    public static void WriteLine(string format)
    {
      Console.WriteLine("{0}: {1}", DateTime.Now.ToShortDateString(), format);
      using(System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine("{0}: {1}", DateTime.Now.ToShortDateString(), format);
      }
    }

    public static void WriteLine(bool value)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      }
    }

    public static void WriteLine(char value)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      }
    }

    public static void WriteLine(decimal value)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      }
    }

    public static void WriteLine(double value)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      }
    }

    public static void WriteLine(float value)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      }
    }

    public static void WriteLine(int value)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      }
    }

    public static void WriteLine(uint value)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      }
    }


    public static void WriteLine(long value)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      }
    }

    public static void WriteLine(ulong value)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      }
    }

    public static void WriteLine(object value)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + value);
      }
    }

    public static void WriteLine(string format, object arg0)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + format, arg0);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + format, arg0);
      }
    }

    public static void WriteLine(string format, object arg0, object arg1)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + format, arg0, arg1);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + format, arg0, arg1);
      }
    }

    public static void WriteLine(string format, object arg0, object arg1, object arg2)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + format, arg0, arg1, arg2);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + format, arg0, arg1, arg2);
      }
    }

    public static void WriteLine(string format, object arg0, object arg1, object arg2, object args3)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + format, arg0, arg1, arg2, args3);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
      {
        sw.WriteLine(DateTime.Now.ToShortDateString() + ": " + format, arg0, arg1, arg2, args3);
      }
    }

    public static void WriteLine(string format, params object[] args)
    {
      Console.WriteLine(DateTime.Now.ToShortDateString() + ": " + format, args);
    }

  }
}
