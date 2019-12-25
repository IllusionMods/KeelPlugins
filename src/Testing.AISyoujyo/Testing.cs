using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin("keelhauled.testing", "TESTINGPLUGIN", "1.0.0")]
    public class Testing : BaseUnityPlugin
    {
        private Harmony harmony;

        private void Awake()
        {
            harmony = HarmonyWrapper.PatchAll(GetType());
        }

        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
    }
}
