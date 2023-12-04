using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviourPun
{
    [SerializeField] CharacterAnimations anim;

    public CharacterAnimations Anim { get => anim; set => anim = value; }

}
