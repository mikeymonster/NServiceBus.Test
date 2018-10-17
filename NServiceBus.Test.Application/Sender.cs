using System;
using System.Threading.Tasks;
using NServiceBus.Test.Domain.Configuration;
using NServiceBus.Test.Domain.Events;
using NServiceBus.Test.Domain.Messages;
//using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus;
using SFA.DAS.UnitOfWork;

//using SFA.DAS.UnitOfWork;

namespace NServiceBus.Test.Application
{
    public interface ISender
    {
        Task Send(string message);
    }

    public class Sender : ISender
    {
        //private ILog _log;
        private NServiceBusConfiguration _nServiceBusConfig;
        private IUnitOfWorkContext _unitOfWorkContext;


        public IEventPublisher Publisher { get; private set; }

        public IEndpointInstance EndpointInstance { get; private set; }

        public Sender(NServiceBusConfiguration nServiceBusConfig,
                IEndpointInstance endpointInstance,
                IEventPublisher publisher,
                IUnitOfWorkContext unitOfWorkContext)
        //, ILog log)
        {
            //_log = log;
            _nServiceBusConfig = nServiceBusConfig;
            Publisher = publisher;
            EndpointInstance = endpointInstance;
            _unitOfWorkContext = unitOfWorkContext;
        }

        public async Task Send(string message)
        {
            try
            {
                //_log.Info($"Sender is sending message '{message}'.");
                //var messageCommand = new StringMessage(message);
                //System.Console.WriteLine($"Sending message: {messageCommand}");
                //await EndpointInstance.Send(messageCommand);
                //System.Console.WriteLine($"Sent message: {messageCommand}");

                var messageEvent = new StringMessageEvent(message);
                System.Console.WriteLine($"Publishing event: {messageEvent}");
                await Publisher.Publish(messageEvent);
                System.Console.WriteLine($"Published event: {messageEvent}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
