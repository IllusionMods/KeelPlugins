using UnityEngine;
using UnityEngine.UI;

namespace UILib
{
    public class OneTimeVerticalLayoutGroup : VerticalLayoutGroup
    {
        public override void OnEnable()
        {
            base.OnEnable();
            if(Application.isEditor == false || Application.isPlaying)
                this.ExecuteDelayed(() => enabled = false, 3);
        }

        public override void OnDisable()
        {
        }

        public void UpdateLayout()
        {
            enabled = true;
        }
    }
}
