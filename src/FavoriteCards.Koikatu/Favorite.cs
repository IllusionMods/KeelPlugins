using System.Collections.Generic;
using System.Linq;
using BepInEx;
using KKAPI.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

[assembly: System.Reflection.AssemblyVersion(FavoriteCards.Koikatu.Favorite.Version)]

namespace FavoriteCards.Koikatu
{
    [BepInPlugin(GUID, "Favorite Cards", Version)]
    public class Favorite : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.favoritecards";
        public const string Version = "1.0.0." + BuildNumber.Version;

        private bool show;
        private Rect rect = new Rect(0f, 0f, 100f, 60f);

        private void Awake()
        {
            Log.SetLogSource(Logger);
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(1))
            {
                var mousePos = Input.mousePosition;
                rect.position = new Vector2(mousePos.x, Screen.height - mousePos.y);
                
                var hits = HitList(mousePos);
                show = hits.Any(x => x.gameObject.name.Contains("ThumbBG"));
            }

            if(show && Input.GetMouseButtonDown(0))
            {
                show = false;
            }
        }

        private void OnGUI()
        {
            if(show)
            {
                GUILayout.Window(0, rect, DrawWindow, "", GUI.skin.box);
                IMGUIUtils.EatInputInRect(rect);
            }
        }

        private void DrawWindow(int id)
        {
            GUILayout.BeginVertical();

            if(GUILayout.Button("Load", GUI.skin.button, GUILayout.ExpandHeight(true)))
            {
                show = false;
            }

            if(GUILayout.Button("Favorite", GUI.skin.button, GUILayout.ExpandHeight(true)))
            {
                show = false;
            }
            
            GUILayout.EndVertical();
        }
        
        public static List<RaycastResult> HitList(Vector2 position)
        {
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
            {
                position = position
            }, raycastResults);
            return raycastResults;
        }
    }
}