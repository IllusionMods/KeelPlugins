using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Studio;
using System;
using System.IO;

namespace KeelPlugins
{
    [BepInProcess(KoikatuConstants.StudioProcessName)]
    [BepInPlugin(GUID, "PosePng", Version)]
    public class PosePng : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.posepng";
        public const string Version = "1.0.0." + BuildNumber.Version;
        internal static new ManualLogSource Logger;

        private static ConfigEntry<string> SaveFolder { get; set; }

        private const string PngExt = ".png";
        private Harmony harmony;

        private void Awake()
        {
            Logger = base.Logger;
            SaveFolder = Config.Bind("", "Save folder path", "");
            harmony = Harmony.CreateAndPatchAll(typeof(Hooks));
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
#endif

        private class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(PauseCtrl), nameof(PauseCtrl.Save))]
            public static bool PoseSavePatch(OCIChar _ociChar, ref string _name)
            {
                var filename = $"{_name}_{DateTime.Now:yyyy_MMdd_HHmm_ss_fff}{PngExt}";
                var path = Path.Combine(SaveFolder.Value, filename);
                var fileInfo = new PauseCtrl.FileInfo(_ociChar);

                try
                {
                    using(var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    using(var binaryWriter = new BinaryWriter(fileStream))
                    {
                        var buffer = Studio.Studio.Instance.gameScreenShot.CreatePngScreen(320, 180, false, false);
                        binaryWriter.Write(buffer);
                        binaryWriter.Write(PauseCtrl.saveIdentifyingCode);
                        binaryWriter.Write(PauseCtrl.saveVersion);
                        binaryWriter.Write(_ociChar.oiCharInfo.sex);
                        binaryWriter.Write(_name);
                        fileInfo.Save(binaryWriter);
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogMessage("Save path has not been set properly");
                    Logger.LogError(ex);
                }

                return false;
            }

            [HarmonyPrefix, HarmonyPatch(typeof(PauseCtrl), nameof(PauseCtrl.Load))]
            public static bool PoseLoadPatch(OCIChar _ociChar, ref string _path, ref bool __result)
            {
                if(Path.GetExtension(_path).ToLower() == PngExt)
                {
                    var fileInfo = new PauseCtrl.FileInfo(null);
                    using(var fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using(var binaryReader = new BinaryReader(fileStream))
                    {
                        PngFile.SkipPng(binaryReader);

                        if(string.Compare(binaryReader.ReadString(), PauseCtrl.saveIdentifyingCode) != 0)
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
        }
    }
}
