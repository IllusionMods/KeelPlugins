using BepInEx;
using HarmonyLib;
using KeelPlugins.Koikatu;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(PosePng.Koikatu.PosePng.Version)]

namespace PosePng.Koikatu
{
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, "PosePng", Version)]
    public class PosePng : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.posepng";
        public const string Version = "1.0.1." + BuildNumber.Version;

        private const string PngExt = ".png";

        private void Awake()
        {
            Log.SetLogSource(Logger);
            Harmony.CreateAndPatchAll(typeof(Hooks));
        }

        /// <summary>
        /// Save as .png with attached data instead of .dat
        /// </summary>
        private class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(PauseCtrl), nameof(PauseCtrl.Save))]
            public static bool PoseSavePatch(OCIChar _ociChar, ref string _name)
            {
                var filename = $"{DateTime.Now:yyyy_MMdd_HHmm_ss_fff}{PngExt}";
                var path = Path.Combine(UserData.Create("studio/pose"), filename);
                var fileInfo = new PauseCtrl.FileInfo(_ociChar);

                try
                {
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    using (var binaryWriter = new BinaryWriter(fileStream))
                    {
                        var buffer = Studio.Studio.Instance.gameScreenShot.CreatePngScreen(320, 180);
                        binaryWriter.Write(buffer);
                        binaryWriter.Write(PauseCtrl.saveIdentifyingCode);
                        binaryWriter.Write(PauseCtrl.saveVersion);
                        binaryWriter.Write(_ociChar.oiCharInfo.sex);
                        binaryWriter.Write(_name);
                        fileInfo.Save(binaryWriter);
                    }
                }
                catch (Exception ex)
                {
                    Log.Message("Save path has not been set properly");
                    Log.Error(ex);
                }

                return false;
            }

            /// <summary>
            /// Add the .png poses after the vanilla code runs. This should be a transpiler so they all get done together.
            /// </summary>
            [HarmonyPostfix, HarmonyPatch(typeof(PauseRegistrationList), nameof(PauseRegistrationList.InitList))]
            private static void PauseRegistrationList_InitList(PauseRegistrationList __instance)
            {
                int sex = __instance.m_OCIChar.oiCharInfo.sex;
                List<string> additionalPoses = Directory.GetFiles(UserData.Create("studio/pose"), "*.png").ToList();

                for (int j = 0; j < additionalPoses.Count; j++)
                {
                    GameObject gameObject = Instantiate(__instance.prefabNode);
                    gameObject.transform.SetParent(__instance.transformRoot, false);
                    StudioNode component = gameObject.GetComponent<StudioNode>();
                    component.active = true;
                    int no = __instance.listPath.Count;
                    component.addOnClick = delegate
                    {
                        __instance.OnClickSelect(no);
                    };
                    component.text = PauseCtrl.LoadName(additionalPoses[j]);
                    __instance.dicNode.Add(__instance.listPath.Count, component);

                    __instance.listPath.Add(additionalPoses[j]);
                }
            }

            /// <summary>
            /// Patch with added PngFile.SkipPng
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(PauseCtrl), nameof(PauseCtrl.Load))]
            public static bool PoseLoadPatch(OCIChar _ociChar, ref string _path, ref bool __result)
            {
                if (Path.GetExtension(_path).ToLower() == PngExt)
                {
                    var fileInfo = new PauseCtrl.FileInfo();
                    using (var fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        PngFile.SkipPng(binaryReader);

                        if (string.CompareOrdinal(binaryReader.ReadString(), PauseCtrl.saveIdentifyingCode) != 0)
                        {
                            __result = false;
                            return false;
                        }

                        int ver = binaryReader.ReadInt32();
                        binaryReader.ReadInt32();
                        binaryReader.ReadString();
                        fileInfo.Load(binaryReader, ver);
                    }

                    fileInfo.Apply(_ociChar);
                    __result = true;
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Patch with added PngFile.SkipPng
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(PauseCtrl), nameof(PauseCtrl.CheckIdentifyingCode))]
            public static bool PauseCtrl_CheckIdentifyingCode(string _path, ref bool __result)
            {
                if (Path.GetExtension(_path).ToLower() == PngExt)
                {
                    using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(input))
                        {
                            PngFile.SkipPng(binaryReader);
                            if (string.Compare(binaryReader.ReadString(), "【pose】") != 0)
                            {
                                __result = false;
                            }
                        }
                    }
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Patch with added PngFile.SkipPng
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(PauseCtrl), nameof(PauseCtrl.LoadName))]
            public static bool PauseCtrl_LoadName(string _path, ref string __result)
            {
                if (Path.GetExtension(_path).ToLower() == PngExt)
                {
                    using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(input))
                        {
                            PngFile.SkipPng(binaryReader);
                            if (string.Compare(binaryReader.ReadString(), "【pose】") != 0)
                            {
                                __result = string.Empty;
                            }
                            binaryReader.ReadInt32();
                            binaryReader.ReadInt32();
                            __result = binaryReader.ReadString();
                        }
                    }
                    return false;
                }
                return true;
            }
        }
    }
}
