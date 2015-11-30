using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dropcopy
{
  public enum DropCopyErrorCode : int
  {
    InvalidDropBox = 100,
    JsonParseError,
    InvalidParameters,
    GenericException = 1337,
  }

  public class DropCopy
  {
    private readonly string sourceRelative;
    private readonly string destination;

    public DropCopy(string sourceRelative, string destination)
    {
      this.sourceRelative = sourceRelative;
      this.destination = destination;
    }

    public void Run()
    {
      var paths = new string[]
      {
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      };

      var info = paths
        .Select(p => Path.Combine(p, "Dropbox", "info.json"))
        .FirstOrDefault(File.Exists);

      if (info == null)
      {
        Console.Error.WriteLine("Dropbox v2.8 or later must be installed");
        Exit(DropCopyErrorCode.InvalidDropBox);
        return;
      }

      string path = ParseJson(info);

      try
      {
        string source = Path.Combine(path, sourceRelative);

        bool sourceIsFile = IsPathFile(source);
        bool destinationIsFile = IsPathFile(destination);

        if (!sourceIsFile && destinationIsFile)
        {
          Console.Error.WriteLine("Attempting to copy a directory to a file, or the source does not exist");
          Exit(DropCopyErrorCode.InvalidParameters);
          return;
        }

        if (sourceIsFile && !destinationIsFile)
        {
          CopyFile(source, Path.Combine(destination, Path.GetFileName(source)));
        }
        else if (sourceIsFile && destinationIsFile)
        {
          CopyFile(source, destination);
        }
        else
        {
          CopyDirectory(source, destination);
        }
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine(ex.ToString());
        Exit(DropCopyErrorCode.GenericException);
      }
    }

    private static void Exit(DropCopyErrorCode code)
    {
      Environment.Exit((int)code);
    }

    private static string ParseJson(string filepath)
    {
      try
      {
        dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(filepath));

        dynamic value = json.personal;
        if (value == null)
        {
          value = json.business;
        }

        return value.path.ToString();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine("Unable to parse json because {0}", ex.Message);
        Exit(DropCopyErrorCode.JsonParseError);
      }

      return null;
    }

    private static string Tabs(int count)
    {
      if (count <= 0)
      {
        return string.Empty;
      }
      else
      {
        return new string('\t', count);
      }
    }

    private static void CopyFile(string source, string destination, int indent = 0)
    {
      var directory = Path.GetDirectoryName(destination);
      if (!Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }

      File.Copy(source, destination, true);
      Console.WriteLine("{0}{1} => {2}", Tabs(indent), source, destination);
    }

    private static void CopyDirectory(string source, string destination, int indent = 0)
    {
      Console.WriteLine("{0}{1} => {2}", Tabs(indent), source, destination);

      var subdirs = Directory.EnumerateDirectories(source);
      foreach (var dir in subdirs)
      {
        var name = Path.GetFileName(dir);

        var newPath = Path.Combine(destination, name);
        if (!Directory.Exists(newPath))
        {
          Directory.CreateDirectory(newPath);
        }

        CopyDirectory(dir, newPath, indent + 1);
      }

      var files = Directory.EnumerateFiles(source);
      foreach (var file in files)
      {
        var name = Path.GetFileName(file);
        CopyFile(file, Path.Combine(destination, name), indent);
      }
    }

    private static bool IsPathFile(string path)
    {
      if (File.Exists(path))
      {
        return true;
      }
      else if (Directory.Exists(path))
      {
        return false;
      }
      else
      {
        // if we get here, it is most likely that the path is invalid. If that is
        // the case, then we should treat the path as a directory, so we return false
        // here.
        return false;
      }
    }
  }
}
