using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviourPun
{
    public float speed, movementMultiplier, jumpForce;
    public float rotSpeed, rbDrag;
    public GameObject cameraHolder;
    public CharacterView view;
    Rigidbody _rb;
    GameManager gameManagerInstance;
    bool isGrounded;

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
    private void Awake()
    {
        if (photonView.IsMine)
            _rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            _rb.freezeRotation = true;
        }
    }

    public void ControlDrag()
    {
        _rb.drag = rbDrag;
    }

    public void Move(float h, float v)
    {
        view.Anim.RunAnimation(true);
        movementDirection = transform.forward * v + transform.right * h;
        _rb.AddForce(movementDirection.normalized * speed * movementMultiplier, ForceMode.Acceleration);
    }

    public void LookDir(Vector3 dir)
    {
        transform.forward = Vector3.Lerp(transform.forward, dir, rotSpeed * Time.deltaTime);
    }

    public bool CheckGround()
    {
        return isGrounded = Physics.Raycast(transform.position, -transform.up, groundDistance, groundMask);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
}

