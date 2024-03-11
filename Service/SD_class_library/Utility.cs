using System;
using System.Linq;
using System.Reflection;
using System.IO;

namespace ClassLibrary
{
    public static class Utility
    {
        public static string author = "MDS DEV IT";
        public static string program = "Program pentru Sincronizare Dosare";
        public static string app_dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString();
        public static string root_dir = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).ToString();
    }
}
