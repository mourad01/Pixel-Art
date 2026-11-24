using OneSignalPush.MiniJSON;
using System.Collections.Generic;

class OneSignalPlatformHelper {
   internal static OSPermissionSubscriptionState parsePermissionSubscriptionState(OneSignalPlatform platform, string jsonStr) {
      var stateDict = Json.Deserialize(jsonStr) as Dictionary<string, object>;

      var state = new OSPermissionSubscriptionState();
      state.permissionStatus = platform.parseOSPermissionState(stateDict["permissionStatus"]);
      state.subscriptionStatus = platform.parseOSSubscriptionState(stateDict["subscriptionStatus"]);

      if (stateDict.ContainsKey("emailSubscriptionStatus"))
         state.emailSubscriptionStatus = platform.parseOSEmailSubscriptionState (stateDict ["emailSubscriptionStatus"]);
      
	  return state;
   }

   internal static OSPermissionStateChanges parseOSPermissionStateChanges(OneSignalPlatform platform, string stateChangesJSONString) {
      var stateChangesJson = Json.Deserialize(stateChangesJSONString) as Dictionary<string, object>;

      var permissionStateChanges = new OSPermissionStateChanges();
      permissionStateChanges.to = platform.parseOSPermissionState(stateChangesJson["to"]);
      permissionStateChanges.from = platform.parseOSPermissionState(stateChangesJson["from"]);

      return permissionStateChanges;
   }

   internal static OSSubscriptionStateChanges parseOSSubscriptionStateChanges(OneSignalPlatform platform, string stateChangesJSONString) {
      var stateChangesJson = Json.Deserialize(stateChangesJSONString) as Dictionary<string, object>;

      var permissionStateChanges = new OSSubscriptionStateChanges();
      permissionStateChanges.to = platform.parseOSSubscriptionState(stateChangesJson["to"]);
      permissionStateChanges.from = platform.parseOSSubscriptionState(stateChangesJson["from"]);

      return permissionStateChanges;
   }

   internal static OSEmailSubscriptionStateChanges parseOSEmailSubscriptionStateChanges(OneSignalPlatform platform, string stateChangesJSONString) {
      var stateChangesJson = Json.Deserialize(stateChangesJSONString) as Dictionary<string, object>;

      var emailStateChanges = new OSEmailSubscriptionStateChanges();
      emailStateChanges.to = platform.parseOSEmailSubscriptionState (stateChangesJson ["to"]);
      emailStateChanges.from = platform.parseOSEmailSubscriptionState (stateChangesJson ["from"]);

      return emailStateChanges;
   }
}
