using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaselessJumping.GameContent.Shapes
{
    public class Trail : IDisposable
    {
        public void Dispose() { }

        public Quad[] Quads;

        public int length;

        public Trail(int length)
        {
            this.length = length;

            Array.Resize(ref Quads, length);
        }
    }
}
