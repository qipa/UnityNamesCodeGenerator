﻿using System.IO;

namespace NamesCodeGenerator
{
    public static class CodeSerializer
    {
        public static void WriteCodeFile(string outputPath, string code, string typeName, string namespaceName)
        {
            string outPath = null;
            if (namespaceName != null)
            {
                var dirPath = Path.Combine(outputPath, namespaceName.Replace('.', '/'));
                CreateDirectoryIfNotExists(dirPath);
                outPath = Path.Combine(dirPath, typeName + ".cs");
            }
            else
            {
                outPath = Path.Combine(outputPath, typeName + ".cs");
            }
            File.WriteAllText(outPath, code);
        }

        public static void ResetDirectory(string directoryPath)
        {
            DeleteDirectoryIfExists(directoryPath, true);
            CreateDirectoryIfNotExists(directoryPath);
        }

        static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        static void DeleteDirectoryIfExists(string path, bool recursive = false)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive);
        }
    }
}