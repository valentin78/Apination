using log4net;

namespace Sage50Connector.Temp.Workflows.FromApination
{
    class CreateInvoiceToSage50 : ISaver
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CreateInvoiceToSage50));

        public string PayloadTypeFiler => "CreateInvoice";

        public void Save(EventData data)
        {
            Log.InfoFormat("Save Invoice to Sage50: {0}", data);
        }
    }
}