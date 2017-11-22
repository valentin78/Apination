using System;

namespace Sage50Connector.Core
{
    public class MessageException : Exception
    {
        public MessageException(string message): base(message)
        {
        }
    }
}