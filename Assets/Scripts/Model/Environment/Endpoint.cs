using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endpoint : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Endpoint GO owner" + photonView.Owner);
        if (photonView.IsMine)
        {
            Debug.Log("BUENAS!");
            if (other.tag == "Player")
            {

                var pv = other.GetComponent<PhotonView>();
                Debug.Log("Colisiono con" + pv.Owner);
                //photonView.RPC("LoadGameOverScene", RpcTarget.Others);
                //SetGameOverEvent(Player client, int id)
                //MasterManager.Instance.RPCMaster("SetWinEvent", pv.Owner);
                //MasterManager.Instance.RPCMaster("RequestGoalPointDestroy");

                MasterManager.Instance.SetWinEvent(pv.Owner);
                MasterManager.Instance.HandleGameOverEvent();


            }
            //else photonView.RPC("Destroy", photonView.Owner);
            //else PhotonNetwork.Destroy(gameObject);
        }
    }
}
