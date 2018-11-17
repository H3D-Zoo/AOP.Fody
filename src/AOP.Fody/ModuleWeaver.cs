using System;
using System.Collections.Generic;
using Fody;
using DryIoc;
using AOP.Core;
using AOP.Core.Contracts;
using AOP.Core.Services;
using AOP.Core.Advice.Weavers;
using AOP.Core.Advice;
using AOP.Core.Mixin;
using System.IO;

namespace AOP.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        public override void Execute()
        {
            var container = new Container();

            //register main services

            container.Register<AopModuleWeaver>(Reuse.Singleton);
            container.Register<IAspectReader, AspectReader>(Reuse.Singleton);
            container.Register<IAspectWeaver, AspectWeaver>(Reuse.Singleton);
            container.Register<IInjectionReader, InjectionReader>(Reuse.Singleton);
            container.UseInstance<BaseModuleWeaver>(this, true);

            //register effects

            container.Register<IEffectReader, MixinReader>();
            container.Register<IEffectReader, AdviceReader>();

            container.Register<IEffectWeaver, MixinWeaver>();
            container.Register<IEffectWeaver, AdviceInlineWeaver>();
            container.Register<IEffectWeaver, AdviceAroundWeaver>();
            container.Register<IEffectWeaver, AdviceStateMachineWeaver>();

            container.Resolve<AopModuleWeaver>().Execute(ModuleDefinition);
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "mscorlib";
            yield return "netstandard";
        }

        public override bool ShouldCleanReference => true;
    }
}
