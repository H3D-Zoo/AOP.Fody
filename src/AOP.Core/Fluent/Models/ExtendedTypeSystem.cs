﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AOP.Core.Fluent.Models
{
    public class ExtendedTypeSystem
    {
        #region Private Fields

        private readonly ModuleDefinition _module;

        #endregion Private Fields

        #region Public Constructors

        public ExtendedTypeSystem(ModuleDefinition md)
        {
            _module = md;

            Boolean = md.TypeSystem.Boolean;
            Byte = md.TypeSystem.Byte;
            Char = md.TypeSystem.Char;
            Double = md.TypeSystem.Double;
            Int16 = md.TypeSystem.Int16;
            Int32 = md.TypeSystem.Int32;
            Int64 = md.TypeSystem.Int64;
            IntPtr = md.TypeSystem.IntPtr;
            Object = md.TypeSystem.Object;
            SByte = md.TypeSystem.SByte;
            Single = md.TypeSystem.Single;
            String = md.TypeSystem.String;
            TypedReference = md.TypeSystem.TypedReference;
            UInt16 = md.TypeSystem.UInt16;
            UInt32 = md.TypeSystem.UInt32;
            UInt64 = md.TypeSystem.UInt64;
            UIntPtr = md.TypeSystem.UIntPtr;
            Void = md.TypeSystem.Void;

            ObjectArray = MakeArrayType(Object);

            Task = md.ImportReference(typeof(Task));
            TaskGeneric = md.ImportReference(typeof(Task<>));
            TaskCompletionGeneric = md.ImportReference(typeof(TaskCompletionSource<>));
            ActionGeneric = md.ImportReference(typeof(Action<>));
            FuncGeneric = md.ImportReference(typeof(Func<>));
            FuncGeneric2 = md.ImportReference(typeof(Func<,>));

            MethodBase = md.ImportReference(typeof(MethodBase));
            Type = md.ImportReference(typeof(Type));

            DebuggerHiddenAttribute = md.ImportReference(typeof(DebuggerHiddenAttribute));
            DebuggerStepThroughAttribute = md.ImportReference(typeof(DebuggerStepThroughAttribute));
            CompilerGeneratedAttribute = md.ImportReference(typeof(CompilerGeneratedAttribute));

            LoadIndirectMap = new Dictionary<TypeReference, OpCode>
            {
                { UIntPtr, OpCodes.Ldind_I },
                { IntPtr, OpCodes.Ldind_I },
                { SByte, OpCodes.Ldind_I1 },
                { Int16, OpCodes.Ldind_I2 },
                { Int32, OpCodes.Ldind_I4 },
                { Int64, OpCodes.Ldind_I8 },
                { UInt64, OpCodes.Ldind_I8 },

                { Boolean, OpCodes.Ldind_U1 },
                { Byte, OpCodes.Ldind_U1 },
                { Char, OpCodes.Ldind_U2 },
                { UInt16, OpCodes.Ldind_U2 },
                { UInt32, OpCodes.Ldind_U4 },

                { Single, OpCodes.Ldind_R4 },
                { Double, OpCodes.Ldind_R8 },
            };

            SaveIndirectMap = new Dictionary<TypeReference, OpCode>
            {
                { UIntPtr, OpCodes.Stind_I },
                { IntPtr, OpCodes.Stind_I },
                { SByte, OpCodes.Stind_I1 },
                { Int16, OpCodes.Stind_I2 },
                { Int32, OpCodes.Stind_I4 },
                { Int64, OpCodes.Stind_I8 },
                { UInt64, OpCodes.Stind_I8 },

                { Boolean, OpCodes.Stind_I1 },
                { Byte, OpCodes.Stind_I1 },
                { Char, OpCodes.Stind_I2 },
                { UInt16, OpCodes.Stind_I2 },
                { UInt32, OpCodes.Stind_I4 },

                { Single, OpCodes.Stind_R4 },
                { Double, OpCodes.Stind_R8 },
            };
        }

        internal FieldReference Import(FieldReference field)
        {
            //IGenericParameterProvider context = null;
            //if (field.DeclaringType.IsGenericParameter)
            //    context = ((GenericParameter)field.DeclaringType).Owner;

            return _module.ImportReference(field/*, context*/);
        }

        public ModuleDefinition GetModule()
        {
            return _module;
        }

        public TypeReference Import(TypeReference type, IGenericParameterProvider genericContext)
        {
            return _module.ImportReference(type, genericContext);
        }

        public TypeReference Import(TypeReference type)
        {
            IGenericParameterProvider context = null;
            //if (type.IsGenericParameter)
            //    context = ((GenericParameter)type).Owner;

            return _module.ImportReference(type, context);
        }

        public TypeReference Import(Type type)
        {
            IGenericParameterProvider context = null;
            //if (type.IsGenericParameter)
            //    context = ((GenericParameter)type).Owner;

            return _module.ImportReference(type, context);
        }

        public MethodReference Import(MethodReference method)
        {
            return _module.ImportReference(method);
        }

        #endregion Public Constructors

        #region Public Properties

        public TypeReference ActionGeneric { get; private set; }

        public TypeReference Boolean { get; private set; }

        public TypeReference Byte { get; private set; }

        public TypeReference Char { get; private set; }

        public TypeReference CompilerGeneratedAttribute { get; private set; }

        public TypeReference DebuggerHiddenAttribute { get; internal set; }

        public TypeReference DebuggerStepThroughAttribute { get; private set; }

        public TypeReference Double { get; private set; }

        public TypeReference FuncGeneric { get; private set; }

        public TypeReference FuncGeneric2 { get; private set; }

        public TypeReference Int16 { get; private set; }

        public TypeReference Int32 { get; private set; }

        public TypeReference Int64 { get; private set; }

        public TypeReference IntPtr { get; private set; }

        public IReadOnlyDictionary<TypeReference, OpCode> LoadIndirectMap { get; private set; }

        public TypeReference MethodBase { get; private set; }

        public TypeReference Object { get; private set; }

        public TypeReference ObjectArray { get; private set; }

        public Dictionary<TypeReference, OpCode> SaveIndirectMap { get; private set; }

        public TypeReference SByte { get; private set; }

        public TypeReference Single { get; private set; }

        public TypeReference String { get; private set; }

        public TypeReference Task { get; private set; }

        public TypeReference TaskCompletionGeneric { get; private set; }

        public TypeReference TaskGeneric { get; private set; }

        public TypeReference Type { get; private set; }

        public TypeReference TypedReference { get; private set; }

        public TypeReference UInt16 { get; private set; }

        public TypeReference UInt32 { get; private set; }

        public TypeReference UInt64 { get; private set; }

        public TypeReference UIntPtr { get; private set; }

        public TypeReference Void { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public TypeReference MakeArrayType(TypeReference argument)
        {
            return Import(new ArrayType(Import(argument)));
        }

        public TypeReference MakeGenericInstanceType(TypeReference openGenericType, params TypeReference[] arguments)
        {
            if (openGenericType.GenericParameters.Count != arguments.Length)
                throw new ArgumentException("Generic arguments number mismatch", "arguments");

            var instance = new GenericInstanceType(openGenericType);
            foreach (var argument in arguments)
                instance.GenericArguments.Add(argument);

            return Import(instance);
        }

        #endregion Public Methods
    }
}