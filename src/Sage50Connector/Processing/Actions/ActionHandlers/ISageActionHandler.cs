using System;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions.ActionHandlers
{
    /// <summary>
    /// Provides methods to handle (process) Sage Actions
    /// </summary>
    public interface ISageActionHandler<in TAction>: IDisposable where TAction: SageAction
    {
        bool Handle(TAction action);
    }
}