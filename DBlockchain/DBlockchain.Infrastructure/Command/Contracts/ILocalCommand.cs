namespace DBlockchain.Infrastructure.Command.Contracts
{
    public interface ILocalCommand
    {
        void Run(string[] args);
    }
}
