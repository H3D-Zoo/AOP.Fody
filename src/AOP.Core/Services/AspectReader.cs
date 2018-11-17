using AOP.Broker;
using AOP.Core.Contracts;
using AOP.Core.Extensions;
using AOP.Core.Models;
using Mono.Cecil;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Fody;

namespace AOP.Core.Services
{
    public class AspectReader : IAspectReader
    {
        private static readonly ConcurrentDictionary<TypeDefinition, AspectDefinition> _cache = new ConcurrentDictionary<TypeDefinition, AspectDefinition>();

        private readonly IEnumerable<IEffectReader> _effectExtractors;
        private readonly BaseModuleWeaver _weaver;

        public AspectReader(IEnumerable<IEffectReader> effectExtractors, BaseModuleWeaver weaver)
        {
            _effectExtractors = effectExtractors;
            _weaver = weaver;
        }

        public IReadOnlyCollection<AspectDefinition> ReadAll(ModuleDefinition module)
        {
            return module.GetTypes().Select(Read).Where(a => a != null).Where(a => a.Validate(_weaver)).ToList();
        }

        public AspectDefinition Read(TypeDefinition type)
        {
            if (!_cache.TryGetValue(type, out var aspectDef))
            {
                var effects = ExtractEffects(type).ToList();
                var aspect = ExtractAspectAttribute(type);

                if (aspect != null)                
                    aspectDef = new AspectDefinition
                    {
                        Host = type,
                        Scope = aspect.GetConstructorValue<Aspect.Scope>(0),
                        Factory = aspect.GetPropertyValue<Aspect>(au => au.Factory) as TypeReference,
                        Effects = effects
                    };                
                else if (effects.Any())
                    _weaver.LogError($"Type {type.FullName} has effects, but is not marked as an aspect. Concider using [Aspect] attribute.");

                _cache.AddOrUpdate(type, aspectDef, (k, o) => aspectDef);
            }

            return aspectDef;
        }

        private CustomAttribute ExtractAspectAttribute(TypeDefinition type)
        {
            var aspectUsage = type.CustomAttributes.FirstOrDefault(ca => ca.AttributeType.FullName == WellKnownTypes.Aspect);
            return aspectUsage;
        }

        private IEnumerable<Effect> ExtractEffects(TypeDefinition type)
        {
            var effects = Enumerable.Empty<Effect>();

            effects = effects.Concat(ExtractEffectsFromProvider(type));
            effects = effects.Concat(type.Methods.SelectMany(ExtractEffectsFromProvider));

            return effects;
        }

        private List<Effect> ExtractEffectsFromProvider(ICustomAttributeProvider host)
        {
            var effects = new List<Effect>();

            foreach (var extractor in _effectExtractors)
                effects.AddRange(extractor.Read(host));

            return effects;
        }
    }
}