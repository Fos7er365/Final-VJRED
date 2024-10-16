using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;

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
        PhotonNetwork.Instantiate("Voice Object", Vector3.zero, Quaternion.identity);
        PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = false;
    }
    private void Update()
    {

        MasterManager.Instance.RequestGroundCheck(PhotonNetwork.LocalPlayer); //reproducir desde master client desde el model
        
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v).normalized;
        CheckMovement(dir);
        CheckJump();

        if (Input.GetKeyDown(KeyCode.T))
        {
            PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = false;
        }
        CheckShoot();
    }

    void CheckMovement(Vector3 dir)
    {
        if (dir != Vector3.zero)  MasterManager.Instance.RequestMove(PhotonNetwork.LocalPlayer, dir.x, dir.z);
        else MasterManager.Instance.RequestStopMovingAnim(PhotonNetwork.LocalPlayer);
    }

    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Intento Jump");
            MasterManager.Instance.RequestJump(PhotonNetwork.LocalPlayer);
            MasterManager.Instance.RequestJumpAnim(PhotonNetwork.LocalPlayer);
            StartCoroutine(WaitToDisableJumpAnim());
        }

    }

    IEnumerator WaitToDisableJumpAnim()
    {
        yield return new WaitForSeconds(1f);
        MasterManager.Instance.RequestStopJumpAnim(PhotonNetwork.LocalPlayer);
    }

    void CheckShoot()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Request shoot");

            MasterManager.Instance.Shoot(PhotonNetwork.LocalPlayer);
            MasterManager.Instance.ShootAnim(PhotonNetwork.LocalPlayer);

            //MasterManager.Instance.RPCMaster("Shoot", PhotonNetwork.LocalPlayer);
            //MasterManager.Instance.RPCMaster("ShootAnim", PhotonNetwork.LocalPlayer);

            //MasterManager.Instance.RequestShootAnim(/*"RequestShootAnim",*/ PhotonNetwork.LocalPlayer);
            //MasterManager.Instance.RequestShoot(/*"RequestShoot",*/ PhotonNetwork.LocalPlayer);
            StartCoroutine(WaitToDisableShootAnim());

            //MasterManager.Instance.RPCMaster("StopShootAnim", PhotonNetwork.LocalPlayer);
            //MasterManager.Instance.RequestStopShootAnim(/*"RequestStopShootAnim",*/ PhotonNetwork.LocalPlayer);
        }
    }

    IEnumerator WaitToDisableShootAnim()
    {
        yield return new WaitForSeconds(1f);
        MasterManager.Instance.StopShootAnim(PhotonNetwork.LocalPlayer);

    }
}
