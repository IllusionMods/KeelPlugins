#pragma warning disable 649 // disable never assigned warning

using ParadoxNotion.Serialization;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BlendShaper.Koikatu
{
    internal class BlendSets
    {
        public static BlendSets LoadBlendSetData()
        {
            string resourceName = $"{nameof(BlendShaper)}.BlendSets.json";
            using(var stream = typeof(BlendShaper).Assembly.GetManifestResourceStream(resourceName))
            {
                using(var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    return JSONSerializer.Deserialize<BlendSets>(json);
                }
            }
        }

        public List<Category> Categories;

        public class Category
        {
            public string Name;
            public List<Set> Sets;
        }

        public class Set
        {
            public void ChangeValue(float value)
            {
                Enabled = true;
                Value = value;
            }

            public void CreateActions(OCIChar chara)
            {
                Actions.Clear();

                var skinnedMeshRenderers = chara.charInfo.animBody.GetComponentsInChildren<SkinnedMeshRenderer>(true).Where(x => x.sharedMesh && x.sharedMesh.blendShapeCount > 0).ToList();

                var firstShape = Shapes.First();
                Value = skinnedMeshRenderers.FirstOrDefault(x => x.name == firstShape.Renderer).GetBlendShapeWeight(firstShape.Index);

                foreach(var renderer in skinnedMeshRenderers)
                {
                    foreach(var shape in Shapes.Where(shape => renderer.name == shape.Renderer))
                        Actions.Add(() => renderer.SetBlendShapeWeight(shape.Index, Value));
                }
            }

            public void Execute()
            {
                if(Enabled)
                {
                    foreach(var action in Actions)
                        action();
                }
            }

            public bool Enabled;
            public float Value;
            public List<Action> Actions = new List<Action>();

            public string Name;
            public List<Shape> Shapes;

            public class Shape
            {
                public string Renderer;
                public int Index;
            }
        }
    }
}
