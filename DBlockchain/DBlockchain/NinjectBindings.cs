using DBlockchain.Infrastructure.Network;
using DBlockchain.Infrastructure.Network.Fabrics;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;
using DBlockchain.Logic.Commands.Contracts;
using DBlockchain.Logic.Commands.Fabrics;
using DBlockchain.Logic.Models;
using Ninject.Modules;

namespace DBlockchain
{
    //public class NinjectBindings : NinjectModule
    //{
    //    public override void Load()
    //    {
    //        Bind<IRequestFabric>().To<RequestFabric>();
    //        Bind<IResponseFabric>().To<ResponseFabric>();
    //        Bind<AsyncClient>().To<AsyncClient>();
    //        Bind<AsyncListener>().To<AsyncListener>();
    //        Bind<ICommandFabric>().To<CommandFabric>();
    //        Bind<Blockchain>().To<Blockchain>().InSingletonScope();
    //    }
    //}
}
