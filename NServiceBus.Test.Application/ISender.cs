using System.Threading.Tasks;

namespace NServiceBus.Test.Application
{
    public interface ISender
    {
        Task PublishStringMessage(string message);

        Task PublishComplexMessage(string description, string username, int count);
    }
}
