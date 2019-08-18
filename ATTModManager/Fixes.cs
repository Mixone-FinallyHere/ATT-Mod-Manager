﻿using System;
using System.Reflection;
using Harmony12;

namespace ATTTModManagerNet
{
    static class Fixes
    {
        public static void Apply()
        {
            if (Environment.Version < new Version(4, 0))
            {
                var harmony = HarmonyInstance.Create(nameof(ATTTModManager));
                var original = typeof(Assembly).GetMethod(nameof(Assembly.GetTypes), BindingFlags.Instance | BindingFlags.Public, null, new Type[0], new ParameterModifier[0]);
                var prefix = typeof(Fixes).GetMethod(nameof(Prefix_GetTypes), BindingFlags.Static | BindingFlags.NonPublic);
                harmony.Patch(original, new HarmonyMethod(prefix));
                //ATTTModManager.Logger.Log("");
            }
        }

        static bool Prefix_GetTypes(Assembly __instance, ref Type[] __result)
        {
            if (__instance.FullName.StartsWith("ATTTModManager"))
            {
                __result = new Type[0];
                return false;
            }
            return true;
        }
    }
}
