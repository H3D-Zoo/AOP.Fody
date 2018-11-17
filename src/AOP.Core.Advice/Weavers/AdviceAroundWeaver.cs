using AOP.Core.Advice.Effects;
using AOP.Core.Advice.Weavers.Processes;
using AOP.Core.Contracts;
using AOP.Core.Models;
using Fody;
using Mono.Cecil;
using System;

namespace AOP.Core.Advice.Weavers
{
    public class AdviceAroundWeaver : AdviceInlineWeaver
    {
        public override byte Priority => 20;

        public AdviceAroundWeaver(BaseModuleWeaver weaver) : base(weaver)
        {
        }

        public override bool CanWeave(Injection injection)
        {
            return injection.Effect is AroundAdviceEffect &&
                (injection.Target is EventDefinition || injection.Target is PropertyDefinition || injection.Target is MethodDefinition);
        }

        protected override void WeaveMethod(MethodDefinition method, Injection injection)
        {
            if (injection.Effect is AroundAdviceEffect)
            {
                var process = new AdviceAroundProcess(_weaver, injection.Source, method, (AroundAdviceEffect)injection.Effect);
                process.Execute();
            }
            else
            {
                throw new Exception("Unknown advice type.");
            }
        }
    }
}