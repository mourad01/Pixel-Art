/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件由会员免费分享，如果商用，请务必联系原著购买授权！

daily assets update for try.

U should buy a license from author if u use it in your project!
*/

using UnityEngine;
using System.Collections.Generic;

using OneSignalPush.MiniJSON;
using System;

public class GameControllerExample : MonoBehaviour {

   private static string extraMessage;
   public string email = "Email Address";

   void Start () {
      extraMessage = null;

      // Enable line below to debug issues with setuping OneSignal. (logLevel, visualLogLevel)
      OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.VERBOSE, OneSignal.LOG_LEVEL.NONE);

      // The only required method you need to call to setup OneSignal to receive push notifications.
      // Call before using any other methods on OneSignal.
      // Should only be called once when your app is loaded.
      // OneSignal.Init(OneSignal_AppId);
      OneSignal.StartInit("78e8aff3-7ce2-401f-9da0-2d41f287ebaf")
               .HandleNotificationReceived(HandleNotificationReceived)
               .HandleNotificationOpened(HandleNotificationOpened)
               //.InFocusDisplaying(OneSignal.OSInFocusDisplayOption.Notification)
               .EndInit();
      
      OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
      OneSignal.permissionObserver += OneSignal_permissionObserver;
      OneSignal.subscriptionObserver += OneSignal_subscriptionObserver;
      OneSignal.emailSubscriptionObserver += OneSignal_emailSubscriptionObserver;

      var pushState = OneSignal.GetPermissionSubscriptionState();
      Debug.Log("pushState.subscriptionStatus.subscribed : " + pushState.subscriptionStatus.subscribed);
      Debug.Log("pushState.subscriptionStatus.userId : " + pushState.subscriptionStatus.userId);
   }

   private void OneSignal_subscriptionObserver(OSSubscriptionStateChanges stateChanges) {
	  Debug.Log("SUBSCRIPTION stateChanges: " + stateChanges);
	  Debug.Log("SUBSCRIPTION stateChanges.to.userId: " + stateChanges.to.userId);
	  Debug.Log("SUBSCRIPTION stateChanges.to.subscribed: " + stateChanges.to.subscribed);
   }

	private void OneSignal_permissionObserver(OSPermissionStateChanges stateChanges) {
	  Debug.Log("PERMISSION stateChanges.from.status: " + stateChanges.from.status);
	  Debug.Log("PERMISSION stateChanges.to.status: " + stateChanges.to.status);
   }

	private void OneSignal_emailSubscriptionObserver(OSEmailSubscriptionStateChanges stateChanges) {
		Debug.Log("EMAIL stateChanges.from.status: " + stateChanges.from.emailUserId + ", " + stateChanges.from.emailAddress);
		Debug.Log("EMAIL stateChanges.to.status: " + stateChanges.to.emailUserId + ", " + stateChanges.to.emailAddress);
	}

   // Called when your app is in focus and a notificaiton is recieved.
   // The name of the method can be anything as long as the signature matches.
   // Method must be static or this object should be marked as DontDestroyOnLoad
   private static void HandleNotificationReceived(OSNotification notification) {
      OSNotificationPayload payload = notification.payload;
      string message = payload.body;

      print("GameControllerExample:HandleNotificationReceived: " + message);
      print("displayType: " + notification.displayType);
      extraMessage = "Notification received with text: " + message;

   Dictionary<string, object> additionalData = payload.additionalData;
   if (additionalData == null) 
      Debug.Log ("[HandleNotificationReceived] Additional Data == null");
   else
      Debug.Log("[HandleNotificationReceived] message "+ message +", additionalData: "+ Json.Serialize(additionalData) as string);
   }
   
   // Called when a notification is opened.
   // The name of the method can be anything as long as the signature matches.
   // Method must be static or this object should be marked as DontDestroyOnLoad
   public static void HandleNotificationOpened(OSNotificationOpenedResult result) {
      OSNotificationPayload payload = result.notification.payload;
      string message = payload.body;
      string actionID = result.action.actionID;

      print("GameControllerExample:HandleNotificationOpened: " + message);
      extraMessage = "Notification opened with text: " + message;
      
      Dictionary<string, object> additionalData = payload.additionalData;
      if (additionalData == null) 
         Debug.Log ("[HandleNotificationOpened] Additional Data == null");
      else
         Debug.Log("[HandleNotificationOpened] message "+ message +", additionalData: "+ Json.Serialize(additionalData) as string);

      if (actionID != null) {
         // actionSelected equals the id on the button the user pressed.
         // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
         extraMessage = "Pressed ButtonId: " + actionID;
      }
	}

   // Test Menu
   // Includes SendTag/SendTags, getting the userID and pushToken, and scheduling an example notification
   void OnGUI () {
      GUIStyle customTextSize = new GUIStyle("button");
      customTextSize.fontSize = 30;

      GUIStyle guiBoxStyle = new GUIStyle("box");
      guiBoxStyle.fontSize = 30;

      GUIStyle textFieldStyle = new GUIStyle ("textField");
      textFieldStyle.fontSize = 30;


      float itemOriginX = 50.0f;
      float itemWidth = Screen.width - 120.0f;
      float boxWidth = Screen.width - 20.0f;
      float boxOriginY = 120.0f;
      float boxHeight = 630.0f;
      float itemStartY = 200.0f;
      float itemHeightOffset = 90.0f;
      float itemHeight = 60.0f;

      GUI.Box(new Rect(10, boxOriginY, boxWidth, boxHeight), "Test Menu", guiBoxStyle);

      float count = 0.0f;

      if (GUI.Button (new Rect (itemOriginX, itemStartY + (count * itemHeightOffset), itemWidth, itemHeight), "SendTags", customTextSize)) {
         // You can tags users with key value pairs like this:
         OneSignal.SendTag("UnityTestKey", "TestValue");
         // Or use an IDictionary if you need to set more than one tag.
         OneSignal.SendTags(new Dictionary<string, string>() { { "UnityTestKey2", "value2" }, { "UnityTestKey3", "value3" } });

         // You can delete a single tag with it's key.
         // OneSignal.DeleteTag("UnityTestKey");
         // Or delete many with an IList.
         // OneSignal.DeleteTags(new List<string>() {"UnityTestKey2", "UnityTestKey3" });
      }

      count++;

      if (GUI.Button (new Rect (itemOriginX, itemStartY + (count * itemHeightOffset), itemWidth, itemHeight), "GetIds", customTextSize)) {
         OneSignal.IdsAvailable((userId, pushToken) => {
            extraMessage = "UserID:\n" + userId + "\n\nPushToken:\n" + pushToken;
         });
      }


      count++;

      if (GUI.Button (new Rect (itemOriginX, itemStartY + (count * itemHeightOffset), itemWidth, itemHeight), "TestNotification", customTextSize)) {
         extraMessage = "Waiting to get a OneSignal userId. Uncomment OneSignal.SetLogLevel in the Start method if it hangs here to debug the issue.";
         OneSignal.IdsAvailable((userId, pushToken) => {
            if (pushToken != null) {
               // See http://documentation.onesignal.com/docs/notifications-create-notification for a full list of options.
               // You can not use included_segments or any fields that require your OneSignal 'REST API Key' in your app for security reasons.
               // If you need to use your OneSignal 'REST API Key' you will need your own server where you can make this call.

               var notification = new Dictionary<string, object>();
               notification["contents"] = new Dictionary<string, string>() { {"en", "Test Message"} };
               // Send notification to this device.
               notification["include_player_ids"] = new List<string>() { userId };
               // Example of scheduling a notification in the future.
               notification["send_after"] = System.DateTime.Now.ToUniversalTime().AddSeconds(30).ToString("U");

               extraMessage = "Posting test notification now.";

               OneSignal.PostNotification(notification, (responseSuccess) => {
                  extraMessage = "Notification posted successful! Delayed by about 30 secounds to give you time to press the home button to see a notification vs an in-app alert.\n" + Json.Serialize(responseSuccess);
               }, (responseFailure) => {
                  extraMessage = "Notification failed to post:\n" + Json.Serialize(responseFailure);
               });
            } else {
               extraMessage = "ERROR: Device is not registered.";
            }
         });
      }

      count++;

      email = GUI.TextField (new Rect (itemOriginX, itemStartY + (count * itemHeightOffset), itemWidth, itemHeight), email, customTextSize);

      count++;

      if (GUI.Button (new Rect (itemOriginX, itemStartY + (count * itemHeightOffset), itemWidth, itemHeight), "SetEmail", customTextSize)) {
         extraMessage = "Setting email to " + email;

         OneSignal.SetEmail (email, () => {
            Debug.Log("Successfully set email");
         }, (error) => {
            Debug.Log("Encountered error setting email: " + Json.Serialize(error));
         });
      }

      count++;

      if (GUI.Button (new Rect (itemOriginX, itemStartY + (count * itemHeightOffset), itemWidth, itemHeight), "LogoutEmail", customTextSize)) {
         extraMessage = "Logging Out of example@example.com";
         
         OneSignal.LogoutEmail (() => {
            Debug.Log("Successfully logged out of email");
         }, (error) => {
            Debug.Log("Encountered error logging out of email: " + Json.Serialize(error));
         });
      }

      if (extraMessage != null) {
         guiBoxStyle.alignment = TextAnchor.UpperLeft;
         guiBoxStyle.wordWrap = true;
         GUI.Box (new Rect (10, boxOriginY + boxHeight + 20, Screen.width - 20, Screen.height - (boxOriginY + boxHeight + 40)), extraMessage, guiBoxStyle);
      }
   }
}
