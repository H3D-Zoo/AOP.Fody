using System;

namespace AOP.Broker
{
    /// <summary>
    /// Marks member to be injection target for specific Aspect.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = true)]
    public class Inject : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Inject" /> class.
        /// </summary>
        /// <param name="aspect">Aspect to inject.</param>
        public Inject(Type aspect)
        {
        }
    }
}
