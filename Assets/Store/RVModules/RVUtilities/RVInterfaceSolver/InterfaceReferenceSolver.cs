// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RVModules.RVUtilities.RVInterfaceSolver
{
    /// <summary>
    /// Collects all interfaces from given objects list, and their parent calsses and passes it for relevant
    /// objects implementing IneedInterface<>
    /// </summary>
    public class InterfaceReferenceSolver
    {
        #region Fields

        private List<object> objects = new List<object>();

        #endregion

        /// <summary>
        /// Default constructor, you need to use AddObjectsWithInterfaces to pass them
        /// </summary>
        public InterfaceReferenceSolver()
        {
        }

        /// <summary>
        /// Creates IRS object, and add objects from array
        /// </summary>
        public InterfaceReferenceSolver(object[] _objects) => AddObjectsWithIterfaces(_objects);

        #region Public methods

        public void AddObjectsWithIterfaces(object[] _objects)
        {
            foreach (var o in _objects)
                if (o == null)
                    Debug.Log("InterfaceReferenceSolver, AddObjectsWithIterfaces: Passed object '" + o + "' is null!");
            objects.AddRange(_objects);
        }

        public void PassInterfaces()
        {
            // First collect all possible interfaces
            var allInterfacesContainers = GetAllInterfacesContainers(objects);

            foreach (var container in allInterfacesContainers)
            {
                if (container.implementingObject == null)
                    continue;

                PassInterfacesForObject(container, allInterfacesContainers.ToArray());
            }
        }

        #endregion

        #region Not public methods

        private List<InterfacesContainer> GetAllInterfacesContainers(List<object> _objects)
        {
            var allInterfacesContainers =
                new List<InterfacesContainer>();
            foreach (var obj in _objects)
            {
                if (obj == null)
                    continue;

                var interfaces = obj.GetType().GetInterfaces().ToList();
                var inheritedTypes = GetAllParentClasses(obj.GetType());

                // get interfaces also from inherited types
                foreach (var type in inheritedTypes)
                    interfaces.AddRange(type.GetInterfaces());

                var iNeedTypes = interfaces.Where(_i => _i.Name == "INeedInterface`1").ToArray();
                var iHaveTypes = interfaces.Where(_i => _i.Name == "IHaveInterface`1").ToArray();
                var iImplementTypes = interfaces.ToArray();

                var interfacesContainer = new InterfacesContainer
                {
                    interfaceINeed = iNeedTypes,
                    implementingObject = obj,
                    interfacesIImplement = interfaces.ToArray(),
                    interfacesIHave = iHaveTypes
                };

                allInterfacesContainers.Add(interfacesContainer);
                // get interfaces from IHaveInterface implementations
                //for (int i = 0; i < interfaces.Count; i++)
                //{
                //    if (interfaces[i].Name == "IHaveInterface`1")
                //    {
                //        MethodInfo mi = interfaces[i].GetMethod("GetInterfaceInstance");
                //        Type interfaceReturnType = mi.ReturnType;
                //        object interfaceReturnedInstance = mi.Invoke(obj, null);
                //        if(interfaceReturnedInstance != null)
                //            interfaces.Add(interfaceReturnType);

                //        InterfacesContainer ic = new InterfacesContainer()
                //        { implementationType = ImplementationType.ihaveInterface,
                //            implementingObject = obj,
                //            interfaces = obj.GetType().GetInterfaces().ToList()
                //        };
                //    }
                //}
            }

            return allInterfacesContainers;
        }

        // _container is IC for whom we will solve interfaces(pass for all his INeedInterface
        private void PassInterfacesForObject(InterfacesContainer _container, InterfacesContainer[] _interfacesContainers)
        {
            // loop through our IC needed interfaces
            foreach (var neededType in _container.interfaceINeed)
                // loop through all InterfacesContainers
            foreach (var interfacesContainer in _interfacesContainers)
            {
                // loop through single InterfacesContainer's implementations
                foreach (var type in interfacesContainer.interfacesIImplement)
                    if (neededType.GetGenericArguments()[0] == type)
                    {
                        var mi = neededType.GetMethod("PassInterface");
                        mi.Invoke(_container.implementingObject, new[] {interfacesContainer.implementingObject});
                    }

                foreach (var type in interfacesContainer.interfacesIHave)
                    if (neededType.GetGenericArguments()[0] == type.GetGenericArguments()[0])
                    {
                        var passInterfaceMethodInfomi = neededType.GetMethod("PassInterface");
                        var getInterfaceMethodInfomi = type.GetMethod("GetInterfaceInstance");

                        var objectImplementingNeededInterface =
                            getInterfaceMethodInfomi.Invoke(interfacesContainer.implementingObject, null);

                        passInterfaceMethodInfomi.Invoke(_container.implementingObject,
                            new[] {objectImplementingNeededInterface});
                    }
            }
        }

        private List<Type> GetAllParentClasses(Type _type)
        {
            var types = new List<Type>();

            var type = _type.BaseType;
            while (type != null)
            {
                types.Add(type);
                type = type.BaseType;
            }

            return types;
        }

        #endregion

        private class InterfacesContainer
        {
            #region Fields

            public object implementingObject;
            public Type[] interfacesIImplement;
            public Type[] interfaceINeed;
            public Type[] interfacesIHave;

            #endregion
        }
    }
}