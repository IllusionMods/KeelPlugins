namespace KeelPlugins
{
    public static class Constants
    {
#if AI
        public const string MainGameProcessName = "AI-Syoujyo";
        public const string MainGameProcessNameSteam = "AI-Shoujo";
        public const string StudioProcessName = "StudioNEOV2";
#elif HS2
        public const string MainGameProcessName = "HoneySelect2";
        public const string MainGameProcessNameSteam = MainGameProcessName;
        public const string VRProcessName = "HoneySelect2VR";
        public const string StudioProcessName = "StudioNEOV2";
#elif KK
        public const string MainGameProcessName = "Koikatu";
        public const string MainGameProcessNameSteam = "Koikatsu Party";
        public const string VRProcessName = "KoikatsuVR";
        public const string VRProcessNameSteam = "Koikatsu Party VR";
        public const string StudioProcessName = "CharaStudio";
#elif KKS
        public const string MainGameProcessName = "KoikatsuSunshine";
        public const string MainGameProcessNameSteam = MainGameProcessName;
        public const string VRProcessName = "KoikatsuSunshine_VR";
        public const string VRProcessNameSteam = VRProcessName;
        public const string StudioProcessName = "CharaStudio";
#elif PH
        public const string MainGameProcessName64bit = "PlayHome64bit";
        public const string MainGameProcessName32bit = "PlayHome32bit";
        public const string StudioProcessName64bit = "PlayHomeStudio64bit";
        public const string StudioProcessName32bit = "PlayHomeStudio32bit";
#endif
    }
}
