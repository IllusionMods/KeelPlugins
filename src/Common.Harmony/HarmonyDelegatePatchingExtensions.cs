using HarmonyLib;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace KeelPlugins.Harmony
{
    internal static class HarmonyDelegatePatchingExtensions
    {
        private static readonly Dictionary<int, Delegate> DelegateCache = new Dictionary<int, Delegate>();
        private static readonly Dictionary<Delegate, MethodInfo> MethodCache = new Dictionary<Delegate, MethodInfo>();
        private static int delegateCounter;

        private static MethodInfo GetPatchMethod<T>(T action) where T : Delegate
        {
            if(action.Method.IsStatic && action.Target == null) return action.Method;
            if(MethodCache.TryGetValue(action, out var method))
                return method;

            var actionParams = action.Method.GetParameters();

            var dmd = new DynamicMethodDefinition($"ActionWrapper_{action.Method.Name}", action.Method.ReturnType,
                actionParams.Select(p => p.ParameterType).ToArray());

            for(var i = 0; i < dmd.Definition.Parameters.Count; i++)
            {
                var paramInfo = actionParams[i];
                var paramDef = dmd.Definition.Parameters[i];
                paramDef.Name = paramInfo.Name;
            }

            var il = dmd.GetILGenerator();
            var preserveCtx = action.Target != null && action.Target.GetType().GetFields().Any(x => !x.IsStatic);

            if(preserveCtx)
            {
                var currentCounter = delegateCounter++;
                DelegateCache[currentCounter] = action;

                var cacheField = AccessTools.Field(typeof(HarmonyDelegatePatchingExtensions), nameof(DelegateCache));
                var getMethod = AccessTools.Method(typeof(Dictionary<int, Delegate>), "get_Item");

                il.Emit(OpCodes.Ldsfld, cacheField);
                il.Emit(OpCodes.Ldc_I4, currentCounter);
                il.Emit(OpCodes.Callvirt, getMethod);
            }
            else
            {
                if(action.Target == null)
                    il.Emit(OpCodes.Ldnull);
                else
                    il.Emit(OpCodes.Newobj,
                        AccessTools.FirstConstructor(action.Target.GetType(),
                            x => x.GetParameters().Length == 0 && !x.IsStatic));

                il.Emit(OpCodes.Ldftn, action.Method);
                il.Emit(OpCodes.Newobj, AccessTools.Constructor(typeof(T), new[] { typeof(object), typeof(IntPtr) }));
            }

            for(var i = 0; i < actionParams.Length; i++)
                il.Emit(OpCodes.Ldarg_S, (short)i);

            il.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(T), "Invoke"));
            il.Emit(OpCodes.Ret);

            // Generate with cecil to ensure it's usable with Harmony (Harmony blocks DynamicMethods by default)
            var result = DMDGenerator<DMDCecilGenerator>.Generate(dmd);
            MethodCache[action] = result;
            return result;
        }

        /// <summary>
        ///     Begin applying delegate patches
        /// </summary>
        /// <param name="instance">Harmony instance</param>
        /// <param name="original">Method to patch</param>
        /// <returns>Patch composer that allows to apply patches as delegates</returns>
        public static PatchComposer StartPatch(this HarmonyLib.Harmony instance, MethodInfo original)
        {
            return new PatchComposer(instance, original);
        }

        /// <summary>
        ///     A general patch processor that allows to apply patches as delegates
        ///     This is a wrapper around Harmony's PatchProcessor. This means that you can apply
        ///     only one patch of each type. To apply multiple prefixes to the same method,
        ///     initialize multiple instances of this composer.
        /// </summary>
        public class PatchComposer
        {
            private readonly PatchProcessor processor;

            /// <summary>
            ///     Begins patch composing
            /// </summary>
            /// <param name="instance">Harmony instance</param>
            /// <param name="original">Method to patch</param>
            public PatchComposer(HarmonyLib.Harmony instance, MethodInfo original)
            {
                processor = new PatchProcessor(instance, original);
            }

            /// <summary>
            ///     Apply a prefix to the original method
            /// </summary>
            /// <param name="action">Prefix to apply</param>
            /// <typeparam name="T">Prefix delegate type</typeparam>
            /// <returns>Current patch processor</returns>
            public PatchComposer Prefix<T>(T action) where T : Delegate
            {
                processor.AddPrefix(GetPatchMethod(action));
                return this;
            }

            /// <summary>
            ///     Apply a postfix to the original method
            /// </summary>
            /// <param name="action">Postfix to apply</param>
            /// <typeparam name="T">Postfix delegate type</typeparam>
            /// <returns>Current patch processor</returns>
            public PatchComposer Postfix<T>(T action) where T : Delegate
            {
                processor.AddPostfix(GetPatchMethod(action));
                return this;
            }

            /// <summary>
            ///     Apply a transpiler to the original method
            /// </summary>
            /// <param name="action">Transpiler to apply</param>
            /// <typeparam name="T">Transpiler delegate type</typeparam>
            /// <returns>Current patch processor</returns>
            public PatchComposer Transpiler<T>(T action) where T : Delegate
            {
                processor.AddTranspiler(GetPatchMethod(action));
                return this;
            }

            /// <summary>
            ///     Apply a finalizer to the original method
            /// </summary>
            /// <param name="action">Finalizer to apply</param>
            /// <typeparam name="T">Finalizer delegate type</typeparam>
            /// <returns>Current patch processor</returns>
            public PatchComposer Finalizer<T>(T action) where T : Delegate
            {
                processor.AddFinalizer(GetPatchMethod(action));
                return this;
            }

            /// <summary>
            ///     Applies patches to the current method
            /// </summary>
            public void Apply()
            {
                processor.Patch();
            }
        }
    }
}
