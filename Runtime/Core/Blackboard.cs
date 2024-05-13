using System;
using System.Collections.Generic;

namespace Shipico.BehaviourTrees
{
    public class Blackboard
    {
        [Serializable]
        public struct Key<T>
        {
            public string name;

            public Key(string name)
            {
                this.name = name;
            }

            public static implicit operator string(Key<T> key) => key.name;
            public static implicit operator Key<T>(string name) => new(name);
        }

        public class Property<T>
        {
            private T _value;
            public T Value
            {
                get => _value;
                set
                {
                    if (Equals(value, _value))
                    {
                        return;
                    }
                    _value = value;
                    OnValueChanged?.Invoke(value);
                }
            }

            public event Action<T> OnValueChanged;
        }

        /// <summary>
        /// Can be null
        /// </summary>
        private readonly Blackboard _parentBoard;
        private readonly Dictionary<string, Type> _propertiesTypes = new();
        
        // all of them are generic properties from T
        private readonly Dictionary<string, object> _properties = new(); 

        public Blackboard(Blackboard parentBoard = null)
        {
            _parentBoard = parentBoard;
        }

        public void SetValue<T>(Key<T> key, T value)
        {
            var type = typeof(T);
            Property<T> property;
            if (!_propertiesTypes.ContainsKey(key))
            {
                _propertiesTypes.Add(key, type);
                property = new Property<T>();
                _properties.Add(key, property);
            }
            else if (!_properties.ContainsKey(key))
            {
                throw new Exception(
                    $"Expected to have property {key} of type {type.FullName}, but is not here, check blackboard implementation");
            }
            else
            {
                property = _properties[key] as Property<T>;
                if (property == null)
                {
                    throw new Exception("Cannot cast property to Property, check blackboard implementation");
                }
            }

            property.Value = value;
        }

        private bool HasProperty(string key, Type type) => _propertiesTypes.ContainsKey(key)
                                                           && _propertiesTypes[key] == type
                                                           && _properties.ContainsKey(key);

        public T GetRawValueOr<T>(Key<T> key, T defaultValue = default)
        {
            if (TryGetRawValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public Property<T> GetPropertyOrThrowError<T>(Key<T> key)
        {
            if (TryGetProperty(key, out var property))
            {
                return property;
            }

            throw new KeyNotFoundException($"Cannot find value of {key.name} property of type {typeof(T).FullName}");
        }

        public T GetValueOrThrowError<T>(Key<T> key)
        {
            if (TryGetRawValue(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Cannot find value of {key.name} property of type {typeof(T).FullName}");
        }

        public bool TryGetRawValue<T>(Key<T> key, out T value)
        {
            if (TryGetProperty(key, out var property))
            {
                value = property.Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryGetProperty<T>(Key<T> key, out Property<T> property)
        {
            var type = typeof(T);
            property = null;
            if (HasProperty(key.name, type))
            {
                property = _properties[key.name] as Property<T>;
                return true;
            }

            return _parentBoard?.TryGetProperty(key, out property) ?? false;
        }
    }
}