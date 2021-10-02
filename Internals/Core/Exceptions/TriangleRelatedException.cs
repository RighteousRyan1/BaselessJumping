using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaselessJumping.Internals.Core.Exceptions
{
    class TriangleRelatedException : Exception
    {
        public TriangleRelatedException() : base() { }

        public TriangleRelatedException(string message) : base(message) { }

        public TriangleRelatedException(string message, object parameter) : base(message + $" | parameter name: {parameter.GetType().Name}") { }
    }
}
