using UnityEngine;
using UnityEngine.UI;

namespace UILib
{
    public class OneTimeHorizontalLayoutGroup : HorizontalLayoutGroup
    {
#if KKS
        public override void OnEnable()
#else
        protected override void OnEnable()
#endif
        {
            base.OnEnable();
            if (Application.isEditor == false || Application.isPlaying)
                this.ExecuteDelayed(() => enabled = false, 3);
        }

#if KKS
        public override void OnDisable()
#else
        protected override void OnDisable()
#endif
        {
        }

        public void UpdateLayout()
        {
            enabled = true;
        }
    }
}
