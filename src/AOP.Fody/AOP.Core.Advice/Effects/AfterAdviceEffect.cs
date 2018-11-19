using System;
using AOP.Broker;
using Mono.Cecil;

namespace AOP.Core.Advice.Effects
{
    internal class AfterAdviceEffect : AdviceEffectBase
    {
        public override Broker.Advice.Type Type => Broker.Advice.Type.After;

        public override bool IsApplicableFor(IMemberDefinition target)
        {
            // check args

            return base.IsApplicableFor(target);
        }
    }
}