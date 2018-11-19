using AOP.Core.Advice.Effects;
using AOP.Core.Advice.Weavers.Processes;
using AOP.Core.Contracts;
using AOP.Core.Extensions;
using AOP.Core.Models;
using Fody;
using Mono.Cecil;
using System;

namespace AOP.Core.Advice.Weavers
{
    public class AdviceStateMachineWeaver : AdviceInlineWeaver
    {
        public override byte Priority => 30;

        public AdviceStateMachineWeaver(BaseModuleWeaver weaver) : base(weaver)
        {
        }

        public override bool CanWeave(Injection injection)
        {
            var target = injection.Target as MethodDefinition;
            return injection.Effect is AfterAdviceEffect && target != null && (target.IsAsync() || target.IsIterator());
        }

        protected override void WeaveMethod(MethodDefinition method, Injection injection)
        {
            if (method.IsAsync())
                new AfterAsyncWeaveProcess(_weaver, method, (AfterAdviceEffect)injection.Effect, injection.Source).Execute();

            if (method.IsIterator())
                new AfterIteratorWeaveProcess(_weaver, method, (AfterAdviceEffect)injection.Effect, injection.Source).Execute();
        }
    }
}