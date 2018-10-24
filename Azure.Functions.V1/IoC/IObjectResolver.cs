using System;

namespace Azure.Functions.V1.IoC
{
    public interface IObjectResolver
    {
        object Resolve(Type type);
        T Resolve<T>();
    }
}
