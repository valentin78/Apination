using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using log4net;
using log4net.Config;

namespace CurrentAccount.WinService
{
    /// <summary>
    /// Provides FaultExceptions for all Methods Calls of a Service that fails with an Exception
    /// </summary>
    public class SvcErrorHandlerBehaviourAttribute : Attribute, IServiceBehavior
    {
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { } //implementation not needed

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        { } //implementation not needed

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase chanDispBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = chanDispBase as ChannelDispatcher;
                if (channelDispatcher == null)
                    continue;
                channelDispatcher.ErrorHandlers.Add(new SvcErrorHandler());
            }
        }
    }

    /// <summary>
    /// Error Handler
    /// </summary>
    public class SvcErrorHandler : IErrorHandler
    {
        #region Logger

        // ReSharper disable InconsistentNaming
        private static readonly ILog log = LogManager.GetLogger(typeof(SvcErrorHandler));
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Логер сервиса
        /// </summary>
        public static ILog Log { get { return log; } }

        #endregion

        static SvcErrorHandler()
        {
            XmlConfigurator.Configure();
        }

        public bool HandleError(Exception error)
        {
            Log.Error(error);
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message msg)
        {
            if (error is FaultException)
                return;

            var faultException = new FaultException(error.Message);
            var messageFault = faultException.CreateMessageFault();
            msg = Message.CreateMessage(version, messageFault, faultException.Action);
        }
    }
}