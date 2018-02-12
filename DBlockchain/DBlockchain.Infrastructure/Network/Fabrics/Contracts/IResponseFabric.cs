namespace DBlockchain.Infrastructure.Network.Fabrics.Contracts
{
    public interface IResponseFabric
    {
        void ReceiveResponse(SocketDataBody data);
    }
}
