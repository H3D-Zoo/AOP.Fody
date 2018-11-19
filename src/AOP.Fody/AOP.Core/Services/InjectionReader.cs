using AOP.Core.Contracts;
using AOP.Core.Extensions;
using AOP.Core.Models;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using Fody;

namespace AOP.Core.Services
{
    public class InjectionReader : IInjectionReader
    {
        private readonly IAspectReader _aspectReader;
        private readonly BaseModuleWeaver _weaver;

        public InjectionReader(IAspectReader aspectReader, BaseModuleWeaver weaver)
        {
            _aspectReader = aspectReader;
            _weaver = weaver;
        }

        public IReadOnlyCollection<Injection> ReadAll(ModuleDefinition module)
        {
            var aspects = ExtractInjections(module);

            aspects = aspects.Concat(ExtractInjections(module));

            foreach (var type in module.GetTypes())
            {
                aspects = aspects.Concat(ExtractInjections(type));

                aspects = aspects.Concat(type.Events.SelectMany(ExtractInjections));
                aspects = aspects.Concat(type.Properties.SelectMany(ExtractInjections));
                aspects = aspects.Concat(type.Methods.Where(m => m.IsNormalMethod() || m.IsConstructor).SelectMany(ExtractInjections));
            }

            aspects = aspects.GroupBy(a => a).Select(g => g.Aggregate(MergeInjections)).ToList();

            return aspects.ToList();
        }

        protected virtual IEnumerable<Injection> ExtractInjections(ICustomAttributeProvider target)
        {
            var injections = Enumerable.Empty<Injection>();

            foreach (var attr in target.CustomAttributes.Where(a => a.AttributeType.FullName == WellKnownTypes.Inject).ToList())
                injections = injections.Concat(ParseInjectionAttribute(target, attr));


            return injections;
        }

        private IEnumerable<Injection> ParseInjectionAttribute(ICustomAttributeProvider target, CustomAttribute attr)
        {
            var aspectRef = attr.GetConstructorValue<TypeReference>(0);
            var aspect = _aspectReader.Read(aspectRef.Resolve());

            if (aspect == null)
            {
                _weaver.LogError($"Type {aspectRef.FullName} should be an aspect class.");
                return Enumerable.Empty<Injection>();
            }

            ushort priority = /* attr.GetPropertyValue<Broker.Inject, ushort>(i => i.Priority)*/ 0;

            // var childFilter = attr.GetPropertyValue<Broker.Inject, InjectionChildFilter>(i => i.Filter);

            var injections = FindApplicableMembers(target, aspect, priority/*, childFilter*/);

            return injections;
        }

        private IEnumerable<Injection> FindApplicableMembers(ICustomAttributeProvider target, AspectDefinition aspect, ushort priority)
        {
            var result = Enumerable.Empty<Injection>();

            if (target is AssemblyDefinition assm)
                result = result.Concat(assm.Modules.SelectMany(nt => FindApplicableMembers(nt, aspect, priority)));

            if (target is ModuleDefinition module)
                result = result.Concat(module.Types.SelectMany(nt => FindApplicableMembers(nt, aspect, priority)));

            if (target is IMemberDefinition member)
                result = result.Concat(CreateInjections(member, aspect, priority));

            if (target is TypeDefinition type)
            {
                result = result.Concat(type.Methods.Where(m => m.IsNormalMethod() || m.IsConstructor)
                    .SelectMany(m => FindApplicableMembers(m, aspect, priority)));
                result = result.Concat(type.Events.SelectMany(m => FindApplicableMembers(m, aspect, priority)));
                result = result.Concat(type.Properties.SelectMany(m => FindApplicableMembers(m, aspect, priority)));
                result = result.Concat(type.NestedTypes.SelectMany(nt => FindApplicableMembers(nt, aspect, priority)));
            }

            return result;
        }

        private IEnumerable<Injection> CreateInjections(IMemberDefinition target, AspectDefinition aspect, ushort priority)
        {
            if (target is TypeDefinition && target.CustomAttributes.Any(a => a.AttributeType.FullName == WellKnownTypes.Aspect))
                return Enumerable.Empty<Injection>();

            return aspect.Effects.Where(e => e.IsApplicableFor(target)).Select(e => new Injection()
            {
                Target = target,
                Source = aspect,
                Priority = priority,
                Effect = e
            });
        }

        private Injection MergeInjections(Injection a1, Injection a2)
        {
            a1.Priority = Enumerable.Max(new[] { a1.Priority, a2.Priority });
            return a1;
        }
    }
}