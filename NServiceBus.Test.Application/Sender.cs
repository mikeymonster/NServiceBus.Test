using System;
using System.Threading.Tasks;
using NServiceBus.Test.Domain.Configuration;
using NServiceBus.Test.Domain.Events;
//using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.UnitOfWork;

namespace NServiceBus.Test.Application
{
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

        public async Task PublishStringMessage(string message)
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

        public async Task PublishComplexMessage(string description, string username, int count)
        {
            try
            {
                var messageEvent = new ComplexMessageEvent(description, username, count);
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

        //public static Task ProcessOutboxMessages([TimerTrigger("0 */10 * * * *")] TimerInfo timer, TraceWriter logger)
        public static Task ProcessOutboxMessages(IProcessClientOutboxMessagesJob job)
        {
            return job.RunAsync();
        }
    }
}
