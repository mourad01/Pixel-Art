
using System;
using UnityEngine;

namespace Assets.Scripts.Navigation.Scenes
{
    internal abstract class BaseScene : MonoBehaviour
    {
        public abstract SceneType SceneType
        {
            get;
        }

        public abstract void BackButtonActions();

        public virtual void Start()
        {
            NavigationService.SetCurrentScene(this);
            UnitySingleton<BackButtonManager>.Instance.SetPause(false);
            UnitySingleton<BackButtonManager>.Instance.BackButtonAction += BackButtonActions;
        }

        public abstract void OnNavigatedTo(NavigationArgs args);

        public abstract void OnNavigatedFrom();
    }
}


