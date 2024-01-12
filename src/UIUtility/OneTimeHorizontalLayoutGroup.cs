using UnityEngine;
using UnityEngine.UI;

namespace UILib
{
    public class OneTimeHorizontalLayoutGroup : HorizontalLayoutGroup
    {
#if HS
        protected override void OnEnable()
#else
        public override void OnEnable()
#endif
        {
            base.OnEnable();
            if (Application.isEditor == false || Application.isPlaying)
                this.ExecuteDelayed(() => enabled = false, 3);
        }

#if HS
        protected override void OnDisable()
#else
        public override void OnDisable()
#endif
        {
        }

        public void UpdateLayout()
        {
            enabled = true;
        }
    }
}
