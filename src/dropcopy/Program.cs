using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dropcopy
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length != 2)
      {
        Console.Error.WriteLine("Invalid number of arguments. Usage: dropcopy <Source> <Destination>");
        Console.Error.WriteLine("\t<Source>: The relative path of source file/directory. The path is relative to the Dropbox root folder");
        Console.Error.WriteLine("\t<Destination>: The full path of the destination file/directory");
        Environment.Exit(1);
      }
      else
      {
        new DropCopy(args[0], args[1]).Run();
      }
    }
  }
}
