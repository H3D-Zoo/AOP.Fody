using System.Collections.Generic;
using AOP.Core.Models;
using Mono.Cecil;

namespace AOP.Core.Contracts
{
    public interface IInjectionReader
    {
        IReadOnlyCollection<Injection> ReadAll(ModuleDefinition module);
    }
}