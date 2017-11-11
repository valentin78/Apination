using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage50Connector.Processes.Triggers
{
    [EventBinding(Type = EventBindingTypes.CreatedCustomer)]
    class CustomerCreatedTrigger: ISage50Trigger
    {
    }
}
