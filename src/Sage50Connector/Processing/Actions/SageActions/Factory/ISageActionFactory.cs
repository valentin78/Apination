namespace Sage50Connector.Processing.Actions.SageActions.Factory
{
    /// <summary>
    /// Provides Interface to create Acton from some object (i.e. JSON string)
    /// </summary>
    public interface ISageActionFactory
    {
        SageAction Create(string json);
    }
}