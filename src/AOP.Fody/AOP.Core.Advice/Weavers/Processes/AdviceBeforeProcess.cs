using AOP.Core.Advice.Effects;
using AOP.Core.Contracts;
using AOP.Core.Fluent;
using AOP.Core.Models;
using Fody;
using Mono.Cecil;

namespace AOP.Core.Advice.Weavers.Processes
{
    internal class AdviceBeforeProcess : AdviceWeaveProcessBase<BeforeAdviceEffect>
    {
        public AdviceBeforeProcess(BaseModuleWeaver weaver, MethodDefinition target, AspectDefinition aspect, BeforeAdviceEffect effect)
            : base(weaver, target, effect, aspect)
        {
        }

        public override void Execute()
        {
            _target.GetEditor().OnEntry(
                e => e
                .LoadAspect(_aspect)
                .Call(_effect.Method, LoadAdviceArgs)
            );
        }
    }
}