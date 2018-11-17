using System.Collections.Generic;
using AOP.Core.Models;
using Mono.Cecil;

namespace AOP.Core.Contracts
{
    public interface IAspectReader
    {
        IReadOnlyCollection<AspectDefinition> ReadAll(ModuleDefinition module);
        AspectDefinition Read(TypeDefinition type);
    }
}