using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

using System.Net.FtpClient;
using System.Net;

namespace OpenRSC
{
  class Program
  {
    static string gamePath = "";
    static string tempPath = "";
    static List<string> extSkip;
    static string pidFile = AppDomain.CurrentDomain.BaseDirectory + "\\openrsc.pid";

    static void Main(string[] args)
    {
      Settings settings = new Settings();
      List<string> skiplist = new List<string>();
      extSkip = new List<string>();

      tempPath = AppDomain.CurrentDomain.BaseDirectory + "\\Temp";

      if (File.Exists(pidFile))
      {
        Log.WriteLine("Another instance is running");
        try {
          Process p = Process.GetProcessById(Convert.ToInt32(File.ReadAllText(pidFile)));
          return;
        }
        catch (ArgumentException) {
          Log.WriteLine("Process no longer exists, continuing!");
        }
      }

      using (StreamWriter sw = File.CreateText(pidFile))
      {
        sw.WriteLine(Process.GetCurrentProcess().Id);
      }
      
      //Country found, let's load the settings
      Log.WriteLine("Loading settings file...");
      settings.Load();

      string fastDLUrl = settings["Redir-FTPSrvr"];

      Log.WriteLine("Looking for {0}.rsc", settings["GameMod"]);
      if (File.Exists(settings["GameMod"] + ".rsc"))
      {
        Log.WriteLine("Skiplist/rsc file exists!");
      }
      else
      {
        Log.WriteLine("Skiplist/rsc file doesn't exist!");
        Log.WriteLine("ABORTING");
        File.Delete(pidFile);
        return;
      }

      //Check gamemod specific exceptions
      if (File.Exists(settings["GameMod"] + ".rsc"))
      {
        using (StreamReader sr = new StreamReader(settings["GameMod"] + ".rsc"))
        {
          string line;
          while ((line = sr.ReadLine()) != null)
          {
            if (!string.IsNullOrWhiteSpace(line))
              skiplist.Add(line);
          }
        }
      }
      //Default exceptions
      if (File.Exists("default.exc"))
      {
        using (StreamReader sr = new StreamReader("default.exc"))
        {
          string line;
          while ((line = sr.ReadLine()) != null)
          {
            if (!string.IsNullOrWhiteSpace(line))
              extSkip.Add(line);
          }
        }
      }
      //Already uploaded files
      if (File.Exists("uploaded.exc"))
      {
        using (StreamReader sr = new StreamReader("uploaded.exc"))
        {
          string line;
          while ((line = sr.ReadLine()) != null)
          {
            if(!string.IsNullOrWhiteSpace(line))
              extSkip.Add(line);
          }
        }
      }

      //Let's find all files yo
      Log.WriteLine("Locating files not on fastdl...");
      gamePath = settings["ServerPath"];
      string[] gameFiles = DirSearch(gamePath, skiplist);
      Log.WriteLine("Looking for files in {0}", gamePath);
      //Copy files to TEMP
      Directory.CreateDirectory(tempPath);
      foreach (string f in gameFiles)
      {
        string newDir = f.Replace(gamePath, tempPath);
        string dirInfo = new FileInfo(newDir).Directory.FullName;
        if (!Directory.Exists(dirInfo))
        {
          Directory.CreateDirectory(dirInfo);
        }
        Console.WriteLine("Copying {0} to temp", Path.GetFileName(f));
        try
        {
          File.Copy(f, newDir);
        }
        catch
        {
          Log.WriteLine("Unable to copy, most likely it already exists");
        }
      }

      if (bool.Parse(settings["Compress"]))
      {
        Log.WriteLine("Compressing files...");

        gameFiles = DirSearch(tempPath);
        foreach (string s in gameFiles)
        {
          FileInfo f = new FileInfo(s);
          if (f.Length > 157286400)
          {
            Log.WriteLine("{0} is above 150mb, skipping", Path.GetFileName(s));
            continue;
          }
          Log.WriteLine("Compressing {0}", Path.GetFileName(s));
          ProcessStartInfo bzip = new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + "\\bzip2.exe", string.Format("--fast \"{0}\"", s));
          bzip.CreateNoWindow = true;
          bzip.UseShellExecute = false;
          Process.Start(bzip).WaitForExit();
          Log.WriteLine("Done");
        }
      }

      if (bool.Parse(settings["Redir-Upload"]))
      {
        Log.WriteLine("Connecting to FTP...");

        FtpClient client = new FtpClient();
        client.Credentials = new NetworkCredential(settings["Redir-FTPUser"], settings["Redir-FTPPass"]);
        client.Host = fastDLUrl;
        client.DataConnectionType = FtpDataConnectionType.AutoActive;

        try
        {
          client.Connect();

          Log.WriteLine("Connected");

          if (!client.DirectoryExists(settings["Redir-FTPPath"]))
          {
            Log.WriteLine("Path doesn't exist on FTP, creating...");
            client.CreateDirectory(settings["Redir-FTPPath"]);
          }
          Log.WriteLine("Uploading files to FTP");
          UploadBz2(tempPath, client, settings["Redir-FTPPath"]);
        }
        catch
        {
          Log.WriteLine("Disconnected from FTP");
        }

        Log.WriteLine("Finished uploading");

        try
        {
          Log.WriteLine("Deleting temp folder");
          Directory.Delete(tempPath, true);
        }
        catch
        {
          Log.WriteLine("Unable to delete temp directory");
        }
      }
      else
      {
        Log.WriteLine("Files found and placed in temp folder");
      }
      //Cleanup pid file
      File.Delete(pidFile);
    }

    /// <summary>
    /// Search a directory recursively for files
    /// </summary>
    /// <param name="sDir">Directory to scan</param>
    /// <param name="skiplist">List of files to skip</param>
    /// <returns>List of files -skiplist</returns>
    static string[] DirSearch(string sDir, List<string> skiplist = null)
    {
      List<string> dir = new List<string>();
      string[] files = Directory.GetFiles(sDir, "*.*", SearchOption.AllDirectories);
      foreach (string f in files)
      {
        if (!extSkip.Any(f.EndsWith))
        {
          if (skiplist != null)
          {
            if (!skiplist.Any(f.EndsWith))
            {
              dir.Add(f);
            }
          }
          else
          {
            dir.Add(f);
          }
        }
      }
      return dir.ToArray();
    }

    /// <summary>
    /// Uploads all files to the specified FTP
    /// </summary>
    /// <param name="sDir">Directory to scan for upload</param>
    /// <param name="client">The FTP Client</param>
    /// <param name="currentDirectory">Current directory on FTP</param>
    static void UploadBz2(string sDir, FtpClient client, string currentDirectory)
    {
      client.SetWorkingDirectory(currentDirectory);
      try
      {
        foreach (string f in Directory.GetFiles(sDir, "*.*"))
        {
          Uri uri = new Uri(f);
          Log.WriteLine("Uploading {0}", f);
          UploadToFTP(f, Path.GetFileName(uri.LocalPath), client);
        }
        foreach (string d in Directory.GetDirectories(sDir))
        {
          client.SetWorkingDirectory(currentDirectory);
          string dirname = new DirectoryInfo(d).Name;

          if (!client.DirectoryExists(dirname))
          {
            client.CreateDirectory(dirname);
          }

          string newCurrentDir = currentDirectory + "/" + dirname;
          UploadBz2(d, client, newCurrentDir);
        }
      }
      catch
      {
        Log.WriteLine("Error uploading Bz2 Files");
      }
    }

    /// <summary>
    /// Upload specified file to FTP
    /// </summary>
    /// <param name="filepath">Local filepath</param>
    /// <param name="uploadPath">File on FTP</param>
    /// <param name="client">FTP Client</param>
    static void UploadToFTP(string filepath, string uploadPath, FtpClient client)
    {
      if (!client.IsConnected)
      {
        client.Connect();
      }
      try
      {
        using (Stream s = client.OpenWrite(uploadPath))
        {
          int totalRead = 0;
          // perform your transfer
          using (var inputStream = new FileStream(filepath, FileMode.Open))
          {
            var buffer = new byte[1024 * 1024];
            int read;
            while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
              s.Write(buffer, 0, read);
              totalRead += read;
              float fProgress = totalRead / (float)inputStream.Length;
              DrawTextProgressBar(Convert.ToInt16(fProgress * 100), 100);
            }
          }
          s.Flush();
          s.Close();
        }
        AddToException(filepath.Replace(".bz2", "").Substring(tempPath.Length+1));
        File.Delete(filepath);
        Log.WriteLine("Done");
      }
      catch (Exception e)
      {
        // Typical exceptions here are IOException, SocketException, or a FtpCommandException
        Log.WriteLine(e);
      }
    }

    /// <summary>
    /// Draws a progressbar in the CMD window
    /// </summary>
    /// <param name="progress">Progress out of load</param>
    /// <param name="total">Total to load</param>
    private static void DrawTextProgressBar(int progress, int total)
    {
      //draw empty progress bar
      Console.CursorLeft = 0;
      Console.Write("["); //start
      Console.CursorLeft = 32;
      Console.Write("]"); //end
      Console.CursorLeft = 1;
      float onechunk = 30.0f / total;

      //draw filled part
      int position = 1;
      for (int i = 0; i < onechunk * progress; i++)
      {
        Console.BackgroundColor = ConsoleColor.Gray;
        Console.CursorLeft = position++;
        Console.Write(" ");
      }

      //draw unfilled part
      for (int i = position; i <= 31; i++)
      {
        Console.BackgroundColor = ConsoleColor.Green;
        Console.CursorLeft = position++;
        Console.Write(" ");
      }

      //draw totals
      Console.CursorLeft = 35;
      Console.BackgroundColor = ConsoleColor.Black;
      Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
    }

    /// <summary>
    /// Add file to exception file
    /// </summary>
    /// <param name="file">File to add to uploaded.exc</param>
    static void AddToException(string file)
    {
      using(StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "uploaded.exc", true))
      {
        sw.WriteLine(file);
      }
    }

  }
}
