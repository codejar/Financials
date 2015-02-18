using System;
using System.Linq;
using StructureMap.Graph;
using StructureMap.TypeRules;
using StructureMap.Configuration.DSL;

namespace Financials.Wpf.Infrastucture
{
    /// <summary>
    /// A class ending in 'Job' is always registered as a singleton
    /// </summary>
    internal class JobConventions : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (type.IsConcrete() && type.Name.EndsWith("Job"))
                registry.For(type).Singleton();
        }
    }
}
