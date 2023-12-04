using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviourPun
{
    CharacterAnimations anim;

    public CharacterAnimations Anim { get => anim; set => anim = value; }

    private void Awake()
    {
        if(photonView.IsMine)
        {
            anim = GetComponent<CharacterAnimations>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
