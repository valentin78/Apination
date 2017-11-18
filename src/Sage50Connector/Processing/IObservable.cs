using System;

namespace Sage50Connector.Processing
{
    interface IObservable<T> { void Subscribe(Action<T> action); }
}