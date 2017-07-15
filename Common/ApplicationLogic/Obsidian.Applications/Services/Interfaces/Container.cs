using System;
using System.Collections.Generic;

namespace Obsidian.Applications.Services.Interfaces
{
    public sealed class Container : IDisposable
    {
        readonly Dictionary<string, Descriptor> _implementationsByKey = new Dictionary<string, Descriptor>();

        bool _isDisposing;
        bool _isDisposed;

        public Container(string debugName)
        {
            if (string.IsNullOrWhiteSpace(debugName))
                throw new ArgumentException("Please supply a debug name for this Container.", nameof(debugName));
            Name = debugName;
        }

        public string Name { get; }

        public void RegisterType<TKey, TImplementation>(string instanceLabel = null, bool replaceExisting = false) where TImplementation : class
        {
            var key = GetKey<TKey>(instanceLabel);
            if (replaceExisting)
                _implementationsByKey.Remove(key);
            if (_implementationsByKey.ContainsKey(key))
                throw new Exception($"{key} is already registered. Use Register(..., replaceExisting = true) to replace.");
            _implementationsByKey.Add(key, new Descriptor
            {
                ImplementationType = typeof(TImplementation),
                Instance = null,
            });
        }


        public void RegisterObject<TKey>(object instance, string instanceLabel = null, bool replaceExisting = false)
        {
#if DEBUG
            var test = (TKey)instance;
#endif
            var key = GetKey<TKey>(instanceLabel);
            if (replaceExisting)
                _implementationsByKey.Remove(key);
#if DEBUG
            if (_implementationsByKey.ContainsKey(key))
                throw new Exception($"{key} is already registered. Use Register(..., replaceExisting = true) to replace.");
#endif
            _implementationsByKey.Add(key, new Descriptor
            {
                ImplementationType = instance.GetType(),
                Instance = instance,
            });
        }
        static string GetKey<TKey>(string instanceLabel)
        {
            return instanceLabel == null
                ? typeof(TKey).FullName
                : $"{typeof(TKey).FullName} - {instanceLabel}";
        }

        public TKey Get<TKey>(string instanceLabel = null) where TKey : class
        {
            if(_isDisposed)
                throw new ObjectDisposedException("Container is disposed.");

            var key = GetKey<TKey>(instanceLabel);
            if (!_implementationsByKey.ContainsKey(key))
            {
                var mbs = Get<IMessageBoxService>();
                Type callerType = null;
                if (mbs != null)
                {
                    callerType = mbs.GetCallersType(1);
                }
                throw new Exception($"Container: {key} is not yet registred but required by {callerType?.Name ?? "unknown"}. Register this Type/object, or register it before dependend Types ask for it.");
            }


            var descriptor = _implementationsByKey[key];

            if (descriptor.Instance != null)
                return (TKey)descriptor.Instance;
            try
            {
                descriptor.Instance = (TKey)Activator.CreateInstance(descriptor.ImplementationType);
            }

            catch (Exception e)
            {
                throw new Exception($"Container could not create instance of {descriptor.ImplementationType}. Key: {key}. {e.Message}");
            }
            return (TKey)descriptor.Instance;
        }


        public void Dispose()
        {
            if(_isDisposing)
                return;
            _isDisposing = true;

            foreach (var descriptor in _implementationsByKey)
            {
                try
                {
                    var instance = descriptor.Value.Instance as IDisposable;
                    instance?.Dispose();
                }
                catch (Exception) { }
            }
            _implementationsByKey.Clear();
        }

       

        sealed class Descriptor
        {
            public Type ImplementationType;
            public object Instance;
        }
    }


}
