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

            IKernel kernel = new StandardKernel(new NinjectBindings());

            var commandFabric = kernel.Get<ICommandFabric>();

            while (true)
            {
                var input = Console.ReadLine();

                commandFabric.RunCommand(input);
            }
        }
    }
}
