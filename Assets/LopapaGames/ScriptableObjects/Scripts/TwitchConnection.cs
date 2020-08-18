using UnityEngine;

namespace LopapaGames
{
    [CreateAssetMenu(fileName = "TwitchConfiguration", menuName = "LopapaGames/Twitch/TwitchConfiguration")]
    public class TwitchConnection : ScriptableObject
    {
        [Header("The Twitch Chat OAuth key (see https://twitchapps.com/tmi/ ):")]
        public string OAuth;
        [Header("The username of above's account:")]
        public string BotName;
        [Header("The channel to listen from:")]
        public string ChannelName;
    }
}