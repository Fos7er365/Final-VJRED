using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWallScript : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.tag == "Player" && other.tag == "Grenade")
            {
                photonView.RPC("HandleDestroy", PhotonNetwork.MasterClient);
            }

        }
    }

    [PunRPC]
    void HandleDestroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }

}