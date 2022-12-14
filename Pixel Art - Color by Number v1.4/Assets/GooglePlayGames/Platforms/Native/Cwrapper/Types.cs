/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件由会员免费分享，如果商用，请务必联系原著购买授权！

daily assets update for try.

U should buy a license from author if u use it in your project!
*/

// <copyright file="Types.cs" company="Google Inc.">
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

#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))


namespace GooglePlayGames.Native.Cwrapper
{
    internal static class Types
    {
        internal enum DataSource
        {
            CACHE_OR_NETWORK = 1,
            NETWORK_ONLY = 2,
        }

        internal enum LogLevel
        {
            VERBOSE = 1,
            INFO = 2,
            WARNING = 3,
            ERROR = 4,
        }

        internal enum AuthOperation
        {
            SIGN_IN = 1,
            SIGN_OUT = 2,
        }

        internal enum ImageResolution
        {
            ICON = 1,
            HI_RES = 2,
        }

        internal enum AchievementType
        {
            STANDARD = 1,
            INCREMENTAL = 2,
        }

        internal enum AchievementState
        {
            HIDDEN = 1,
            REVEALED = 2,
            UNLOCKED = 3,
        }

        internal enum EventVisibility
        {
            HIDDEN = 1,
            REVEALED = 2,
        }

        internal enum LeaderboardOrder
        {
            LARGER_IS_BETTER = 1,
            SMALLER_IS_BETTER = 2,
        }

        internal enum LeaderboardStart
        {
            TOP_SCORES = 1,
            PLAYER_CENTERED = 2,
        }

        internal enum LeaderboardTimeSpan
        {
            DAILY = 1,
            WEEKLY = 2,
            ALL_TIME = 3,
        }

        internal enum LeaderboardCollection
        {
            PUBLIC = 1,
            SOCIAL = 2,
        }

        internal enum ParticipantStatus
        {
            INVITED = 1,
            JOINED = 2,
            DECLINED = 3,
            LEFT = 4,
            NOT_INVITED_YET = 5,
            FINISHED = 6,
            UNRESPONSIVE = 7,
        }

        internal enum MatchResult
        {
            DISAGREED = 1,
            DISCONNECTED = 2,
            LOSS = 3,
            NONE = 4,
            TIE = 5,
            WIN = 6,
        }

        internal enum MatchStatus
        {
            INVITED = 1,
            THEIR_TURN = 2,
            MY_TURN = 3,
            PENDING_COMPLETION = 4,
            COMPLETED = 5,
            CANCELED = 6,
            EXPIRED = 7,
        }
         
        internal enum MultiplayerEvent
        {
            UPDATED = 1,
            UPDATED_FROM_APP_LAUNCH = 2,
            REMOVED = 3,
        }

        internal enum MultiplayerInvitationType
        {
            TURN_BASED = 1,
            REAL_TIME = 2,
        }

        internal enum RealTimeRoomStatus
        {
            INVITING = 1,
            CONNECTING = 2,
            AUTO_MATCHING = 3,
            ACTIVE = 4,
            DELETED = 5,
        }

        internal enum SnapshotConflictPolicy
        {
            MANUAL = 1,
            LONGEST_PLAYTIME = 2,
            LAST_KNOWN_GOOD = 3,
            MOST_RECENTLY_MODIFIED = 4,
        }

        internal enum VideoCaptureMode {
            UNKNOWN = -1,
            FILE = 0,
            STREAM = 1,
        }

        internal enum VideoQualityLevel {
            UNKNOWN = -1,
            SD = 0,
            HD = 1,
            XHD = 2,
            FULLHD = 3
        }

        internal enum VideoCaptureOverlayState {
            UNKNOWN = -1,
            SHOWN = 1,
            STARTED = 2,
            STOPPED = 3,
            DISMISSED = 4
        };
    }
}
#endif // (UNITY_ANDROID || UNITY_IPHONE)
