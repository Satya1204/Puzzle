using System;
using System.Collections.Generic;

namespace PuzzleApp.App.Signals
{
    public interface ISignalBus
    {
        IDisposable Subscribe<TSignal>(Action<TSignal> handler);
        void Publish<TSignal>(TSignal signal);
    }

    public sealed class SignalBus : ISignalBus, IDisposable
    {
        readonly Dictionary<Type, Delegate> _subscriptions = new();

        public IDisposable Subscribe<TSignal>(Action<TSignal> handler)
        {
            var type = typeof(TSignal);
            _subscriptions.TryGetValue(type, out var current);
            _subscriptions[type] = Delegate.Combine(current, handler);
            return new Subscription<TSignal>(this, handler);
        }

        public void Publish<TSignal>(TSignal signal)
        {
            if (!_subscriptions.TryGetValue(typeof(TSignal), out var current))
                return;

            if (current is Action<TSignal> handlers)
                handlers.Invoke(signal);
        }

        void Unsubscribe<TSignal>(Action<TSignal> handler)
        {
            var type = typeof(TSignal);
            if (!_subscriptions.TryGetValue(type, out var current))
                return;

            var updated = Delegate.Remove(current, handler);
            if (updated == null)
                _subscriptions.Remove(type);
            else
                _subscriptions[type] = updated;
        }

        public void Dispose()
        {
            _subscriptions.Clear();
        }

        sealed class Subscription<TSignal> : IDisposable
        {
            SignalBus _bus;
            Action<TSignal> _handler;

            public Subscription(SignalBus bus, Action<TSignal> handler)
            {
                _bus = bus;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_bus == null || _handler == null)
                    return;

                _bus.Unsubscribe(_handler);
                _bus = null;
                _handler = null;
            }
        }
    }
}
