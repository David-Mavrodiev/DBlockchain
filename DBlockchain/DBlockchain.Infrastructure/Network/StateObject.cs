using System.Net.Sockets;
using System.Text;

namespace DBlockchain.Infrastructure.Network
{
    public class StateObject
    {
        public Socket workSocket = null;

        public const int BufferSize = 256 * 100;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();
    }
}
