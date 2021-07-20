using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace BaselessJumping.Internals
{
    // TODO: Finish Logger
    public sealed class Logger
    {
        private readonly string writeTo;
        public string Name { get; }

        private readonly Assembly assembly;

        public enum WriteType
        {
            Info,
            Warn,
            Error,
            Debug
        }

        public Logger(string writeFile, string name)
        {
            assembly = Assembly.GetExecutingAssembly();
            Name = name;
            writeTo = writeFile;
        }
        public void Write(object write, WriteType writeType)
        {
            string withName = Path.Combine(writeTo, $"{Name}.log");
            using var stream = new StreamWriter(withName);
            stream.WriteLine($"[{assembly.GetName().Name}] [{writeType}]: {write}");
            // File.WriteAllLines(withName, new string[] { $"[{assembly.GetName().Name}] [{writeType}]: {write}" });
            Debug.WriteLine($"[{assembly.GetName().Name}] [{writeType}]: {write}");
        }
    }
}