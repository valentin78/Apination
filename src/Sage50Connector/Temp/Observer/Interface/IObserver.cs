using System;
using Sage50Connector.Temp.Workflows;

namespace Sage50Connector.Temp.Observer.Interface
{
    internal interface IObserver: IDisposable
    {
        string Identity { get;  }

        void TriggerOnDataEvent(EventData data);
        void AddChecker(IChecker workflow);
        void Subscribe(ISaver saver);
    }
}