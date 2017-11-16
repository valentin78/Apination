using Sage50Connector.Temp.Observer;

namespace Sage50Connector.Temp.Workflows
{
    interface IChecker
    {
        void Check(IObserver o);
    }
}