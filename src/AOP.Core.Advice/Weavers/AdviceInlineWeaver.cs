using AOP.Core.Advice.Effects;
using AOP.Core.Advice.Weavers.Processes;
using AOP.Core.Contracts;
using AOP.Core.Extensions;
using AOP.Core.Models;
using Mono.Cecil;
using static AOP.Broker.Advice;
using Fody;

namespace AOP.Core.Advice.Weavers
{
    public class AdviceInlineWeaver : IEffectWeaver
    {
        public virtual byte Priority => 90;

        protected readonly BaseModuleWeaver _weaver;

        public AdviceInlineWeaver(BaseModuleWeaver weaver)
        {
            _weaver = weaver;
        }

        public virtual bool CanWeave(Injection injection)
        {
            var result =
                (injection.Effect is BeforeAdviceEffect || injection.Effect is AfterAdviceEffect) &&
                (injection.Target is EventDefinition || injection.Target is PropertyDefinition || injection.Target is MethodDefinition);

            if (result && injection.Target is MethodDefinition && injection.Effect is AfterAdviceEffect)
            {
                var md = (MethodDefinition)injection.Target;
                if (md.IsAsync() || md.IsIterator())
                    result = false;
            }

            return result;
        }

        public void Weave(Injection injection)
        {
            var effect = (AdviceEffectBase)injection.Effect;

            if (injection.Target is EventDefinition)
            {
                var target = (EventDefinition)injection.Target;

                if (target.AddMethod != null && effect.Target.HasFlag(Target.EventAdd))
                    WeaveMethod(target.AddMethod, injection);

                if (target.RemoveMethod != null && effect.Target.HasFlag(Target.EventRemove))
                    WeaveMethod(target.RemoveMethod, injection);
                return;
            }

            if (injection.Target is PropertyDefinition)
            {
                var target = (PropertyDefinition)injection.Target;

                if (target.SetMethod != null && effect.Target.HasFlag(Target.Setter))
                    WeaveMethod(target.SetMethod, injection);

                if (target.GetMethod != null && effect.Target.HasFlag(Target.Getter))
                    WeaveMethod(target.GetMethod, injection);

                return;
            }

            if (injection.Target is MethodDefinition)
            {
                var target = (MethodDefinition)injection.Target;

                if (target.IsConstructor && effect.Target.HasFlag(Target.Constructor))
                    WeaveMethod(target, injection);

                if (target.IsNormalMethod() && effect.Target.HasFlag(Target.Method))
                    WeaveMethod(target, injection);

                return;
            }

            _weaver.LogError($"Unsupported target {injection.Target.GetType().Name}");
        }

        protected virtual void WeaveMethod(MethodDefinition method, Injection injection)
        {
            if (injection.Effect is AfterAdviceEffect)
            {
                var process = new AdviceAfterProcess(_weaver, method, injection.Source, (AfterAdviceEffect)injection.Effect);
                process.Execute();
            }
            else if (injection.Effect is BeforeAdviceEffect)
            {
                var process = new AdviceBeforeProcess(_weaver, method, injection.Source, (BeforeAdviceEffect)injection.Effect);
                process.Execute();
            }
            else
            {
                throw new System.Exception("Unknown advice type.");
            }
        }
    }
}