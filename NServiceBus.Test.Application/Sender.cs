using System;
using System.Threading.Tasks;
using NServiceBus.Test.Domain.Configuration;
using NServiceBus.Test.Domain.Events;
//using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus;
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
        private IEventPublisher _publisher;
        //private IUnitOfWorkContext _unitOfWorkContext;

        public Sender(NServiceBusConfiguration nServiceBusConfig, IEventPublisher publisher)
        //, IUnitOfWorkContext unitOfWorkContext
        //, ILog log)
        {
            //_log = log;
            _nServiceBusConfig = nServiceBusConfig;
            _publisher = publisher;
            //_unitOfWorkContext = unitOfWorkContext;
        }

        public async Task Send(string message)
        {
            try
            {
                //_log.Info($"Sender is sending message '{message}'.");
                var messageCarrier = new StringMessageEvent { Message = message };

                await _publisher.Publish(messageCarrier);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
