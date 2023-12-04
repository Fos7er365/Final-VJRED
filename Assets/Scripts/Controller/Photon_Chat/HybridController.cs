using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class HybridController : MonoBehaviour
{

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(Camera.main != null) Destroy(Camera.main.gameObject);
            Destroy(this);
        }
    }
    private void Start()
    {
        //MasterManager.Instance.photonView.RPC("RequestConnectPlayer", PhotonNetwork.MasterClient, PhotonNetwork.LocalPlayer);
        MasterManager.Instance.RPC("RequestConnectPlayer", PhotonNetwork.LocalPlayer, PhotonNetwork.LocalPlayer);
        //MasterManager.Instance.RPC("SetCameraToPlayer", PhotonNetwork.LocalPlayer, PhotonNetwork.LocalPlayer);
    }
    private void Update()
    {

        MasterManager.Instance.RequestGroundCheck(PhotonNetwork.LocalPlayer);
        
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v).normalized;
        CheckMovement(dir);
        CheckJump();

        if (Input.GetKeyDown(KeyCode.T))
        {

        }
    }

    void CheckMovement(Vector3 dir)
    {

        if (dir != Vector3.zero)
        {
            MasterManager.Instance.RequestMove(PhotonNetwork.LocalPlayer, dir.x, dir.z);
            //MasterManager.Instance.RPC("RequestMove", PhotonNetwork.LocalPlayer, dir);
        }
        else
        {
            MasterManager.Instance.RequestStopMovingAnim(PhotonNetwork.LocalPlayer);
        }
    }

    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Intento Jump");
            MasterManager.Instance.RequestJump(PhotonNetwork.LocalPlayer);
            MasterManager.Instance.RequestJumpAnim(PhotonNetwork.LocalPlayer);
        }
        MasterManager.Instance.RequestStopJumpAnim(PhotonNetwork.LocalPlayer);

    }

    void CheckShoot()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Request shoot");
            MasterManager.Instance.RequestShoot(PhotonNetwork.LocalPlayer);
            MasterManager.Instance.RequestShootAnim(PhotonNetwork.LocalPlayer);
        }
        MasterManager.Instance.RequestStopShootAnim(PhotonNetwork.LocalPlayer);
    }
}
