using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviourPun
{
    [SerializeField] CharacterModel charModel;

    float horizontalInput;
    float verticalInput;

    Animator animator;

    private void Awake()
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    Debug.Log("Es master!!!");
        //    if (Camera.main.gameObject != null) Destroy(Camera.main.gameObject);
        //    Destroy(this);
        //}
        if (!photonView.IsMine) Destroy(this);

    }

    // Start is called before the first frame update
    void Start()
    {
        //PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = false;

        //PhotonNetwork.Instantiate("VoiceObject", Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputHandler();
    }

    public void InputHandler()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical"); 
        
        Vector3 dir = new Vector3(horizontalInput, 0, verticalInput).normalized;
        if (dir != Vector3.zero)
        {
            //charModel.LookDir(dir);
            //charModel.ControlDrag();
            charModel.Move(horizontalInput, verticalInput);
        }
    }


}
