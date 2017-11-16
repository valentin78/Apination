using Sage50Connector.Temp.Observer;
using Sage50Connector.Temp.Observer.Interface;

namespace Sage50Connector.Temp.Workflows
{
    interface IChecker
    {
        void Check(IObserver o);
    }
}