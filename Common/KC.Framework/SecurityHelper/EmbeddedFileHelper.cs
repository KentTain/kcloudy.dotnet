using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KC.Framework.SecurityHelper
{
    public static class EmbeddedFileHelper
    {
        private static IDictionary<string, List<string>> cachedFileContents = new Dictionary<string, List<string>>();
        public static byte[] GetEmbeddedFileBytes(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies.Where(x => x.FullName != null && !x.FullName.StartsWith("Anonymously")))
            {
                try
                {
                    using Stream stream = assembly.GetManifestResourceStream(name);
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    return bytes;
                }
                catch
                {
                }
            }
            return null;
        }

        public static List<string> GetEmbeddedFileContents(string name)
        {
            if (cachedFileContents.ContainsKey(name))
                return cachedFileContents[name];

            var lines = new List<string>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies.Where(x => x.FullName != null && !x.FullName.StartsWith("Anonymously")))
            {
                try
                {
                    using Stream stream = assembly.GetManifestResourceStream(name);
                    if (stream == null) continue;
                    var textStreamReader = new StreamReader(stream, new UTF8Encoding());
                    var line = textStreamReader.ReadLine();
                    while (line != null)
                    {
                        if (line.Trim().Length > 0)
                            lines.Add(line);
                        line = textStreamReader.ReadLine();
                    }
                }
                catch
                {
                }
            }

            if (lines.Count > 0)
            {
                cachedFileContents.Add(name, lines);
            }

            return lines;
        }

        public static Stream GetEmbeddedFileStream(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies.Where(x => x.FullName != null && !x.FullName.StartsWith("Anonymously")))
            {
                try
                {
                    var stream = assembly.GetManifestResourceStream(name);
                    return stream;
                }
                catch
                {
                }
            }
            return null;
        }

        public static string GetEmbeddedFileText(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies.Where(x => x.FullName != null && !x.FullName.StartsWith("Anonymously")))
            {
                try
                {
                    using Stream stream = assembly.GetManifestResourceStream(name);
                    var textStreamReader = new StreamReader(stream, new UTF8Encoding());
                    var text = textStreamReader.ReadToEnd();
                    return text;
                }
                catch
                {
                }
            }
            return null;
        }

        public static List<string> ToStatements(List<string> sqlContentLines)
        {
            StringComparison noCase = StringComparison.OrdinalIgnoreCase;

            var statements = new List<string>();
            var statementBuilder = new StringBuilder();
            for (int i = 0, count = sqlContentLines.Count; i < count; i++)
            {
                var line = sqlContentLines[i];
                if (line.Trim().Equals("GO", noCase))
                {
                    statements.Add(statementBuilder.ToString());
                    statementBuilder.Clear();
                }
                else
                {
                    statementBuilder.AppendLine(line);
                }
            }
            if (statementBuilder.Length > 0)
                statements.Add(statementBuilder.ToString());
            return statements;
        }
    }
}
