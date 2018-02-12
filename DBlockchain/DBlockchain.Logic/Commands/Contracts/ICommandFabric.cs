namespace DBlockchain.Logic.Commands.Contracts
{
    public interface ICommandFabric
    {
        void RunCommand(string input);
    }
}
