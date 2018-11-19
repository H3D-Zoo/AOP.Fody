using AOP.Core.Models;

namespace AOP.Core.Contracts
{
    public interface IEffectWeaver
    {
        byte Priority { get; }

        void Weave(Injection injection);

        bool CanWeave(Injection injection);
    }
}