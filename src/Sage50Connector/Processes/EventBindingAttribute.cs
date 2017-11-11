using System;
using Sage50Connector.Processes.Triggers;

namespace Sage50Connector.Processes
{
    /// <summary>
    /// Event binding type attribute
    /// </summary>
    internal class EventBindingAttribute : Attribute
    {
        public EventBindingTypes Type { get; set; }
    }
}