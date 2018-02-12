using Newtonsoft.Json;

namespace DBlockchain.Infrastructure.Network
{
    public class SocketDataBody
    {
        public string CommandName { get; set; }

        public string Body { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
