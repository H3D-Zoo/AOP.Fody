using Mono.Cecil;
using static AOP.Broker.Advice;

namespace AOP.Core.Advice.Effects
{
    internal class AdviceArgument
    {
        public Argument.Source Source { get; set; }

        public ParameterDefinition Parameter { get; set; }
    }
}