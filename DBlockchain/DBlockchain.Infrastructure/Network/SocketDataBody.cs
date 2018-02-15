using Newtonsoft.Json;
using System;

namespace DBlockchain.Infrastructure.Network
{
    public class SocketDataBody
    {
        public Tuple<string, string> NodesPair;

        public string CommandName { get; set; }

        public string Body { get; set; }

        public SocketDataType Type { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
