using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Win32;
namespace Flow.Launcher.Plugin.RegFlow.Tests;

public class RegTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        RegInfo info = RegInfo.get("Computer\\HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run");
        Console.WriteLine(info.fullpath);
        for (int i = 0; i < info.subValues.Length; i++)
        {
            Console.WriteLine(info.subValues[i]);
        }

        for (int i = 0; i < info.subFolders.Length; i++)
        {
            Console.WriteLine(info.subFolders[i]);
        }

    }
}
