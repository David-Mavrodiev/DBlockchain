﻿using DBlockchain.Infrastructure.Network;
using System;
using System.Collections.Generic;
using System.Net;

namespace DBlockchain.Infrastructure.Command.Contracts
{
    public interface IGlobalCommand
    {
        string Send(string[] args);

        string Aggregate();

        void Receive(SocketDataBody data);

        List<Tuple<IPAddress, int>> GetTargets(string[] args);
    }
}