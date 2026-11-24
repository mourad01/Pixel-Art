

using Assets.Scripts.Navigation.Scenes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Navigation
{
    internal static class NavigationService
    {
        const float AsyncLoadingAccuracy = 0.9f;
        static BaseScene _currentScene;
        static NavigationArgs _navigationArgs;
        static AsyncOperation _asyncOperation;

        public static Action AsyncLoadingComplete;

        public static SceneType CurrentSceneType
        {
            get
            {
                if (_currentScene == null)
                {
                    return SceneType.Undefined;
                }
                return _currentScene.SceneType;
            }
        }

        public static void Navigate(NavigationArgs navigator, bool allowSceneActivation = true)
        {
            if (_currentScene != null)
            {
                _currentScene.OnNavigatedFrom();
            }
            _navigationArgs = navigator;
            if (_currentScene != null)
            {
                LoadLevelAsynchronously(navigator.SceneName, allowSceneActivation);
            }
            else
            {
                Application.LoadLevel(navigator.SceneName);
            }
        }

        public static void LoadLevelAsynchronously(string name, bool allowSceneActivation)
        {
            UnitySingleton<MonoHelper>.Instance.StartCoroutine(LoadLevelAsync(name, allowSceneActivation));
        }

        static void OnAsyncLoadingComplete()
        {
            if (AsyncLoadingComplete != null)
            {
                _currentScene.OnNavigatedTo(_navigationArgs);
                AsyncLoadingComplete();
            }
        }

        static IEnumerator LoadLevelAsync(string name, bool allowSceneActivation)
        {
            _asyncOperation = SceneManager.LoadSceneAsync(name);
            _asyncOperation.allowSceneActivation = allowSceneActivation;
            if (!_asyncOperation.isDone)
            {
                if (_asyncOperation.progress >= 0.9f)
                {
                    OnAsyncLoadingComplete();
                }
                yield return null;
            }
            yield return _asyncOperation;
        }

        public static void SetCurrentScene(BaseScene scene)
        {
            _currentScene = scene;
            _currentScene.OnNavigatedTo(_navigationArgs);
        }
    }
}
