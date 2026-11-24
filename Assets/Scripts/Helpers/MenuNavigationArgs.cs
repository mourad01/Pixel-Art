namespace Assets.Scripts.Navigation.Scenes.Map
{
    internal class MenuNavigationArgs : NavigationArgs
    {
        const string MapSceneName = "LibraryScene";

        public readonly MainMenuPage MainMenuPage;

        public MenuNavigationArgs()
            : base(SceneType.Menu, "LibraryScene")
        {
        }

        public MenuNavigationArgs(MainMenuPage mainMenuPage)
            : base(SceneType.Menu, "LibraryScene")
        {
            MainMenuPage = mainMenuPage;
        }
    }
}


