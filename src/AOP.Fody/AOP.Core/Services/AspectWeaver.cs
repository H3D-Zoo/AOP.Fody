using AOP.Core.Contracts;
using AOP.Core.Fluent;
using AOP.Core.Models;
using Mono.Cecil;
using System.Linq;
using Fody;

namespace AOP.Core.Services
{
    public class AspectWeaver : IAspectWeaver
    {
        private readonly BaseModuleWeaver _weaver;

        public AspectWeaver(BaseModuleWeaver weaver)
        {
            _weaver = weaver;
        }

        public void WeaveGlobalAssests(AspectDefinition target)
        {
            EnsureSingletonField(target);
        }

        private void EnsureSingletonField(AspectDefinition aspect)
        {
            var ts = aspect.Host.Module.GetTypeSystem();

            var singletonField = aspect.Host.Fields.FirstOrDefault(m => m.Name == Constants.AspectGlobalField);

            if (singletonField == null)
            {
                singletonField = new FieldDefinition(Constants.AspectGlobalField, FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly, ts.Import(aspect.Host));
                aspect.Host.Fields.Add(singletonField);

                var cctor = aspect.Host.Methods.FirstOrDefault(c => c.IsConstructor && c.IsStatic);

                if (cctor == null)
                {
                    cctor = new MethodDefinition(".cctor",
                        MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                            ts.Void);

                    aspect.Host.Methods.Add(cctor);

                    cctor.GetEditor().Instead(i => i.Return());
                }

                cctor.GetEditor().OnInit(i => i.Store(singletonField, aspect.CreateAspectInstance));
            }

            aspect.Host.IsBeforeFieldInit = false;
        }
    }
}