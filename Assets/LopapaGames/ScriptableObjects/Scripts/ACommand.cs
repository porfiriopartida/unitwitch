using System.Collections;
using UnityEngine;

namespace LopapaGames
{
    public abstract class ACommand : ScriptableObject
    {
        public string CommandKey;

        /// <summary>
        /// Customized command that the parser calls
        /// </summary>
        /// <param name="user">The user that is sending the message. e. g. "PorfirioPartida" </param>
        /// <param name="message">The message the user is sending: e.g. "!roll 5"</param>
        public abstract IEnumerator Execute(string user, string message);
    }
}