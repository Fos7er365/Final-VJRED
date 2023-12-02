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
                Debug.Log("Colisiono con" + pv.Owner);
                MasterManager.Instance.RPC("SetWinEvent", pv.Owner);
                MasterManager.Instance.RPC("SetGameOverEvent", pv.Owner, pv.ViewID);
                PhotonNetwork.Destroy(gameObject);

            }
            //else photonView.RPC("Destroy", photonView.Owner);
            else PhotonNetwork.Destroy(gameObject);
        }
    }

}
