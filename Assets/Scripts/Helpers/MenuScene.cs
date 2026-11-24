
using System;
using UnityEngine;

namespace Assets.Scripts.Navigation.Scenes.Map
{
    internal class MenuScene : BaseScene
    {
        public override SceneType SceneType
        {
            get
            {
                return SceneType.Menu;
            }
        }

        public override void OnNavigatedTo(NavigationArgs args)
        {
            var menuNavigationArgs = args as MenuNavigationArgs;
            SubscribeToPopupClose();
        }

        public override void OnNavigatedFrom()
        {
            UnitySingleton<BackButtonManager>.Instance.BackButtonAction -= ((BaseScene)this).BackButtonActions;
            UnsubscribeFromPopupClose();
        }

        public override void BackButtonActions()
        {
            Application.Quit();
        }

        void SubscribeToPopupClose()
        {
        }

        void UnsubscribeFromPopupClose()
        {
        }
    }
}


