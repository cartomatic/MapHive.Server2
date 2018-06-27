using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MapHive.Core.DataModel;

namespace MapHive.Core.DataModel
{
    /// <summary>
    /// Extension get type uuid for object
    /// </summary>
    public static class ObjectTypeExtensions
    {
        /// <summary>
        /// All objects types with type uuids
        /// </summary>
        private static readonly Dictionary<Type, Guid> TypesToUuids = new Dictionary<Type, Guid>();

        /// <summary>
        /// uuids to types dict
        /// </summary>
        private static readonly Dictionary<Guid, Type> UuidsToTypes = new Dictionary<Guid, Type>();


        /// <summary>
        /// Returns a list of all types that inherit from BaseObject and have been registered
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetRegisteredTypes()
        {
            var subclasses = GetSubclassingTypes();
            var registered = new List<Type>();

            foreach (var subclass in subclasses.Where(s => !s.IsAbstract))
            {
                try
                {
                    if (!registered.Contains(subclass))
                    {
                        //need to create an instance, so a static ctor registers object...
                        Activator.CreateInstance(subclass);
                        GetTypeUuid(subclass); //this line will throw if a type has not been registered
                        registered.Add(subclass);
                    }
                }
                catch
                {
                    //ignore
                    //it will pretty much discard all the subclasses that have not been registered and therefore do not have uuids
                }
            }

            return registered;

        }

        /// <summary>
        /// returns all the tyoes that subclass BaseObject
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetSubclassingTypes()
        {
            var subclasses = new List<Type>();
            var baseType = typeof(Base);

            //need to obtain all the types that inherit from BaseObject
            //and need to look in all the assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                //Note: GetTypes tends to throw if cannot load assembly...buger
                try
                {
                    //foreach (var type in assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t)))
                    foreach (var type in assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)))
                    {
                        if (!subclasses.Contains(type))
                        {
                            subclasses.Add(type);
                        }
                    }
                }
                catch
                {
                    //ignore
                }
            }

            return subclasses;
        }

        /// <summary>
        /// Registers a type's typeuuid
        /// </summary>
        /// <param name="type"></param>
        /// <param name="uuid"></param>
        public static void RegisterTypeUuid(Type type, Guid uuid)
        {
            try
            {
                UuidsToTypes.Add(uuid, type);
            }
            catch
            {
                //uhuh, looks like a guid has already been taken
                throw new Exception($"The specified type uuid has already been registered. Please try another one. Uuid: {uuid}, type: {type}.");
            }

            try
            {
                TypesToUuids.Add(type, uuid);
            }
            catch
            {
                //uhuh, looks like a type has already been registered
                throw new Exception($"The specified base type has already been registered. You cannot register the same type uuid more than once. Uuid: {uuid}, type: {type}.");
            }
        }

        /// <summary>
        /// Registers a type's typeuuid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uuid"></param>
        public static void RegisterTypeUuid<T>(Guid uuid)
            where T : Base
        {
            var type = typeof(T);

            RegisterTypeUuid(type, uuid);
        }

        /// <summary>
        /// Return type uuid for given object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid GetTypeUuid<T>(this T obj)
            where T : Base
        {
            return GetTypeUuid(obj.GetType());
        }

        /// <summary>
        /// Gets a type uuid for a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Guid GetTypeUuid<T>()
            where T : Base
        {
            return GetTypeUuid(typeof(T));
        }

        /// <summary>
        /// Returns type uuid for a type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Guid GetTypeUuid(Type t)
        {
            try
            {
                return TypesToUuids[t];
            }
            catch
            {
                //ok, type has not been registered!
                throw new Exception($"Type <{t}> has no TypeUuid registered. Please make sure to register the type uuid first! It is usually done via static constructor. Review examples in the MapHive.Core.Data namespace for more details.");
            }
        }

        /// <summary>
        /// Returns type uuid for a type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Type GetTypeFromUuid(Guid id)
        {
            try
            {
                return UuidsToTypes[id];
            }
            catch
            {
                //ok, type has not been registered!
                throw new Exception($"TypeUuid <{id}> has no Type registered. Please make sure to register the type uuid first! It is usually done via static constructor. Review examples in the MapHive.Core.Data namespace for more details.");
            }
        }
    }
}
