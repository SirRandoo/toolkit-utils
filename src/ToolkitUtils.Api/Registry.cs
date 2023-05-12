using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using ToolkitUtils.Data.Models;

namespace ToolkitUtils.Api
{
    /// <summary>
    ///     A class for storing unique, one-time objects that contain data
    ///     required for a given system to function, like RimWorld items are
    ///     required by item purchasing systems.
    /// </summary>
    /// <typeparam name="T">
    ///     An <see cref="IIdentifiable"/> implementation
    ///     that describes the data being stored within the registry.
    /// </typeparam>
    public class Registry<T> where T : IIdentifiable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly List<T> _registry = new List<T>();
        private readonly Dictionary<string, T> _registryKeyed = new Dictionary<string, T>();

        /// <summary>
        ///     Returns a read-only copy of the objects currently registered.
        /// </summary>
        [NotNull]
        public IReadOnlyList<T> AllRegistrants
        {
            get
            {
                if (!_lock.TryEnterReadLock(300))
                {
                    return new List<T>(0);
                }

                var copy = new List<T>(_registry);
                _lock.ExitReadLock();

                return copy;
            }
        }

        /// <summary>
        ///     Gets a registered object by its id.
        /// </summary>
        /// <param name="id">The id of the object being retrieved.</param>
        /// <returns>
        ///     The object with the given id, or a <see langword="default"/>
        ///     instance of the class (typically <see langword="null"/>).
        /// </returns>
        [CanBeNull]
        public T Get([NotNull] string id)
        {
            if (!_lock.TryEnterReadLock(300))
            {
                return default;
            }

            if (!_registryKeyed.TryGetValue(id, out T obj))
            {
                _lock.ExitReadLock();

                return default;
            }

            _lock.ExitReadLock();

            return obj;
        }

        /// <summary>
        ///     Gets a registered object by its name.
        /// </summary>
        /// <param name="name">The name of the object being retrieved.</param>
        /// <returns>
        ///     The object with the given name, or a
        ///     <see langword="default"/> instance of the class (typically
        ///     <see langword="null"/>).
        /// </returns>
        [CanBeNull]
        public T GetNamed([NotNull] string name)
        {
            if (!_lock.TryEnterReadLock(300))
            {
                return default;
            }

            try
            {
                foreach (T obj in _registry)
                {
                    if (!string.Equals(obj.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    _lock.ExitReadLock();

                    return obj;
                }
            }
            catch (Exception)
            {
                _lock.ExitReadLock();

                return default;
            }

            _lock.ExitReadLock();

            return default;
        }

        /// <summary>
        ///     Registers an object to the registry.
        /// </summary>
        /// <param name="obj">The object to register within the registry.</param>
        /// <returns>
        ///     Whether the object was successfully registered. If the return
        ///     value is <see langword="false"/>, the object has already been
        ///     registered.
        /// </returns>
        public bool Register([NotNull] T obj)
        {
            if (!_lock.TryEnterWriteLock(300))
            {
                return false;
            }

            if (!_registryKeyed.TryAdd(obj.Id, obj))
            {
                _lock.ExitWriteLock();

                return false;
            }

            _registry.Add(obj);
            _lock.ExitWriteLock();

            return true;
        }

        /// <summary>
        ///     Unregisters an object from the registry.
        /// </summary>
        /// <param name="obj">The object to unregister from the registry.</param>
        /// <returns>
        ///     Whether the object was successfully unregistered. If the
        ///     return value is <see langword="false"/>, the object was already
        ///     unregistered or never registered.
        /// </returns>
        public bool Unregister([NotNull] T obj)
        {
            if (!_lock.TryEnterWriteLock(300))
            {
                return false;
            }

            bool unregisteredKeyed = _registryKeyed.Remove(obj.Id);
            bool unregistered = _registry.Remove(obj);

            _lock.ExitWriteLock();

            return unregistered || unregisteredKeyed;
        }

        /// <summary>
        ///     Unregisters an object from the registry by its id.
        /// </summary>
        /// <param name="id">The id of the object to unregister</param>
        /// <returns>
        ///     Whether the object was successfully unregistered. If the
        ///     return value is <see langword="false"/>, the object was already
        ///     unregistered or never registered.
        /// </returns>
        public bool Unregister([NotNull] string id)
        {
            if (!_lock.TryEnterWriteLock(300))
            {
                return false;
            }

            if (!_registryKeyed.TryGetValue(id, out T obj))
            {
                _lock.ExitWriteLock();

                return false;
            }

            bool removedKeyed = _registryKeyed.Remove(id);
            bool removed = _registry.Remove(obj);

            _lock.ExitWriteLock();

            return removedKeyed || removed;
        }
    }
}
