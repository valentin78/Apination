using log4net;

namespace Sage50Connector.Temp.Workflows.FromApination
{
    internal class CreateCustomerToSage50 : ISaver
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(CreateCustomerToSage50));


        public string PayloadTypeFiler => "CreateCustomer";

        public void Save(EventData data)
        {
            Log.InfoFormat("Save Customer to Sage50: {0}", data);

            // save to sage50
        }
    }
}