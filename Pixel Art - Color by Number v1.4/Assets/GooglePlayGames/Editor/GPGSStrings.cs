/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件由会员免费分享，如果商用，请务必联系原著购买授权！

daily assets update for try.

U should buy a license from author if u use it in your project!
*/

// <copyright file="GPGSStrings.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

// Keep the strings all the time even if on an unsupported configuration.

namespace GooglePlayGames.Editor
{
    public class GPGSStrings
    {
        public const string Error = "Error";
        public const string Ok = "OK";
        public const string Cancel = "Cancel";
        public const string Yes = "Yes";
        public const string No = "No";
        public const string Success = "Success";
        public const string Warning = "Warning";

        public class PostInstall
        {
            public const string Title = "Google Play Games Plugin for Unity";
            public const string Text = "The Google Play Games Plugin for Unity version $VERSION " +
                                       "is now ready to use. If this is a new installation or of you have " +
                                       "just upgraded from a previous version, please click the 'Google Play Games' " +
                                       "menu and select 'Android Setup' to set up your project.";
        }

        public class Setup
        {
            public const string AppIdTitle = "Google Play Games Application ID";
            public const string AppId = "Application ID";
            public const string AppIdBlurb = "Enter your application ID below. This is the numeric\n" +
                                             "identifier provided by the Developer Console (for example, 123456789012).";

            public const string AppIdError = "The App Id does not appear to be valid. " +
                                             "It must consist solely of digits, usually 10 or more.";

            public const string WebClientIdTitle = "Web App Client ID (Optional)";
            public const string ClientId = "Client ID";
            public const string ClientIdError = "The Client ID does not appear to be valid. " +
                                                "It should end in .apps.googleusercontent.com.";
            public const string AppIdMismatch = "Web app client ID not associated with this game!";

            public const string NearbyServiceId = "Nearby Connection Service ID";
            public const string NearbyServiceBlurb = "Enter the service id that identifies the " +
                                                     "nearby connections service scope";

            public const string SetupButton = "Setup";
        }

        public class NearbyConnections
        {
            public const string Title = "Google Play Games - Nearby Connections Setup";
            public const string Blurb = "To configure Nearby Connections in this project,\n" +
                                        "please enter the information below and click on the Setup button.";

            public const string SetupComplete = "Nearby connections configured successfully.";
        }

        public class AndroidSetup
        {
            public const string Title = "Google Play Games - Android Configuration";
            public const string Blurb = "To configure Google Play Games in this project,\n" +
                                        "go to the Play Game console, then enter the information below and click on the Setup button.";

            public const string WebClientIdBlurb = "The web app client ID is needed to access the user's ID token and " +
                "call other APIs onbehalf of the user." +
                "  It is not required for Game Services.  Enter your oauth2 client ID below.\nTo obtain this " +
                "ID, generate a web linked app in Developer Console. Example:\n" +
                "123456789012-abcdefghijklm.apps.googleusercontent.com";

            public const string PkgName = "Package name";
            public const string PkgNameBlurb = "Enter your application's package name below.\n" +
                                               "(for example, com.example.lorem.ipsum).";

            public const string PackageNameError = "The package name does not appear to be valid. " +
                                                   "Enter a valid Android package name (for example, com.example.lorem.ipsum).";

            public const string SdkNotFound = "Android SDK Not found";
            public const string SdkNotFoundBlurb = "The Android SDK path was not found. " +
                                                   "Please configure it in the Unity preferences window (under External Tools).";

            public const string LibProjNotFound = "Google Play Services Library Project Not Found";
            public const string LibProjNotFoundBlurb = "Google Play Services library project " +
                                                       "could not be found your SDK installation. Make sure it is installed (open " +
                                                       "the SDK manager and go to Extras, and select Google Play Services).";

            public const string SupportJarNotFound = "Android Support Library v4 Not Found";
            public const string SupportJarNotFoundBlurb = "Android Support Library v4 " +
                                                          "could not be found your SDK installation. Make sure it is installed (open " +
                                                          "the SDK manager and go to Extras, and select 'Android Support Library').";

            public const string LibProjVerNotFound = "The version of your copy of the Google Play " +
                                                     "Services Library Project could not be determined. Please make sure it is " +
                                                     "at least version {0}. Continue?";

            public const string LibProjVerTooOld = "Your copy of the Google Play " +
                                                   "Services Library Project is out of date. Please launch the Android SDK manager " +
                                                   "and upgrade your Google Play Services bundle to the latest version (your version: " +
                                                   "{0}; required version: {1}). Proceeding may cause problems. Proceed anyway?";

            public const string SetupComplete = "Google Play Games configured successfully.";
        }

        public class ExternalLinks
        {
            public const string GettingStartedGuideURL =
                "https://github.com/playgameservices/play-games-plugin-for-unity";

            public const string PlayGamesServicesApiURL =
                "https://developers.google.com/games/services";

            public const string GooglePlayGamesAndroidSdkTitle = "Google Play Games Android SDK Download";
            public const string GooglePlayGamesAndroidSdkBlurb = "The Google Play Games SDK for " +
                                                                 "Android must be downloaded via the Android SDK Manager. Do you wish to " +
                                                                 "start the SDK manager now?";

            public const string GooglePlayGamesAndroidSdkInstructions = "The Android SDK manager " +
                                                                        "will be launched. Install or upgrade the 'Google Play Services' package, " +
                                                                        "which can be found under the 'Extras' " +
                                                                        "category.";

            public const string GooglePlayGamesAndroidSdkManagerFailed = "Failed to find the " +
                                                                         "Android SDK manager executable. Make sure the Android SDK is properly installed " +
                                                                         "and that its path is correctly configured in the Unity preferences window " +
                                                                         "(under External Tools).";
        }

        public const string AboutTitle = "Google Play Games Plugin for Unity";
        public const string AboutText = "Copyright (C) 2014 Google Inc.\n\nThis is an open-source " +
                                        "plugin that allows cross-platform integration with Google Play games services. " +
                                        "For more information, visit the official site on Github:\n\n" +
                                        "https://github.com/playgameservices/play-games-plugin-for-unity\n\nPlugin version: ";

        public const string LicenseTitle = "Google Play Games Plugin for Unity";
        public const string LicenseText = "Copyright (C) 2014 Google Inc. All Rights Reserved.\n\n" +
                                          "Licensed under the Apache License, Version 2.0 (the \"License\"); " +
                                          "you may not use this file except in compliance with the License. " +
                                          "You may obtain a copy of the License at\n\n" +
                                          "      http://www.apache.org/licenses/LICENSE-2.0\n\n" +
                                          "Unless required by applicable law or agreed to in writing, software " +
                                          "distributed under the License is distributed on an \"AS IS\" BASIS, " +
                                          "WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. " +
                                          "See the License for the specific language governing permissions and " +
                                          "limitations under the License.";
    }
}
