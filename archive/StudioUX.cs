using BepInEx;
using BepInEx.Harmony;
using BepInEx.Logging;
using HarmonyLib;
using Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Vectrosity;
using SharedPluginCode;

namespace StudioUX
{
    [BepInProcess(KoikatuConstants.KoikatuStudioProcessName)]
    [BepInPlugin("keelhauled.studiouxexperiment", "StudioUX", "1.0.0")]
    public class StudioUX : BaseUnityPlugin
    {
        new static ManualLogSource Logger;

        void Start()
        {
            Logger = base.Logger;
            StartCoroutine(EndOfFrame());

            var characters = GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null);
            foreach(var chara in characters)
            {
                var skins = new List<Renderer> { chara.charInfo.rendBody, chara.charInfo.rendFace };
                BoundsUtils.VisualizeRenderers(skins, 2);
            }
        }

        IEnumerator EndOfFrame()
        {
            while(true)
            {
                yield return new WaitForEndOfFrame();
                VectorLineUpdater.UpdateAllLines();
            }
        }

        void Update()
        {
            if(Input.GetMouseButtonDown(1))
            {
                var mousePos = Input.mousePosition;
                Logger.Log(LogLevel.Info, mousePos);
            }
        }

        void OnDestroy()
        {
            VectorLineUpdater.DeleteAllLines();
        }
    }

    public class VectorLineUpdater
    {
        public VectorLine VectorLine;
        public Action Update;

        public static List<VectorLineUpdater> lines = new List<VectorLineUpdater>();
        public VectorLineUpdater() => lines.Add(this);
        public static void UpdateAllLines() => lines.ForEach(x => x.Update());
        public static void DeleteAllLines() => lines.ForEach(x => VectorLine.Destroy(ref x.VectorLine));
    }
}
