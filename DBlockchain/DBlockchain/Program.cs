using DBlockchain.Logic.Commands.Contracts;
using Ninject;
using System;

namespace DBlockchain
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "DBlockchain";

            DrawTitle();

            IKernel kernel = new StandardKernel(new NinjectBindings());

            var commandFabric = kernel.Get<ICommandFabric>();

            while (true)
            {
                var input = Console.ReadLine();

                commandFabric.RunCommand(input);
            }

            Console.ReadKey();
        }

        public static void DrawTitle()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"____________ _            _        _           _       ");
            Console.WriteLine(@"|  _  \ ___ \ |          | |      | |         (_)      ");
            Console.WriteLine(@"| | | | |_/ / | ___   ___| | _____| |__   __ _ _ _ __  ");
            Console.WriteLine(@"| | | | ___ \ |/ _ \ / __| |/ / __| '_ \ / _` | | '_ \ ");
            Console.WriteLine(@"| |/ /| |_/ / | (_) | (__|   < (__| | | | (_| | | | | |");
            Console.WriteLine(@"|___/ \____/|_|\___/ \___|_|\_\___|_| |_|\__,_|_|_| |_|");
            Console.ResetColor();
            Console.WriteLine(@"^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
        }
    }
}
