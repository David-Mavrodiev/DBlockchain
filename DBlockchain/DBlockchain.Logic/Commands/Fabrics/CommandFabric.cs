using DBlockchain.Infrastructure.Command.Helpers;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;
using DBlockchain.Logic.Commands.AllCommands.Mining;
using DBlockchain.Logic.Commands.Contracts;
using DBlockchain.Logic.Models;
using DBlockchain.Logic.Wallet;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace DBlockchain.Logic.Commands.Fabrics
{
    public class CommandFabric : ICommandFabric
    {
        private static IRequestFabric requestFabric;
        private static AsyncListener listener;
        private static Blockchain blockchain;
        private static IDictionary<string, int> threadsInfo;

        public CommandFabric(IRequestFabric fabric, AsyncListener asyncListener, Blockchain blockchain)
        {
            WalletProvider.UnlockWallet();
            requestFabric = fabric;
            listener = asyncListener;
            Blockchain = blockchain;
            listener.StartListening();
            threadsInfo = new Dictionary<string, int>();
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

        public static IDictionary<string, int> ThreadsInfo
        {
            get
            {
                return threadsInfo;
            }
        }

        public static void RunDynamic(string input)
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

                if (global.Item1 != null)
                {
                    var globalCommand = global.Item1;
                    var attribute = global.Item2;

                    var args = ReverseStringFormat(attribute.Template, input).ToArray();

                    if (globalCommand.ValidateInput(args))
                    {
                        requestFabric.MakeRequest(name, args, globalCommand, attribute);
                    }
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

                if (global.Item1 != null)
                {
                    var globalCommand = global.Item1;
                    var attribute = global.Item2;

                    var args = ReverseStringFormat(attribute.Template, input).ToArray();

                    if (globalCommand.ValidateInput(args))
                    {
                        requestFabric.MakeRequest(name, args, globalCommand, attribute);
                    }
                }
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
