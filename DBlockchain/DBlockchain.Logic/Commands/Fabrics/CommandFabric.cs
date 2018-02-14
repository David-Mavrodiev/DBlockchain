﻿using DBlockchain.Infrastructure.Command.Helpers;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;
using DBlockchain.Logic.Commands.Contracts;
using DBlockchain.Logic.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DBlockchain.Logic.Commands.Fabrics
{
    public class CommandFabric : ICommandFabric
    {
        private readonly IRequestFabric requestFabric;
        private readonly AsyncListener listener;
        private static Blockchain blockchain;

        public CommandFabric(IRequestFabric requestFabric, AsyncListener listener, Blockchain blockchain)
        {
            this.requestFabric = requestFabric;
            this.listener = listener;
            Blockchain = blockchain;
            listener.StartListening();
        }

        public static Blockchain Blockchain
        {
            get
            {
                return blockchain;
            }

            set
            {
                if (blockchain == null)
                {
                    blockchain = value;
                }
            }
        }

        public void RunCommand(string input)
        {
            var name = input.Split(' ')[0];

            var local = CommandsReflector.GetLocalCommand(name);

            if (local.Item1 != null)
            {
                var localCommand = local.Item1;
                var attribute = local.Item2;

                var args = ReverseStringFormat(attribute.Template, input).ToArray();

                localCommand.Run(args);
            }
            else
            {
                var global = CommandsReflector.GetGlobalCommand(name);
                var globalCommand = global.Item1;
                var attribute = global.Item2;

                var args = ReverseStringFormat(attribute.Template, input).ToArray();

                requestFabric.MakeRequest(name, args, globalCommand, attribute);
            }
        }

        private static List<string> ReverseStringFormat(string template, string str)
        {
            string pattern = "^" + Regex.Replace(template, @"\{[0-9]+\}", "(.*?)") + "$";

            Regex r = new Regex(pattern);
            Match m = r.Match(str);

            List<string> ret = new List<string>();

            for (int i = 1; i < m.Groups.Count; i++)
            {
                ret.Add(m.Groups[i].Value);
            }

            return ret;
        }
    }
}
