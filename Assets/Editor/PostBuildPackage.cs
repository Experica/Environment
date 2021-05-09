using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
namespace Experica.Editor
{
    public class PostBuildPackage : MonoBehaviour
    {
        static string builddir = "Build";
        static List<string> files = new List<string>()
        {
            "Environment.exe",
            "EnvironmentConfig.yaml",
            "LICENSE.md",
            "README.md",
            "UnityCrashHandler64.exe",
            "UnityPlayer.dll"
        };
        static List<string> dirs = new List<string>()
        {
            "Data",
            "Environment_Data",
            "MonoBleedingEdge"
        };

        [MenuItem("File/PostBuildPackage")]
        public static void Package()
        {
            builddir = Path.Combine("Build", $"Environment_v{Application.version}");
            if (Directory.Exists(builddir))
            {
                Directory.Delete(builddir, true);
            }
            if (!Directory.Exists(builddir))
            {
                Directory.CreateDirectory(builddir);
            }
            foreach (var f in files)
            {
                File.Copy(f, Path.Combine(builddir, f));
            }
            foreach (var d in dirs)
            {
                d.CopyDirectory(Path.Combine(builddir, d), ".mp");
            }
            Debug.Log("Build Package Finished.");
        }
    }
}
#endif