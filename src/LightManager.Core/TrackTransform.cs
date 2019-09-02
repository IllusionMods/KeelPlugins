using UnityEngine;

namespace KeelPlugins
{
    internal class TrackTransform : MonoBehaviour
    {
        public string targetName;
        public Transform target;
        public int targetKey;
        public float rotationSpeed = 1f;

        private void Update()
        {
            if(target)
            {
                var rotation = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
