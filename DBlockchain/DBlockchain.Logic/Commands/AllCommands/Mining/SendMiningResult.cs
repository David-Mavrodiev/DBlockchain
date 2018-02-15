using System;
using System.Collections.Generic;
using System.Net;
using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Infrastructure.Network;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("send-mining-result", "{0}", CommandType.Global)]
    public class SendMiningResult : IGlobalCommand
    {
        public string Aggregate()
        {
            throw new NotImplementedException();
        }

        public List<Tuple<IPAddress, int>> GetTargets(string[] args)
        {
            throw new NotImplementedException();
        }

        public void Receive(SocketDataBody data)
        {
            throw new NotImplementedException();
        }

        public string Send(string[] args)
        {
            throw new NotImplementedException();
        }

        public bool ValidateInput(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
