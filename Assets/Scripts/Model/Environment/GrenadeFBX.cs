using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GrenadeFBX : MonoBehaviourPun
{
    public float effectLifeTime = 2f;

    void Update()
    {
        if (photonView.IsMine)
        {
            effectLifeTime -= Time.deltaTime;
            if (effectLifeTime <= 0.0f)
            {
                photonView.RPC("DestroyGrenadeFBX", PhotonNetwork.MasterClient);
            }
        }
    }

    [PunRPC]
    void DestroyGrenadeFBX()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
}
