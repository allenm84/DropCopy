using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dropcopy.tests
{
  [TestClass]
  public class Tests
  {
    const string OutputDirectory = @"D:\Downloads\Apples";

    private static void DeleteOutputDirectory()
    {
      if (Directory.Exists(OutputDirectory))
      {
        Directory.Delete(OutputDirectory, true);
      }
    }

    [ClassInitialize]
    public static void Setup(TestContext context)
    {
      DeleteOutputDirectory();
    }

    [TestCleanup]
    public void Teardown()
    {
      DeleteOutputDirectory();
    }

    [TestMethod]
    public void TestCopyFileToFile()
    {
      string filepath = Path.Combine(OutputDirectory, "apples.xml");
      if (!Directory.Exists(OutputDirectory))
      {
        Directory.CreateDirectory(OutputDirectory);
      }
      File.WriteAllText(filepath, "");

      var copy = new DropCopy(@"Programs\BillPayer\billpayer.xml", filepath);
      copy.Run();

      File.Delete(filepath);
    }

    [TestMethod]
    public void TestCopyFileToDirectory()
    {
      var copy = new DropCopy(@"Programs\BillPayer\billpayer.xml", OutputDirectory);
      copy.Run();
    }

    [TestMethod]
    public void TestCopyDirectoryToDirectory()
    {
      var copy = new DropCopy(@"Downloads\Projects\WindowsFormsApplication1", OutputDirectory);
      copy.Run();
    }
  }
}