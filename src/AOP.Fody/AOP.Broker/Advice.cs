﻿using System;

namespace AOP.Broker
{
    /// <summary>
    /// Defines method as a special advice method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class Advice : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Advice" /> class.
        /// </summary>
        /// <param name="type">Specifies when advice method should be called.</param>
        /// <param name="targets">Specifies what target members this method is injected to.</param>
        public Advice(Type type, Target targets)
        {
        }

        /// <summary>
        /// Advice method injection points enumeration.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Advice method is called before target method.
            /// </summary>
            Before = 1,

            /// <summary>
            /// Advice method is called after target method.
            /// </summary>
            After = 2,

            /// <summary>
            /// Advice method is called instead of target method. Consider using <see cref="Argument.Source.Target" /> and <see cref="Argument.Source.Arguments" /> in order to make a subsequent call to target method.
            /// </summary>
            Around = 4
        }

        /// <summary>
        /// Advice method injection targets enumeration.
        /// </summary>
        [Flags]
        public enum Target
        {
            /// <summary>
            /// Advice method is injected into target's constructors.
            /// </summary>
            Constructor = 1,

            /// <summary>
            /// Advice method is injected into target's methods.
            /// </summary>
            Method = 2,

            /// <summary>
            /// Advice method is injected into target's property getters.
            /// </summary>
            Getter = 4,

            /// <summary>
            /// Advice method is injected into target's property setters.
            /// </summary>
            Setter = 8,

            /// <summary>
            /// Advice method is injected into target's event subscribe handlers.
            /// </summary>
            EventAdd = 16,

            /// <summary>
            /// Advice method is injected into target's event unsubscribe handlers.
            /// </summary>
            EventRemove = 32
        }

        /// <summary>
        /// Defines method parameter as a special advice parameter.
        /// </summary>
        [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
        public sealed class Argument : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Argument" /> class.
            /// </summary>
            /// <param name="source">Specifies source of advice argument.</param>
            public Argument(Source source)
            {
            }

            /// <summary>
            /// Advice argument sources enumeration.
            /// </summary>
            public enum Source
            {
                /// <summary>
                /// Target's instance or <c>null</c> if target is static.
                /// Should be of type <see cref="object" />.
                /// </summary>
                Instance = 1,

                /// <summary>
                /// Target's class type.
                /// Should be of type <see cref="System.Type" />.
                /// </summary>
                Type = 2,

                /// <summary>
                /// Target method.
                /// Should be of type <see cref="System.Reflection.MethodBase" />.
                /// </summary>
                Method = 3,

                /// <summary>
                /// Target method delegate. Usage <example>Target(<see cref="Arguments"/>)</example> for chaining methods.
                /// Should be of type <c>Func&lt;object[],object&gt;</c>.
                /// Works only with <see cref="Advice.Type.Around" />.
                /// </summary>
                Target = 4,

                /// <summary>
                /// Target name.
                /// Should be of type <see cref="string" />.
                /// </summary>
                Name = 5,

                /// <summary>
                /// Target method arguments.
                /// Should be of type <c>object[]</c>.
                /// </summary>
                Arguments = 6,

                /// <summary>
                /// Target method result.
                /// Should be of type <see cref="object" />.
                /// </summary>
                ReturnValue = 7,

                /// <summary>
                /// Target method result type.
                /// Should be of type <see cref="System.Type" />.
                /// </summary>
                ReturnType = 8,

                ///// <summary>
                ///// Set of Attributes that target was tagged with. Methods inherit type's attributes. />.
                ///// Should be of type <c>Attribute[]</c> or derived.
                ///// </summary>
                //Attributes = 9
            }
        }
    }
}
