using Sage50Connector.Models;

namespace Sage50Connector.Temp.Observer.Interface
{
    internal interface IObserverFabrik
    {
        IObserver Create(Config config);
    }
}