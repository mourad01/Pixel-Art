/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件由会员免费分享，如果商用，请务必联系原著购买授权！

daily assets update for try.

U should buy a license from author if u use it in your project!
*/

// <copyright file="RealTimeMultiplayerListener.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc. All rights reserved.
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

namespace GooglePlayGames.BasicApi.Multiplayer
{
  /// <summary>
  /// Real time multiplayer listener. This listener will be called to notify you
  /// of real-time room events.
  /// </summary>
  public interface RealTimeMultiplayerListener
  {
    /// <summary>
    /// Called during room setup to notify of room setup progress.
    /// </summary>
    /// <param name="percent">The room setup progress in percent (0.0 to 100.0).</param>
    void OnRoomSetupProgress(float percent);

    /// <summary>
    /// Notifies that room setup is finished. If <c>success == true</c>, you should
    /// react by starting to play the game; otherwise, show an error screen.
    /// </summary>
    /// <param name="success">Whether setup was successful.</param>
    void OnRoomConnected(bool success);

    /// <summary>
    /// Notifies that the current player has left the room. This may have happened
    /// because you called LeaveRoom, or because an error occurred and the player
    /// was dropped from the room. You should react by stopping your game and
    /// possibly showing an error screen (unless leaving the room was the player's
    /// request, naturally).
    /// </summary>
    void OnLeftRoom();

    /// <summary>
    /// Raises the participant left event.
    /// This is called during room setup if a player declines an invitation
    /// or leaves.  The status of the participant can be inspected to determine
    /// the reason.  If all players have left, the room is closed automatically.
    /// </summary>
    /// <param name="participant">Participant that left</param>
    void OnParticipantLeft(Participant participant);

    /// <summary>
    /// Called when peers connect to the room.
    /// </summary>
    /// <param name="participantIds">Participant identifiers.</param>
    void OnPeersConnected(string[] participantIds);

    /// <summary>
    /// Called when peers disconnect from the room.
    /// </summary>
    /// <param name="participantIds">Participant identifiers.</param>
    void OnPeersDisconnected(string[] participantIds);

    /// <summary>
    /// Called when a real-time message is received.
    /// </summary>
    /// <param name="isReliable">Whether the message was sent as a reliable message or not.</param>
    /// <param name="senderId">Sender identifier.</param>
    /// <param name="data">Data.</param>
    void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data);
  }
}
#endif
