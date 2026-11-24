
using System;

namespace Assets.Scripts.Navigation
{
    internal class BackButtonManager : UnitySingleton<BackButtonManager>
    {
        bool _pause;

        public Action BackButtonAction;

        public event Action PopupBackButtonAction;

        BackButtonManager()
        {
        }

        public void SetPause(bool value)
        {
            _pause = value;
        }

        void OnPopupBackButtonAction()
        {
            var popupBackButtonAction = PopupBackButtonAction;
            if (popupBackButtonAction != null)
            {
                popupBackButtonAction();
            }
        }
    }
}


