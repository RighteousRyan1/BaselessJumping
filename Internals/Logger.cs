using System.IO;
using System.Diagnostics;

namespace BaselessJumping.Internals
{
    // TODO: Finish Logger
    public sealed class Logger
    {
        public readonly string writeTo;

        public Logger(string fileToWriteIn)
        {
            writeTo = fileToWriteIn;

        }
        public void Write()
        {
        }
        public void WriteLine()
        {
        }
    }
}