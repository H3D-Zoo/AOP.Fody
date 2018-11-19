using System.Collections.Generic;
using AOP.Core.Contracts;
using Mono.Cecil;
using AOP.Core.Advice.Effects;
using System.Linq;
using AOP.Core.Extensions;
using static AOP. Broker.Advice;
using AOP.Core.Models;
using Fody;

namespace AOP.Core.Advice
{
    public class AdviceReader : IEffectReader
    {
        private readonly BaseModuleWeaver _weaver;

        public AdviceReader(BaseModuleWeaver weaver)
        {
            _weaver = weaver;
        }

        public IReadOnlyCollection<Effect> Read(ICustomAttributeProvider host)
        {
            if (host is MethodDefinition source)
                return Extract(source);

            return new List<Effect>();
        }

        private IReadOnlyCollection<AdviceEffectBase> Extract(MethodDefinition method)
        {
            var advices = new List<AdviceEffectBase>();

            foreach (var ca in method.CustomAttributes.ToList())
            {
                if (ca.AttributeType.FullName == WellKnownTypes.Advice)
                {
                    var adviceType = ca.GetConstructorValue<Broker.Advice.Type>(0);
                    var advice = CreateEffect(adviceType);
                    if (advice == null)
                    {
                        _weaver.LogError($"Unknown advice type {adviceType.ToString()}");
                        continue;
                    }

                    advice.Method = method;
                    advice.Target = ca.GetConstructorValue<Target>(1);
                    advice.Arguments = ExtractArguments(method);

                    advices.Add(advice);
                }
            }

            return advices;
        }

        private List<AdviceArgument> ExtractArguments(MethodDefinition method)
        {
            var args = new List<AdviceArgument>();

            foreach (var par in method.Parameters)
            {
                var argAttr = par.CustomAttributes.FirstOrDefault(ca => ca.AttributeType.FullName == WellKnownTypes.Argument);

                if (argAttr == null)
                {
                    _weaver.LogError("Unbound arguments are not supported.");
                    continue;
                }

                args.Add(new AdviceArgument
                {
                    Source = argAttr.GetConstructorValue<Argument.Source>(0),
                    Parameter = par
                });
            }

            return args;
        }

        internal static AdviceEffectBase CreateEffect(Broker.Advice.Type adviceType)
        {
            switch (adviceType)
            {
                case Broker.Advice.Type.After: return new AfterAdviceEffect();
                case Broker.Advice.Type.Before: return new BeforeAdviceEffect();
                case Broker.Advice.Type.Around: return new AroundAdviceEffect();
                default: return null;
            }
        }
    }
}