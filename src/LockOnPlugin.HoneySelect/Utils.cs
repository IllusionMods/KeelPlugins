using System;
using System.Reflection;
using UnityEngine;

namespace LockOnPlugin
{
    internal static class Utils
    {
        public static bool DebugGUI(float screenWidthMult, float screenHeightMult, float width, float height, string msg)
        {
            float xpos = Screen.width * screenWidthMult - width / 2f;
            float ypos = Screen.height * screenHeightMult - height / 2f;
            xpos = Mathf.Clamp(xpos, 0f, Screen.width - width);
            ypos = Mathf.Clamp(ypos, 0f, Screen.height - height);
            return GUI.Button(new Rect(xpos, ypos, width, height), msg);
        }

        public static FieldType GetSecureField<FieldType, ObjectType>(string fieldName, ObjectType target = null)
            where FieldType : class
            where ObjectType : UnityEngine.Object
        {
            if(target.Equals(null))
            {
                target = GameObject.FindObjectOfType<ObjectType>();
            }

            if(!target.Equals(null))
            {
                FieldInfo fieldinfo = typeof(ObjectType).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                if(!fieldinfo.Equals(null))
                {
                    return fieldinfo.GetValue(target) as FieldType;
                }
            }

            return null;
        }

        public static MethodInfo GetSecureMethod<ObjectType>(string methodName)
            where ObjectType : UnityEngine.Object
        {
            MethodInfo methodinfo = typeof(ObjectType).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if(!methodinfo.Equals(null))
            {
                return methodinfo;
            }

            return null;
        }

        public static object InvokePluginMethod(string typeName, string methodName, object[] parameters = null)
        {
            Type type = FindType(typeName);

            if(type != null)
            {
                UnityEngine.Object instance = GameObject.FindObjectOfType(type);

                if(instance != null)
                {
                    MethodInfo methodInfo = type.GetMethod(methodName);

                    if(methodInfo != null)
                    {
                        if(methodInfo.GetParameters().Length == 0)
                        {
                            return methodInfo.Invoke(instance, null);
                        }
                        else
                        {
                            return methodInfo.Invoke(instance, parameters);
                        }
                    }
                }
            }

            return null;
        }

        public static Type FindType(string qualifiedTypeName)
        {
            Type t = Type.GetType(qualifiedTypeName);

            if(t != null)
            {
                return t;
            }
            else
            {
                foreach(Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = asm.GetType(qualifiedTypeName);
                    if(t != null)
                    {
                        return t;
                    }
                }

                return null;
            }
        }

        //public static Vector3 CameraAdjustedEulerAngles(GameObject target, Transform cameraTransform)
        //{
        //    float x = AngleSigned(target.transform.forward, Vector3.forward, cameraTransform.right);
        //    float y = AngleSigned(target.transform.right, Vector3.right, cameraTransform.up);
        //    float z = AngleSigned(target.transform.up, Vector3.up, cameraTransform.forward);
        //    return new Vector3(x, y, z);
        //}

        //public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        //{
        //    return Mathf.Atan2(
        //        Vector3.Dot(n, Vector3.Cross(v1, v2)),
        //        Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        //}
    }
}
