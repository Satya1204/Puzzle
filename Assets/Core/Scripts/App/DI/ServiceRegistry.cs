using System;
using System.Collections.Generic;

namespace PuzzleApp.App.DI
{
    public interface IServiceRegistry : IDisposable
    {
        void RegisterInstance<TService>(TService instance);
        void RegisterSingleton<TService>(Func<IServiceRegistry, TService> factory);
        TService Resolve<TService>();
        bool TryResolve<TService>(out TService service);
    }

    /// <summary>
    /// Lightweight service container for app-level singletons.
    /// </summary>
    public sealed class ServiceRegistry : IServiceRegistry
    {
        readonly Dictionary<Type, object> _instances = new();
        readonly Dictionary<Type, Func<IServiceRegistry, object>> _factories = new();

        public void RegisterInstance<TService>(TService instance)
        {
            _instances[typeof(TService)] = instance;
        }

        public void RegisterSingleton<TService>(Func<IServiceRegistry, TService> factory)
        {
            _factories[typeof(TService)] = services => factory(services);
        }

        public TService Resolve<TService>()
        {
            if (TryResolve<TService>(out var service))
                return service;

            throw new InvalidOperationException($"Service not registered: {typeof(TService).FullName}");
        }

        public bool TryResolve<TService>(out TService service)
        {
            var type = typeof(TService);

            if (_instances.TryGetValue(type, out var existing))
            {
                service = (TService)existing;
                return true;
            }

            if (_factories.TryGetValue(type, out var factory))
            {
                var created = factory(this);
                _instances[type] = created;
                service = (TService)created;
                return true;
            }

            service = default;
            return false;
        }

        public void Dispose()
        {
            var disposed = new HashSet<object>();

            foreach (var instance in _instances.Values)
            {
                if (instance == null || !disposed.Add(instance))
                    continue;

                if (instance is IDisposable disposable)
                    disposable.Dispose();
            }

            _instances.Clear();
            _factories.Clear();
        }
    }
}
