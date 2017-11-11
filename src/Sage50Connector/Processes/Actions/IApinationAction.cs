using Sage50Connector.API;

namespace Sage50Connector.Processes.Actions
{
    internal interface IApinationAction
    {
        void Execute(Sage50Api api);
    }
}