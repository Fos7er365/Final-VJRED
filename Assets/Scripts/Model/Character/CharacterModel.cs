using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviourPun
{
    public float speed, movementMultiplier, jumpForce, gravity;
    public float rotSpeed, rbDrag;
    public GameObject cameraHolder;
    public Transform orientation;
    public CharacterView view;
    Rigidbody _rb;
    GameManager gameManagerInstance;
    public bool isGrounded;
    public bool groundedTest;
    public float cd;
    float lastJump;

    [Header("Attack variables and components")]
    [SerializeField] Transform shootPosition;
    [SerializeField] GameObject grenadeLauncherGO;
    [SerializeField] GameObject grenadeGO;
    [SerializeField] float shootRange;
    PlayerLook playerLook;
    bool canShoot;


    [Header("Ground Detection")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 10f;
    //aaa
    Vector3 movementDirection;

    public GameManager SetManager
    {
        set
        {
            gameManagerInstance = value;
        }
    }

    public bool CanShoot { get => canShoot; set => canShoot = value; }
    public PlayerLook PlayerLook { get => playerLook; set => playerLook = value; }
    public Transform ShootPosition { get => shootPosition; set => shootPosition = value; }

    private void Awake()
    {
        if (photonView.IsMine)
        {
            _rb = GetComponent<Rigidbody>();
            playerLook = GetComponent<PlayerLook>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            _rb.freezeRotation = true;
            //shootPosition = playerLook.Cam.transform;
        }
    }

    public void ControlDrag()
    {
        _rb.drag = rbDrag;
    }

    public void Move(float h, float v)
    {
        view.Anim.RunAnimation(true);
        movementDirection = orientation.forward * v + orientation.right * h;
        _rb.AddForce(movementDirection.normalized * speed * movementMultiplier, ForceMode.Force);



        var CharacterRotation = orientation.transform.rotation;
        CharacterRotation.x = 0;
        CharacterRotation.z = 0;
        transform.rotation = CharacterRotation;
    }

    public void LookDir(Vector3 dir)
    {
        transform.forward = Vector3.Lerp(transform.forward, dir, rotSpeed * Time.deltaTime);
    }

    public bool CheckGround()
    {

        return isGrounded;


        //Physics.Raycast(transform.position, Vector3.down, 5f * 0.5f + 0.2f, groundMask);
    }

    public void Jump()
    {
        if (Time.time-lastJump<cd)
        {
            return;
        }
        lastJump = Time.time;
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    public void Shoot()
    {
        if(photonView.IsMine)
            MasterManager.Instance.RPCMaster("RequestSpawnGrenade", photonView.Owner);
    }

    public void SpawnGrenade()
    {
        GameObject go = PhotonNetwork.Instantiate("Grenade", shootPosition.position, Quaternion.identity);
        //PhotonNetwork.Instantiate("Grenade", spawnPoint.position, spawnPoint.rotation);
        go.GetComponent<Rigidbody>().AddForce(go.transform.forward * shootRange, ForceMode.Impulse);
        canShoot = false;
    }
}


