using AOP.Core.Contracts;
using System.Collections.Generic;
using Fody;
using AOP.Core.Fluent;
using System.IO;
using Mono.Cecil;
using System.Linq;
using AOP.Core.Models;

namespace AOP.Core
{
    public class AopModuleWeaver
    {
        private readonly IAspectReader _aspectExtractor;
        private readonly IAspectWeaver _aspectWeaver;
        private readonly IEnumerable<IEffectWeaver> _effectWeavers;
        private readonly IInjectionReader _injectionCollector;
        private readonly BaseModuleWeaver _weaver;

        public AopModuleWeaver(
            IAspectReader aspectExtractor,
            IInjectionReader injectionCollector,
            IAspectWeaver aspectWeaver,
            IEnumerable<IEffectWeaver> effectWeavers,
            BaseModuleWeaver weaver)
        {
            _aspectExtractor = aspectExtractor;
            _injectionCollector = injectionCollector;
            _aspectWeaver = aspectWeaver;
            _effectWeavers = effectWeavers;
            _weaver = weaver;
        }

        public void Execute(ModuleDefinition module, bool optimize = false)
        {
            _weaver.LogInfo($"AopModuleWeaver execute for {module}");
            ProcessModule(module, optimize);
        }

        public void ProcessModule(ModuleDefinition module, bool optimize)
        {
            var aspects = _aspectExtractor.ReadAll(module);
            _weaver.LogInfo($"Found {aspects.Count} aspects");

            var injections = _injectionCollector.ReadAll(module).ToList();
            _weaver.LogInfo($"Found {injections.Count} injections");

            if (aspects.Count != 0)
            {
                _weaver.LogInfo($"Processing aspects...");
                foreach (var aspect in aspects)
                    _aspectWeaver.WeaveGlobalAssests(aspect);
            }

            if (injections.Count != 0)
            {
                _weaver.LogInfo($"Processing injections...");

                foreach (var injector in _effectWeavers.OrderByDescending(i => i.Priority))
                {
                    _weaver.LogInfo($"Executing {injector.GetType().Name}...");

                    foreach (var prioritizedInjections in injections.GroupBy(i => i.Priority).OrderByDescending(a => a.Key).ToList())
                        foreach (var injection in prioritizedInjections.OrderByDescending(i => i.Effect.Priority))
                            if (injector.CanWeave(injection))
                            {
                                injector.Weave(injection);
                                injections.Remove(injection);
                            }
                }

                foreach (var injection in injections)
                    _weaver.LogError($"Couldn't find weaver for {injection}");
            }

            if (optimize)
                _weaver.LogInfo($"Cleanup and optimize...");
            else
                _weaver.LogInfo($"Cleanup...");


            if (optimize)
                EditorFactory.Optimize(module);

            EditorFactory.CleanUp(module);
        }

        private List<Injection> ExcludeAspectInjections(IEnumerable<Injection> injections, IEnumerable<AspectDefinition> aspects)
        {
            var aspectTypes = new HashSet<TypeDefinition>(aspects.Select(a => a.Host));
            return injections.Where(i => !aspectTypes.Contains(i.Target.DeclaringType)).ToList();
        }
    }
}
