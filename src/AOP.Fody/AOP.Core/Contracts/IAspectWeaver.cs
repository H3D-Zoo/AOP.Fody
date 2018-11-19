using AOP.Core.Models;

namespace AOP.Core.Contracts
{
    public interface IAspectWeaver
    {
        void WeaveGlobalAssests(AspectDefinition target);
    }
}