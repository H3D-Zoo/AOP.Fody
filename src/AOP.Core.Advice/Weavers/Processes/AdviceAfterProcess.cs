using AOP.Core.Advice.Effects;
using AOP.Core.Contracts;
using AOP.Core.Extensions;
using AOP.Core.Fluent;
using AOP.Core.Models;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System;
using System.Collections.Generic;
using Fody;

namespace AOP.Core.Advice.Weavers.Processes
{
    internal class AdviceAfterProcess : AdviceWeaveProcessBase<AfterAdviceEffect>
    {
        private static readonly Dictionary<MethodBody, VariableDefinition> _returnVariablesCache = new Dictionary<MethodBody, VariableDefinition>();

        private VariableDefinition _retvar;

        public AdviceAfterProcess(BaseModuleWeaver weaver, MethodDefinition target, AspectDefinition aspect, AfterAdviceEffect effect)
            : base(weaver, target, effect, aspect)
        {
            if (_target.ReturnType.FullName != WellKnownTypes.Void && effect.Arguments.Any(a => a.Source == Broker.Advice.Argument.Source.ReturnValue))
                _retvar = GetOrCreateRetVar();
        }

        private VariableDefinition GetOrCreateRetVar()
        {
            if (!_returnVariablesCache.TryGetValue(_target.Body, out var ret))
            {
                ret = new VariableDefinition(_target.ReturnType);
                _target.Body.Variables.Add(ret);
                _target.Body.InitLocals = true;

                _returnVariablesCache.Add(_target.Body, ret);
            }

            return ret;
        }

        public override void Execute()
        {
            _target.GetEditor().OnExit(
                il =>
                {
                    if (_retvar != null)
                        il.Store(_retvar);

                    il.LoadAspect(_aspect);
                    il.Call(_effect.Method, LoadAdviceArgs);

                    if (_retvar != null)
                        il.Load(_retvar);
                });
        }

        protected override void LoadReturnValueArgument(PointCut pc, AdviceArgument parameter)
        {
            if (_retvar == null)
                pc.Null();
            else
                pc.Load(_retvar).ByVal(_retvar.VariableType);
        }
    }
}