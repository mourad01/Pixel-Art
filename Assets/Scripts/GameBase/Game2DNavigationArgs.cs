namespace Assets.Scripts.Navigation.Scenes.Game
{
    internal class WorkbookNavigationArgs : NavigationArgs
    {
        const string GameSceneName = "2DScene";

        public ImageInfo ImageInfo;
        public MainMenuPage Page;
        public SavedWorkData SavedWorkData;

        public int Part
        {
            get;
            set;
        }

        public WorkbookNavigationArgs(ImageInfo imageInfo, MainMenuPage page)
            : base(SceneType.Workbook, "2DScene")
        {
            ImageInfo = imageInfo;
            Page = page;
        }

        public WorkbookNavigationArgs(SavedWorkData savedWorkData = null)
            : base(SceneType.Workbook, "2DScene")
        {
            SavedWorkData = savedWorkData;
        }

        public WorkbookNavigationArgs(ImageInfo imageInfo, int part, SavedWorkData savedWorkData = null)
            : base(SceneType.Workbook, "2DScene")
        {
            ImageInfo = imageInfo;
            SavedWorkData = savedWorkData;
            Part = part;
        }
    }
}


