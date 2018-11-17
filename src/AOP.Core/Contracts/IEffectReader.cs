using AOP.Core.Models;
using Mono.Cecil;
using System.Collections.Generic;

namespace AOP.Core.Contracts
{
    public interface IEffectReader
    {
        IReadOnlyCollection<Effect> Read(ICustomAttributeProvider host);
    }
}