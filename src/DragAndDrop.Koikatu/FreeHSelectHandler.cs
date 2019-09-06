using HarmonyLib;
using UnityEngine;

namespace KeelPlugins
{
    internal class FreeHSelectHandler : CardHandlerMethods
    {
        private const string NormalMaleCanvas = "FreeHScene/Canvas/Panel/Normal/MaleCard";
        private const string NormalFemaleCanvas = "FreeHScene/Canvas/Panel/Normal/FemaleCard";
        private const string MasturbationCanvas = "FreeHScene/Canvas/Panel/Masturbation/FemaleCard";
        private const string LesbianFemaleCanvas = "FreeHScene/Canvas/Panel/Lesbian/FemaleCard";
        private const string LesbianPartnerCanvas = "FreeHScene/Canvas/Panel/Lesbian/PartnerCard";
        private const string ThreesomeMaleCanvas = "FreeHScene/Canvas/Panel/3P/MaleCard";
        private const string ThreesomeFemaleCanvas = "FreeHScene/Canvas/Panel/3P/FemaleCard";
        private const string Stage1Canvas = "FreeHScene/Canvas/Panel/3P/Stage1";
        private const string DarknessMaleCanvas = "FreeHScene/Canvas/Panel/Dark/MaleCard";
        private const string DarknessFemaleCanvas = "FreeHScene/Canvas/Panel/Dark/FemaleCard";

        public override bool Condition => GameObject.FindObjectOfType<FreeHScene>() && !GameObject.FindObjectOfType<FreeHCharaSelect>();

        public override void Character_Load(string path, POINT pos, byte sex)
        {
            if(ActiveAndInBounds(NormalMaleCanvas, pos))
                SetupCharacter(path, ResultType.Player);

            else if(ActiveAndInBounds(NormalFemaleCanvas, pos))
                SetupCharacter(path, ResultType.Heroine);

            else if(ActiveAndInBounds(MasturbationCanvas, pos))
                SetupCharacter(path, ResultType.Heroine);

            else if(ActiveAndInBounds(LesbianFemaleCanvas, pos))
                SetupCharacter(path, ResultType.Heroine);

            else if(ActiveAndInBounds(LesbianPartnerCanvas, pos))
                SetupCharacter(path, ResultType.Partner);

            else if(ActiveAndInBounds(ThreesomeMaleCanvas, pos))
                SetupCharacter(path, ResultType.Player);

            else if(ActiveAndInBounds(ThreesomeFemaleCanvas, pos))
            {
                if(GameObject.Find(Stage1Canvas).activeInHierarchy)
                    SetupCharacter(path, ResultType.Heroine);
                else
                    SetupCharacter(path, ResultType.Partner);
            }

            else if(ActiveAndInBounds(DarknessMaleCanvas, pos))
                SetupCharacter(path, ResultType.Player);

            else if(ActiveAndInBounds(DarknessFemaleCanvas, pos))
                SetupCharacter(path, ResultType.Heroine);
        }

        private bool ActiveAndInBounds(string gameObjectPath, POINT pos)
        {
            var gameObject = GameObject.Find(gameObjectPath);

            if(gameObject && gameObject.activeInHierarchy)
            {
                var rectTransform = gameObject.GetComponent<RectTransform>();
                if(rectTransform)
                {
                    var left = rectTransform.position.x;
                    var top = Screen.height - rectTransform.position.y;
                    var right = left + rectTransform.rect.width;
                    var bottom = top + rectTransform.rect.height;
                    return left < pos.x && pos.x < right && top < pos.y && pos.y < bottom;
                }
            }

            return false;
        }

        private void SetupCharacter(string path, ResultType type)
        {
            var chaFileControl = new ChaFileControl();
            if(chaFileControl.LoadCharaFile(path, 255, false, true))
            {
                var hscene = GameObject.FindObjectOfType<FreeHScene>();
                var member = Traverse.Create(hscene).Field("member");

                switch(type)
                {
                    case ResultType.Heroine:
                        member.Field("resultHeroine").Property("Value").SetValue(new SaveData.Heroine(chaFileControl, false));
                        break;

                    case ResultType.Player:
                        member.Field("resultPlayer").Property("Value").SetValue(new SaveData.Player(chaFileControl, false));
                        break;

                    case ResultType.Partner:
                        member.Field("resultPartner").Property("Value").SetValue(new SaveData.Heroine(chaFileControl, false));
                        break;
                }
            }
        }

        private enum ResultType
        {
            Heroine,
            Player,
            Partner,
        }
    }
}
