using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviourPun
{
    [Header("References")]

    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    Transform cam = null;
    [SerializeField] Transform orientation = null;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    float cursorFixTimer;
    [SerializeField]float cursorFixMaxTimer;

    public Transform Cam { get => cam; set => cam = value; }

    private void Awake()
    {

        if (PhotonNetwork.IsMasterClient) Destroy(this);
    }
    private void Start()
    {
        if (photonView.IsMine)
        {
            cam = Camera.main.gameObject.transform; 
            cam.position = orientation.position;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            WaitToSendPositionToMaster();
            cam.position = orientation.position; //Era transform position
            cam.transform.forward = orientation.transform.forward;
            cam.transform.rotation = orientation.transform.rotation;

            mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            xRotation -= mouseY; // * sensY * multiplier;
            yRotation += mouseX; // * sensX * multiplier;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);

            cam.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
            orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            cam.Rotate(Vector3.up * mouseX);

            ////yRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        }
    }

    public void WaitToSendPositionToMaster()
    {
        cursorFixTimer += Time.deltaTime;
        if (cursorFixTimer >= cursorFixMaxTimer)
        {
            MasterManager.Instance.RPCMaster("RequestSetCameraPointerPosition", PhotonNetwork.LocalPlayer, cam.position);
            cursorFixTimer = 0;
        }
    }



}
