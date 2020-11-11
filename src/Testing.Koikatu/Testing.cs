using BepInEx;
using System;
using System.Collections.Generic;

namespace Testing.Koikatu
{
    [BepInPlugin("keelhauled.testing", "TESTINGPLUGIN", "1.0.0")]
    public class Testing : BaseUnityPlugin
    {
        private List<Action> delete = new List<Action>();

        private void Awake()
        {


            delete.Add(() =>
            {

            });
        }

        private void OnDestroy()
        {
            foreach(var item in delete)
                item();
        }
    }
}
