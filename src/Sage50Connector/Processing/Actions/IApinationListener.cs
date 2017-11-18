using System;
using System.Collections.Generic;
using Sage50Connector.Processing.Actions.SageActions;

namespace Sage50Connector.Processing.Actions
{
    /// <summary>
    /// Provide event that new SageActions appear
    /// </summary>
    interface IApinationListener
    {
        event EventHandler<IEnumerable<SageAction>> OnNewSageActions;
    }
}