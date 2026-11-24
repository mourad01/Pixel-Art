

using System;
using UnityEngine;

public class DialogToolWrapper : MonoBehaviour
{
    delegate void DialogCallbackDelegate(int buttonIndex);

    static DialogToolWrapper s_instance;
    static Action OnFacebookLogin;
    static Action RateUsDialogCloseHandler;
    static Action<bool> DeleteDialogCloseHandler;

    public static void ShowUpgradeDialog()
    {
        ActionSheetWrapper.actionSheet.Show2ButtonsDialog(LocalizationManager.Instance.GetString("upgrade_title"), LocalizationManager.Instance.GetString("upgrade_body"), LocalizationManager.Instance.GetString("upgrade_no"), LocalizationManager.Instance.GetString("upgrade_yes"), (buttonIndex) => { UpgradeDialogCallbackHandler(buttonIndex); });
    }

    public static void ShowLoginToFbDialog(Action onFacebookLogin)
    {
        OnFacebookLogin = onFacebookLogin;
        ActionSheetWrapper.actionSheet.Show2ButtonsDialog(LocalizationManager.Instance.GetString(string.Empty), LocalizationManager.Instance.GetString("login_to_fb_text"), LocalizationManager.Instance.GetString("cancel"), LocalizationManager.Instance.GetString("ok"), (buttonIndex) => { LoginToFbDialogCallbackHandler(buttonIndex); });
    }

    public static void ShowRateUsDialog(Action closeHandler)
    {
        AppData.AddNewRateUsView();
        RateUsDialogCloseHandler = closeHandler;
        ActionSheetWrapper.actionSheet.Show2ButtonsDialog(LocalizationManager.Instance.GetString("MyColorful"), LocalizationManager.Instance.GetString("applike") + "\r\n" + LocalizationManager.Instance.GetString("rateus"), LocalizationManager.Instance.GetString("no"), LocalizationManager.Instance.GetString("yes"), (buttonIndex) => { RateUsDialogCallbackHandler(buttonIndex); });
    }

    public static void ShowNoInternetDialog()
    {
        ActionSheetWrapper.actionSheet.ShowOneButtonDialog(LocalizationManager.Instance.GetString(string.Empty), LocalizationManager.Instance.GetString("connectionerror"), LocalizationManager.Instance.GetString("ok"));
    }

    public static void ShowNoAdsDialog()
    {
        ActionSheetWrapper.actionSheet.ShowOneButtonDialog(LocalizationManager.Instance.GetString(string.Empty), LocalizationManager.Instance.GetString("no_ads"), LocalizationManager.Instance.GetString("ok"));
    }

    public static void ShowActivityIndicator(bool show)
    {
    }

    public static void ShowDeleteDialog(Action<bool> callback)
    {
        DeleteDialogCloseHandler = callback;
        ActionSheetWrapper.actionSheet.Show2ButtonsDialog(LocalizationManager.Instance.GetString("delete_title"), LocalizationManager.Instance.GetString("delete_body"), LocalizationManager.Instance.GetString("cancel"), LocalizationManager.Instance.GetString("delete"), (buttonIndex) =>
        {
            DeleteDialogCallbackHandler(buttonIndex);
        });
    }

    //private static void Show2ButtonsDialogInternal(string title, string message, string firstText, string secondText, string callbackName)
    //{
    //    if (DialogToolWrapper.s_instance == null)
    //    {
    //        GameObject gameObject = new GameObject("DialogPluginListener");
    //        DialogToolWrapper.s_instance = gameObject.AddComponent<DialogToolWrapper>();
    //    }
    //    AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.my.DialogPlugin");
    //    androidJavaClass.CallStatic("ShowDialog", title, message, firstText, secondText, DialogToolWrapper.s_instance.name, callbackName);
    //}

    //private static void ShowOneButtonDialogInternal(string title, string message, string buttonText)
    //{
    //    AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.my.DialogPlugin");
    //    androidJavaClass.CallStatic("ShowOneButtonDialog", title, message, buttonText);
    //}

    //button index 1 is the yes button of the dialog
    public static void UpgradeDialogCallbackHandler(int buttonIndexS)
    {
        if (buttonIndexS == 1)
        {
            var newInappsWindow = WindowManager.Instance.OpenInappsWindow();
            newInappsWindow.Init(string.Empty, null);
        }
    }

    public static void LoginToFbDialogCallbackHandler(int buttonIndexS)
    {
        if (buttonIndexS == 1)
        {

        }
    }

    public static void RateUsDialogCallbackHandler(int buttonIndexS)
    {
        if (buttonIndexS == 1)
        {
            AppData.AppRated = true;
            RateUsTool.OpenRateUs();
        }
        RateUsDialogCloseHandler.SafeInvoke();
    }

    public static void DeleteDialogCallbackHandler(int buttonIndexS)
    {
        DeleteDialogCloseHandler.SafeInvoke(buttonIndexS == 1);
    }
}


