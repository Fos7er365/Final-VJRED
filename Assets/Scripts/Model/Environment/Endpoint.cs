using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endpoint : MonoBehaviourPun
{

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            Debug.Log("BUENAS!");
            if (other.tag == "Player")
            {
                var pv = other.GetComponent<PhotonView>();
                MasterManager.Instance.RPCMaster("WinGame", pv.Owner);
                MasterManager.Instance.RPCMaster("LoseGame", RpcTarget.Others);
                //photonView.RPC("DestroyBullet", photonView.Owner);
                PhotonNetwork.Destroy(gameObject);
            }
            //else photonView.RPC("Destroy", photonView.Owner);
            else PhotonNetwork.Destroy(gameObject);
        }
    }

}
