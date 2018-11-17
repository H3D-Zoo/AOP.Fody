using AOP.Core.Advice.Effects;
using AOP.Core.Fluent;
using AOP.Core.Models;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AOP.Core.Fluent.Models;
using static AOP.Broker.Advice.Argument;
using AOP.Core.Contracts;
using AOP.Core.Extensions;
using Fody;

namespace AOP.Core.Advice.Weavers.Processes
{
    internal abstract class AdviceWeaveProcessBase<TEffect>
        where TEffect : AdviceEffectBase
    {
        protected readonly MethodDefinition _target;
        protected readonly TEffect _effect;
        protected readonly ExtendedTypeSystem _ts;
        protected readonly BaseModuleWeaver _weaver;
        protected readonly AspectDefinition _aspect;

        public AdviceWeaveProcessBase(BaseModuleWeaver weaver, MethodDefinition target, TEffect effect, AspectDefinition aspect)
        {
            _weaver = weaver;
            _target = target;
            _effect = effect;
            _aspect = aspect;

            _ts = target.Module.GetTypeSystem();
        }

        public abstract void Execute();

        protected void LoadAdviceArgs(PointCut pc)
        {
            foreach (var arg in _effect.Arguments.OrderBy(a => a.Parameter.Index))
            {
                switch (arg.Source)
                {
                    case Source.Arguments: LoadArgumentsArgument(pc, arg); break;
                    //case Source.Attributes: LoadAttributesArgument(pc, arg); break;
                    case Source.Instance: LoadInstanceArgument(pc, arg); break;
                    case Source.Method: LoadMethodArgument(pc, arg); break;
                    case Source.Name: LoadNameArgument(pc, arg); break;
                    case Source.ReturnType: LoadReturnTypeArgument(pc, arg); break;
                    case Source.ReturnValue: LoadReturnValueArgument(pc, arg); break;
                    case Source.Target: LoadTargetArgument(pc, arg); break;
                    case Source.Type: LoadTypeArgument(pc, arg); break;
                    default: _weaver.LogError($"Unknown argument source {arg.Source.ToString()}"); break;
                }
            }
        }

        protected virtual void LoadTypeArgument(PointCut pc, AdviceArgument parameter)
        {
            pc.TypeOf(_target.DeclaringType);
        }

        protected virtual void LoadTargetArgument(PointCut pc, AdviceArgument parameter)
        {
            _weaver.LogWarning($"Advice {_effect.Type.ToString()} does not support {parameter.Source.ToString()} argument and will always return null");
            pc.Null();
        }

        protected virtual void LoadReturnValueArgument(PointCut pc, AdviceArgument parameter)
        {
            _weaver.LogWarning($"Advice {_effect.Type.ToString()} does not support {parameter.Source.ToString()} argument and will always return null");
            pc.Null();
        }

        protected virtual void LoadReturnTypeArgument(PointCut pc, AdviceArgument parameter)
        {
            pc.TypeOf(_target.ReturnType);
        }

        protected virtual void LoadMethodArgument(PointCut pc, AdviceArgument parameter)
        {
            pc.MethodOf(_target).Cast(_ts.MethodBase);
        }

        protected virtual void LoadInstanceArgument(PointCut pc, AdviceArgument parameter)
        {
            if (_target.IsStatic)
                pc.Null();
            else
                pc.This();
        }

        protected virtual void LoadAttributesArgument(PointCut pc, AdviceArgument parameter)
        {
            pc.Null();
        }

        protected virtual void LoadArgumentsArgument(PointCut pc, AdviceArgument parameter)
        {
            var elements = _target.Parameters.Select<ParameterDefinition, Action<PointCut>>(p => il =>
                il.Load(p).ByVal(p.ParameterType)
            ).ToArray();

            pc.CreateArray<object>(elements);
        }

        protected virtual void LoadNameArgument(PointCut pc, AdviceArgument parameter)
        {
            pc.Value(_target.Name);
        }
    }
}