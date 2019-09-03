using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vectrosity;
using System;

namespace StudioUX
{
    public static class BoundsUtils
    {
        public static Bounds CombineBounds(List<Renderer> renderers)
        {
            if(renderers.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(renderers), "Need at least one renderer");
            }
            else if(renderers.Count == 1)
            {
                return renderers[0].bounds;
            }
            else
            {
                var bounds = renderers[0].bounds;

                for(int i = 1; i < renderers.Count; i++)
                    bounds.Encapsulate(renderers[i].bounds);

                return bounds;
            }
        }

        public static Rect BoundsToScreenRect(this Bounds bounds, Camera camera)
        {
            var cen = bounds.center;
            var ext = bounds.extents;
            var extentPoints = new Vector2[8]
            {
                camera.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
                camera.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
                camera.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
                camera.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
                camera.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
                camera.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
                camera.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
                camera.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
            };

            var min = extentPoints[0];
            var max = extentPoints[0];
            foreach(var v in extentPoints)
            {
                min = Vector2.Min(min, v);
                max = Vector2.Max(max, v);
            }

            return new Rect(min.x, min.y, max.x-min.x, max.y-min.y);
        }

        public static Vector2 WorldToGUIPoint(Vector3 world)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(world);
            screenPoint.y = Screen.height - screenPoint.y;
            return screenPoint;
        }

        public static void VisualizeRenderers(List<Renderer> renderers, int type)
        {
            var skins = renderers.Select(x => x as SkinnedMeshRenderer).Where(x => x);
            foreach(var skin in skins) skin.updateWhenOffscreen = true;

            if((type & 1) != 0)
            {
                var bounds3d = new VectorLineUpdater();
                bounds3d.VectorLine = new VectorLine("Bounds3D", new List<Vector3>(24), 1f, LineType.Discrete);
                bounds3d.Update = () =>
                {
                    var bounds = CombineBounds(renderers);
                    bounds3d.VectorLine.MakeCube(bounds.center, bounds.size.x, bounds.size.y, bounds.size.z);
                    bounds3d.VectorLine.SetColor(Color.red);
                    bounds3d.VectorLine.Draw();
                };
            }

            if((type & 2) != 0)
            {
                var bounds2d = new VectorLineUpdater();
                bounds2d.VectorLine = new VectorLine("Bounds2D", new List<Vector2>(8), 1f, LineType.Discrete);
                bounds2d.Update = () =>
                {
                    var bounds = CombineBounds(renderers);
                    var rect = BoundsToScreenRect(bounds, Camera.main);
                    bounds2d.VectorLine.MakeRect(rect);
                    bounds2d.VectorLine.SetColor(Color.green);
                    bounds2d.VectorLine.Draw();
                }; 
            }
        }
    }
}
