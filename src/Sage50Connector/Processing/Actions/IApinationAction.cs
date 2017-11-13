using Sage50Connector.API;

namespace Sage50Connector.Processing.Actions
{
    internal interface IApinationAction<in TModel>
    {
        void Execute(Sage50Api api, TModel model);
    }
}