using Sage50Connector.API;
using Sage50Connector.Models;

namespace Sage50Connector.Processing.Triggers
{
    internal interface ISage50Trigger
    {
        void Execute(ApinationApi api, Sage50TriggersConfig triggerConfig);
    }
}