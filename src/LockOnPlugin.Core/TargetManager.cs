using IllusionUtility.GetUtility;
using System.Collections.Generic;
using UnityEngine;

namespace LockOnPlugin
{
    internal class CameraTargetManager : MonoBehaviour
    {
        private const string CENTERPOINT_NAME = "CenterPoint";

        private List<GameObject> quickTargets = new List<GameObject>();
        private List<CustomTarget> customTargets = new List<CustomTarget>();
        private CenterPoint centerPoint;

        public static CameraTargetManager GetTargetManager(ChaInfo chara)
        {
            var targetManager = chara.gameObject.GetOrAddComponent<CameraTargetManager>();
            targetManager.UpdateAllTargets(chara);
            return targetManager;
        }

        public static bool IsCenterPoint(GameObject point)
        {
            return point.name == CENTERPOINT_NAME;
        }

        public List<GameObject> GetTargets()
        {
            return quickTargets;
        }

        private void Update()
        {
            UpdateCustomTargetTransforms();
        }

        private void UpdateCustomTargetTransforms()
        {
            customTargets.ForEach(x => x.UpdateTransform());
            centerPoint?.UpdatePosition();
        }

        private void UpdateAllTargets(ChaInfo character)
        {
            if(character)
            {
                centerPoint = new CenterPoint(character);
                customTargets = UpdateCustomTargets(character);
                quickTargets = UpdateQuickTargets(character);
            }
        }

        private List<GameObject> UpdateQuickTargets(ChaInfo character)
        {
            var quickTargets = new List<GameObject>();

            foreach(var targetName in TargetData.Instance.quickTargets)
            {
                bool customFound = false;

                if(targetName == CENTERPOINT_NAME)
                {
                    quickTargets.Add(centerPoint.GetPoint());
                    customFound = true;
                }

                foreach(var customTarget in customTargets)
                {
                    if(customTarget.GetTarget().name == targetName)
                    {
                        quickTargets.Add(customTarget.GetTarget());
                        customFound = true;
                    }
                }

                if(!customFound)
                {
                    var bone = character.objBodyBone.transform.FindLoop(targetName);
                    if(bone) quickTargets.Add(bone);
                }
            }

            return quickTargets;
        }

        private List<CustomTarget> UpdateCustomTargets(ChaInfo character)
        {
            var customTargets = new List<CustomTarget>();

            foreach(var data in TargetData.Instance.customTargets)
            {
                bool targetInUse = TargetData.Instance.quickTargets.Contains(data.target);

                if(!targetInUse)
                {
                    foreach(var target in TargetData.Instance.customTargets)
                    {
                        if(target.point1 == data.target || target.point2 == data.target)
                        {
                            targetInUse = true;
                            break;
                        }
                    }
                }

                if(targetInUse)
                {
                    var point1 = character.objBodyBone.transform.FindLoop(data.point1);
                    var point2 = character.objBodyBone.transform.FindLoop(data.point2);

                    foreach(var target in customTargets)
                    {
                        if(target.GetTarget().name == data.point1)
                        {
                            point1 = target.GetTarget();
                        }

                        if(target.GetTarget().name == data.point2)
                        {
                            point2 = target.GetTarget();
                        }
                    }

                    if(point1 && point2)
                    {
                        var target = new CustomTarget(data.target, point1, point2, data.midpoint);
                        target.GetTarget().transform.SetParent(character.transform);
                        customTargets.Add(target);
                    }
                    else
                    {
                        Log.Info($"CustomTarget '{data.target}' failed");
                    }
                }
                else
                {
                    Log.Debug($"CustomTarget '{data.target}' skipped because it is not in use");
                }
            }

            return customTargets;
        }

        private class CustomTarget
        {
            private readonly GameObject target;
            private readonly GameObject point1;
            private readonly GameObject point2;
            private readonly float midpoint;

            public CustomTarget(string name, GameObject point1, GameObject point2, float midpoint = 0.5f)
            {
                target = new GameObject(name);
                this.point1 = point1;
                this.point2 = point2;
                this.midpoint = midpoint;
                UpdateTransform();
            }

            public GameObject GetTarget()
            {
                return target;
            }

            public void UpdateTransform()
            {
                UpdatePosition();
                UpdateRotation();
            }

            private void UpdatePosition()
            {
                var pos1 = point1.transform.position;
                var pos2 = point2.transform.position;
                target.transform.position = Vector3.Lerp(pos1, pos2, midpoint);
            }

            private void UpdateRotation()
            {
                var rot1 = point1.transform.rotation;
                var rot2 = point2.transform.rotation;
                target.transform.rotation = Quaternion.Slerp(rot1, rot2, 0.5f);
            }
        }

        private class CenterPoint
        {
            private readonly List<WeightPoint> points = new List<WeightPoint>();
            private readonly GameObject point;

            public CenterPoint(ChaInfo character)
            {
                foreach(var data in TargetData.Instance.centerWeigths)
                {
                    var point = character.objBodyBone.transform.FindLoop(data.bone);
                    points.Add(new WeightPoint(point, data.weigth));
                }

                if(points.Count > 0)
                {
                    point = new GameObject(CENTERPOINT_NAME);
                    point.transform.SetParent(character.transform);
                    UpdatePosition();
                }
                else
                {
                    point = null;
                }
            }

            public GameObject GetPoint()
            {
                return point;
            }

            public void UpdatePosition()
            {
                if(point)
                    point.transform.position = CalculateCenterPoint(points);
            }

            private Vector3 CalculateCenterPoint(List<WeightPoint> points)
            {
                var center = new Vector3();
                float totalWeight = 0f;
                for(int i = 0; i < points.Count; i++)
                {
                    center += points[i].point.transform.position * points[i].weight;
                    totalWeight += points[i].weight;
                }

                return center / totalWeight;
            }
        }

        private class WeightPoint
        {
            public readonly GameObject point;
            public readonly float weight;

            public WeightPoint(GameObject point, float weight)
            {
                this.point = point;
                this.weight = weight;
            }
        }
    }
}
