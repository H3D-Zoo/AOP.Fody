using AOP.Core.Contracts;
using AOP.Core.Extensions;
using AOP.Core.Models;
using Mono.Cecil;
using System.Collections.Generic;
using static AOP.Broker.Advice;
using System;
using System.Linq;
using Fody;

namespace AOP.Core.Advice.Effects
{
    internal abstract class AdviceEffectBase : Effect
    {
        public Target Target { get; set; }
        public abstract Broker.Advice.Type Type { get; }
        public MethodDefinition Method { get; set; }

        public List<AdviceArgument> Arguments { get; set; } = new List<AdviceArgument>();

        public override bool IsApplicableFor(IMemberDefinition target)
        {
            if (Target.HasFlag(Target.Method) && target is MethodDefinition && ((MethodDefinition)target).IsNormalMethod())
                return true;

            if (Target.HasFlag(Target.Constructor) && target is MethodDefinition && ((MethodDefinition)target).IsConstructor)
                return true;

            if (Target.HasFlag(Target.Setter) && target is PropertyDefinition && ((PropertyDefinition)target).SetMethod != null)
                return true;

            if (Target.HasFlag(Target.Getter) && target is PropertyDefinition && ((PropertyDefinition)target).GetMethod != null)
                return true;

            if (Target.HasFlag(Target.EventAdd) && target is EventDefinition && ((EventDefinition)target).AddMethod != null)
                return true;

            if (Target.HasFlag(Target.EventRemove) && target is EventDefinition && ((EventDefinition)target).RemoveMethod != null)
                return true;

            return false;
        }

        protected override bool IsEqualTo(Effect effect)
        {
            var other = effect as AdviceEffectBase;

            if (other == null)
                return false;

            return other.Target == Target && other.Type == Type && other.Method == Method;
        }

        public override bool Validate(AspectDefinition aspect, BaseModuleWeaver weaver)
        {
            if (Method.IsStatic)
            {
                weaver.LogError($"Advice {Method.FullName} cannot be static.");
                return false;
            }

            if (!Method.IsPublic)
            {
                weaver.LogError($"Advice {Method.FullName} should be public.");
                return false;
            }

            if (Method.ReturnType != Method.Module.TypeSystem.Void)
            {
                weaver.LogError($"Advice {Method.FullName} should be void.");
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"{Type.ToString()}{Target.ToString()}";
        }
    }
}