﻿using System.Runtime.CompilerServices;


namespace AOP.Core
{
    public static class WellKnownTypes
    {
        public static readonly string Type = typeof(System.Type).FullName;
        public static readonly string Object = typeof(object).FullName;
        public static readonly string Void = typeof(void).FullName;

        public static readonly string IteratorStateMachineAttribute = typeof(IteratorStateMachineAttribute).FullName;
        public static readonly string AsyncStateMachineAttribute = typeof(AsyncStateMachineAttribute).FullName;

        public static readonly string Inject = typeof(Broker.Inject).FullName;
        public static readonly string Aspect = typeof(Broker.Aspect).FullName;
        public static readonly string Mixin = typeof(Broker.Mixin).FullName;
        public static readonly string Advice = typeof(Broker.Advice).FullName;
        public static readonly string Argument = Adopt(typeof(Broker.Advice.Argument).FullName);

        //mono&clr type fullname is inconsistent
        private static string Adopt(string clr)
        {
            return clr.Replace('+', '/');
        }
    }
}
