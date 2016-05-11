using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OpenRSC
{
  public class Settings : Dictionary<string, string>
  {

    public Settings() { }

    public void Load()
    {
      string filename = "";
      if (File.Exists("openrsc.ini")) { filename = "openrsc.ini"; }
      if (File.Exists("codrsc.ini")) { filename = "codrsc.ini"; }
      if (File.Exists("srcrsc.ini")) { filename = "srcrsc.ini"; }
      using (StreamReader sr = new StreamReader(filename))
      {
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine();
          line = line.Trim();
          if (line.StartsWith("//")) { continue; }
          if (line.StartsWith("[") && line.EndsWith("]"))
          {
            this.Add(line.Substring(1, line.Length - 2), sr.ReadLine());
          }
        }
      }
    }

  }
}
