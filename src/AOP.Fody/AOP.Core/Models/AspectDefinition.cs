using AOP.Core.Contracts;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using Fody;
using static AOP.Broker.Aspect;

namespace AOP.Core.Models
{
    public class AspectDefinition
    {
        private MethodDefinition _factoryMethod;

        public TypeDefinition Host { get; set; }

        public List<Effect> Effects { get; set; }

        public Scope Scope { get; set; }

        public TypeReference Factory { get; set; }

        private MethodReference GetFactoryMethod()
        {
            if (_factoryMethod == null)
            {
                if (Factory != null)
                {
                    _factoryMethod = Factory.Resolve().Methods.FirstOrDefault(m =>
                    m.IsStatic && m.IsPublic
                    && m.Name == Constants.AspectFactoryMethodName
                    && m.ReturnType.FullName == WellKnownTypes.Object
                    && m.Parameters.Count == 1 && m.Parameters[0].ParameterType.FullName == WellKnownTypes.Type
                    );
                }
                else
                    _factoryMethod = Host.Methods.FirstOrDefault(m => m.IsConstructor && !m.IsStatic && m.IsPublic && !m.HasParameters);
            }

            return _factoryMethod;
        }

        public void CreateAspectInstance(PointCut c)
        {
            c = c.Call(GetFactoryMethod(), arg =>
            {
                if (Factory != null)
                    arg.TypeOf(Host);
            });

            if (Factory != null)
                c.Cast(Host);
        }

        public bool Validate(BaseModuleWeaver weaver)
        {
            if (!Effects.Any())
                weaver.LogWarning($"Type {Host.FullName} has defined as an aspect, but lacks any effect.");

            if (Host.HasGenericParameters)
            {
                weaver.LogError($"Aspect {Host.FullName} should not have generic parameters.");
                return false;
            }

            if (Host.IsAbstract)
            {
                weaver.LogError($"Aspect {Host.FullName} cannot be static nor abstract.");
                return false;
            }

            if (GetFactoryMethod() == null)
            {
                if (Factory != null)
                    weaver.LogError($"Type {Factory.FullName} should have 'public static object GetInstance(Type)' method in order to be aspect factory.");
                else
                    weaver.LogError($"Aspect {Host.FullName} has no parameterless public constructor nor valid factory.");
                return false;
            }

            return Effects.All(e => e.Validate(this, weaver));
        }
    }
}