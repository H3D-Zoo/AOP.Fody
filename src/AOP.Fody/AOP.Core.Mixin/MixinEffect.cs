using System;
using AOP.Core.Contracts;
using AOP.Core.Extensions;
using AOP.Core.Models;
using Mono.Cecil;
using Fody;

namespace AOP.Core.Mixin
{
    internal class MixinEffect : Effect
    {
        public TypeReference InterfaceType { get; set; }

        public override bool IsApplicableFor(IMemberDefinition target)
        {
            return target is TypeDefinition;
        }

        protected override bool IsEqualTo(Effect effect)
        {
            var other = effect as MixinEffect;

            if (effect == null)
                return false;

            return other.InterfaceType.Match(InterfaceType);
        }

        public override bool Validate(AspectDefinition aspect,BaseModuleWeaver weaver)
        {
            if (!InterfaceType.Resolve().IsInterface)
            {
                weaver.LogError($"{InterfaceType.FullName} is not an interface.");
                return false;
            }

            if (!aspect.Host.Implements(InterfaceType))
            {
                weaver.LogError($"{aspect.Host.FullName} should implement {InterfaceType.FullName}.");
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"Mixin::{InterfaceType.Name}";
        }
    }
}