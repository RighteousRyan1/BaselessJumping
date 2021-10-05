using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaselessJumping.Net
{
    public class Server
    {
        public BinaryReader reader;
        public BinaryWriter writer;

        public Client[] ConnectedClients { get; }

        public Packet[] ContainedData { get; set; } = new Packet[100];

        public void SendPacketsToClients()
        {
            foreach (var packet in ContainedData)
                foreach (var client in ConnectedClients)
                    packet.Send(client, this, packet.buffer);
        }

        public void ReceivePacket()
        {
            OnPacketReceive?.Invoke(this, new());
        }

        public event EventHandler OnPacketReceive;
    }
}
