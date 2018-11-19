using AOP.Core.Contracts;
using Mono.Cecil;
using System;
using Fody;
using System.Linq;

namespace AOP.Core.Models
{
    public abstract class Effect : IEquatable<Effect>
    {
        public uint Priority { get; protected set; }

        public bool Equals(Effect other)
        {
            if (other == null)
                return false;

            return IsEqualTo(other);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        protected virtual bool IsEqualTo(Effect other)
        {
            return this == other;
        }

        public abstract bool IsApplicableFor(IMemberDefinition target);

        public abstract bool Validate(AspectDefinition aspect, BaseModuleWeaver weaver);

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}