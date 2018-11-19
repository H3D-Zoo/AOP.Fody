﻿namespace AOP.Core.Advice
{
    internal static class Constants
    {
        public static readonly string Prefix = Core.Constants.Prefix;
        public static readonly string AfterStateMachineMethodName = $"{Prefix}after_state_machine";
        public static readonly string MovedThis = $"{Prefix}this";
        public static readonly string MovedArgs = $"{Prefix}args";
    }
}