using AOP.Core.Contracts;
using AOP.Core.Models;
using Mono.Cecil;
using Fody;

namespace AOP.Core.Advice.Effects
{
    internal class AroundAdviceEffect : AdviceEffectBase
    {
        public override Broker.Advice.Type Type => Broker.Advice.Type.Around;

        public override bool IsApplicableFor(IMemberDefinition target)
        {
            //check args

            if (target is MethodDefinition && ((MethodDefinition)target).IsConstructor)
                return false;

            return base.IsApplicableFor(target);
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

            if (Method.ReturnType != Method.Module.TypeSystem.Object)
            {
                weaver.LogError($"Around advice {Method.FullName} should return an object. Could return null.");
                return false;
            }

            return true;
        }
    }
}