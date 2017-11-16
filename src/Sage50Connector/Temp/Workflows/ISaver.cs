namespace Sage50Connector.Temp.Workflows
{
    interface ISaver
    {
        string PayloadTypeFiler { get; }
        void Save(EventData data);
    }
}