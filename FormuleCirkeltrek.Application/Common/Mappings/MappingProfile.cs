using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace FormuleCirkeltrek.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        const string Mapping_Method = "Mapping";

        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod(Mapping_Method);
                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}
