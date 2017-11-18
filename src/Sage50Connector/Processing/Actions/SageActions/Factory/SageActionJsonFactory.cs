using System;

namespace Sage50Connector.Processing.Actions.SageActions.Factory
{
    /// <summary>
    /// Creates Sage Actions from JSON strings
    /// </summary>
    class SageActionJsonFactory : ISageActionFactory
    {
        public SageAction Create(object obj)
        {
            if (!(obj is string json))
                return null;

            // TODO need to implement factory

            throw new NotImplementedException();
        }
    }
}