using System;
using System.Collections.Generic;

namespace Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void RegisterService<T>(T service)
        {
            services[typeof(T)] = service;
        }

        public static T GetService<T>()
        {
            if (services.ContainsKey(typeof(T))) return (T)services[typeof(T)];
            else return default(T);
        }
    }
}
