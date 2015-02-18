using System;
using System.Linq;
using StructureMap.Graph;
using StructureMap.TypeRules;
using StructureMap.Configuration.DSL;


namespace Financials.Wpf.Infrastucture
{

    /// <summary>
    /// Register interface as a singletone where the interface name matches the class
    /// </summary>
    internal class InterfaceConventions : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            // Only work on concrete types
            if (!type.IsConcrete() || type.IsGenericType) return;

            // Register against all the interfaces implemented
            // by this concrete class
            type.GetInterfaces()
                .Where(@interface => @interface.Name == string.Format("I{0}", type.Name))
                .ForEach(@interface => registry.For(@interface).Use(type).Singleton());
        }
    }



}
