using System.Collections;
using LopapaGames;
using UnityEngine;

[CreateAssetMenu(fileName = "SampleCommand", menuName = "LopapaGames/Twitch/Sample/Command")]
public class SampleCommand : ACommand
{
    public bool setActive;
    public override IEnumerator Execute(string user, string parameters)
    {
        Debug.Log(user + " : " + parameters);
        DemoUIManager.Instance.textElement.SetActive(setActive);
        yield return null;
    }
}
