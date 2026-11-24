using Assets.Scripts.Navigation.Scenes.Map;
using System;
using UnityEngine;

namespace Assets.Scripts.Navigation.Scenes.Game
{
    internal class WorkbookScene : BaseScene
    {
        public override SceneType SceneType
        {
            get
            {
                return SceneType.Workbook;
            }
        }

        public override void Start()
        {
            NavigationService.SetCurrentScene(this);
            UnitySingleton<BackButtonManager>.Instance.BackButtonAction += ((BaseScene)this).BackButtonActions;
        }

        public override void OnNavigatedTo(NavigationArgs args)
        {
            var workbookNavigationArgs = args as WorkbookNavigationArgs;
            if (workbookNavigationArgs.SavedWorkData == null)
            {
                MainMenu.LastPage = workbookNavigationArgs.Page;
                MainMenu.ImageId = workbookNavigationArgs.ImageInfo.Id;
                MainMenu.WorkId = null;
            }
            else
            {
                Resources.UnloadUnusedAssets();
                UnitySingleton<BackButtonManager>.Instance.SetPause(true);
            }
        }

        public override void OnNavigatedFrom()
        {
            NewWorkbookManager.Instance.SaveWork(false);
            UnitySingleton<BackButtonManager>.Instance.BackButtonAction -= ((BaseScene)this).BackButtonActions;
        }

        public override void BackButtonActions()
        {
            NavigationService.Navigate(new MenuNavigationArgs(), true);
        }
    }
}


