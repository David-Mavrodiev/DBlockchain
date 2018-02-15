using DBlockchain.Infrastructure.Network;
using System;
using System.Collections.Generic;
using System.Net;

namespace DBlockchain.Infrastructure.Command.Contracts
{
    public interface IGlobalCommand
    {
        bool ValidateInput(string[] args);

        string Send(string[] args);

        string Aggregate(SocketDataBody data);

        void Receive(SocketDataBody data);

        List<Tuple<IPAddress, int>> GetTargets(string[] args);
    }
}
