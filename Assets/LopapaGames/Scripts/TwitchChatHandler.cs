using System;
using System.Collections;
using UnityEngine;
using WebSocket = WebSocketSharp.WebSocket;
using WebSocketState = WebSocketSharp.WebSocketState;

namespace LopapaGames
{
    [RequireComponent(typeof(UnityMainThreadDispatcher))]
    public class TwitchChatHandler : MonoBehaviour
    {
        private static string URL = "wss://irc-ws.chat.twitch.tv:443/";
        private static string WHISPER = "WHISPER";
        private static string PRIVMSG = "PRIVMSG";
        
        [Header("Required scriptable object configuration with the twitch connection oauth/bot name/channel to read.")]
        public TwitchConnection configuration;

        [Header("Attempts to parse messages sent directly to the user (defined in Configuration)")]
        public bool ListenToWhispers = true;
        [Header("Attempts to parse messages sent to the public chat (defined in Configuration)")]
        public bool ListenToPublicChat = true;
        [Header("Enables the !help (configurable below) to be used.")]
        public bool EnableHelp = true;
        [Header("The key string to trigger the help command")]
        public string HelpCommand = "!help";
        [Header("IRC events listener")]
        public TwitchChatCallbackHandler twitchChatCallbackHandler;

        private WebSocket ws;

        public ACommand[] Commands;

        private bool _hasErrors = false;
        void Start()
        {
            if (configuration)
            {
                Debug.Log("Starting TwitchHandler.");
                using (ws = new WebSocket(URL, "irc"))
                {
                    Debug.Log("WebSocket: " + URL);

                    ws.EmitOnPing = true;

                    ws.OnMessage += (sender, e) => OnMessage(e.Data);

                    ws.OnClose += (sender, e) => OnClose(e.Reason);

                    ws.OnError += (sender, e) => OnError(e.Message);

                    ws.OnOpen += (sender, e) => OnConnect(e.ToString());
                }
                
                if (String.IsNullOrEmpty(this.configuration.OAuth))
                {
                    _hasErrors = true;
                }
                if (String.IsNullOrEmpty(this.configuration.BotName))
                {
                    _hasErrors = true;
                }
                if (String.IsNullOrEmpty(this.configuration.ChannelName))
                {
                    _hasErrors = true;
                }
            }
            else
            {
                _hasErrors = true;
                Debug.LogError("Twitch Configuration was not found or not assigned to the TwitchChatHandler in " + this.gameObject.name);
            }
        }

        public void Reconnect()
        {
            if (_hasErrors)
            {
                return;
            }

            twitchChatCallbackHandler.Reconnect();
            ws.Connect();

            if (ws.ReadyState == WebSocketState.Open)
            {
                ws.Send("CAP REQ :twitch.tv/tags");
                ws.Send("CAP REQ :twitch.tv/commands");
                ws.Send("PASS " + configuration.OAuth);
                ws.Send("NICK " + configuration.BotName);
                ws.Send("JOIN #" + configuration.ChannelName);
            }
            else
            {
                Debug.Log("Connection NOT Ready.");
            }
        }
        private string getUserName(string twitchMessage)
        {
            string magicWord = "display-name=";
            int indexOfWhisper = twitchMessage.IndexOf(magicWord) + magicWord.Length;
            twitchMessage = twitchMessage.Substring(indexOfWhisper);
            int indexOfDelimiter = twitchMessage.IndexOf(";");
            string userName = twitchMessage.Substring(0, indexOfDelimiter); //-1 to remove the semi colon.

            return userName;
        }
        public void OnMessage(string msg)
        {
            if (_hasErrors)
            {
                return;
            }
            twitchChatCallbackHandler.OnMessage(msg);
            
            if (msg.StartsWith("PING"))
            {
                ws.Send("PONG");
            }
            else if (ListenToWhispers && msg.Contains(WHISPER))
            {
                string userName = getUserName(msg);
                int indexOfWhisper = msg.IndexOf(WHISPER) + WHISPER.Length;
                msg = msg.Substring(indexOfWhisper);
                int indexOfColon = msg.IndexOf(":") + 1;
                msg = msg.Substring(indexOfColon).Trim(); //Here the msg may contain BOMB 2

                ParseMessage(userName, msg);
            }
            else if (ListenToPublicChat && msg.Contains(PRIVMSG))
            {
                string userName = getUserName(msg);

                int indexOfMsg = msg.IndexOf(PRIVMSG) + PRIVMSG.Length;
                msg = msg.Substring(indexOfMsg);
                
                int indexOfColon = msg.IndexOf(":") + 1;
                msg = msg.Substring(indexOfColon).Trim(); //Here the msg may contain BOMB 2
                
                ParseMessage(userName, msg);
            }
        }
        private IEnumerator SendBombsAsync(string sender, int count)
        {
            //gameLogicController.AddBombs(sender, count);
            yield return null;
        }
        public void SendBombs(string sender, int count)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(SendBombsAsync(sender, count));
        }
        private IEnumerator TriggerMethodAsync(string sender)
        {
            yield return null;
        }
        public void TriggerMethod(string sender)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(TriggerMethodAsync(sender));
        }

        private void ParseMessage(string userName, string message)
        {
            if (_hasErrors)
            {
                return;
            }
            if (String.IsNullOrEmpty(userName))
            {
                return;
            }
            if (String.IsNullOrEmpty(message))
            {
                return;
            }
            if (EnableHelp && message.StartsWith(HelpCommand))
            {
                foreach (ACommand _command in Commands)
                {
                    if (!String.IsNullOrEmpty(_command.CommandKey))
                    {
                        Debug.Log(_command.CommandKey);
                    }
                }
                return;
            }

            foreach (ACommand _command in Commands)
            {
                if (!String.IsNullOrEmpty(_command.CommandKey) && message.StartsWith(_command.CommandKey))
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(_command.Execute(userName, message));
                }
            }
        }
        public void OnError(string errorMsg)
        {
            twitchChatCallbackHandler.OnError(errorMsg);
        }
        public void OnConnect(string e)
        {
            twitchChatCallbackHandler.OnConnect(e);
        }
        public void OnClose(string reason)
        {
            twitchChatCallbackHandler.OnClose(reason);
        }

        void Update()
        {
            if (configuration)
            {
                if (_hasErrors)
                {
                    return;
                }
                if (ws.ReadyState != WebSocketState.Open)
                {
                    Reconnect();
                }
            }
        }

        void OnApplicationQuit()
        {
            //Trying to close twitch connection.
            ws.Close();
        }
    }
}