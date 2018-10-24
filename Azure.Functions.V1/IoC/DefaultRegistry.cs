using System.Linq;
using System.Net.Http;
using StructureMap;

namespace Azure.Functions.V1.IoC
{
    public interface ISomething
    {
    }

    public class Something : ISomething
    {
    }

    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            For<ISomething>().Use<Something>();
        }
    }
}
