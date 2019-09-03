using UnityEngine;

namespace KeelPlugins
{
    internal class NeckRotator
    {
        public NeckMode mode;
        public Vector3 first;
        public Vector3 second;

        public NeckRotator(NeckMode mode)
        {
            this.mode = mode;
        }
    }

    internal enum NeckMode
    {
        Both,
        First,
        Second
    }
}
