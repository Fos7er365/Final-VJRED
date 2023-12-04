using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Realtime;
using TMPro;
public class VoiceChatUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    Dictionary<Speaker, Player> _dic = new Dictionary<Speaker, Player>();
    Speaker speaker;

    public void AddSpeaker(Speaker speaker, Player player)
    {
        _dic[speaker] = player;
    }

    private void Update()
    {
        string voicechat = "";

        foreach (var item in _dic)
        {
            var speaker = item.Key;
            if (speaker.IsPlaying)
            {
                voicechat += item.Value.NickName + "\n";
            }
        }
        text.text = voicechat;
    }
}
