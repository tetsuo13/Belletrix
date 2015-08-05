using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Belletrix.Infrastructure
{
    public class BelletrixDependencyResolver : IDependencyResolver
    {
        private IUnityContainer unityContainer;

        public BelletrixDependencyResolver(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return unityContainer.Resolve(serviceType);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return unityContainer.ResolveAll(serviceType);
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }
    }
}
