namespace Assets.Scripts.Navigation.Scenes
{
    public abstract class NavigationArgs
    {
        public readonly string SceneName;
        public readonly SceneType SceneType;

        protected NavigationArgs(SceneType sceneType, string sceneName)
        {
            SceneType = sceneType;
            SceneName = sceneName;
        }
    }
}


