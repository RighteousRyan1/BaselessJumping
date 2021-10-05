using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaselessJumping.Net
{
    public struct Packet
    {
        public const byte READ_OVERFLOW = 8;

        public byte[] buffer;

        public void Send(Client client, Server server, byte[] data) 
        {
            client.writer.Write(data);
            server.reader.ReadBytes(data.Length);

            server.ReceivePacket();
        }
    }
}
