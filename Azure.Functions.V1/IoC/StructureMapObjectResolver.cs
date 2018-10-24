using System;
using StructureMap;

namespace Azure.Functions.V1.IoC
{
    public class StructureMapObjectResolver : IObjectResolver
    {
        private static IContainer _container;
        private static readonly object _lockObject = new object();

        protected IContainer Container
        {
            get
            {
                if (_container != null)
                    return _container;

                lock (_lockObject)
                {
                    return _container ?? (_container = new Container(c => { c.AddRegistry<DefaultRegistry>(); }));
                }

            }
        }

        public object Resolve(Type type)
        {
            return Container.GetInstance(type);
        }

        public T Resolve<T>()
        {
            return Container.GetInstance<T>();
        }
    }
}
