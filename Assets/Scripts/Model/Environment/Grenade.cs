using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Grenade : MonoBehaviourPun
{
    public float delay = .5f;

    public float explosionForce = 10f;
    public float radius = 10f;
    public GameObject effect;
    public LayerMask targetLayers;
    public float grenadeLifetime = 2f;
    float explodeTimer = 0;


    private void Start()
    {
        if(photonView.IsMine)
        {
            Debug.Log("Instancio fbx explosion");
            photonView.RPC("Explode", PhotonNetwork.MasterClient);
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, radius);
    //}

    [PunRPC]
    private void Explode()
    {

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius, targetLayers);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPos, radius, 3.0f, ForceMode.Impulse);


            }
            PhotonNetwork.Instantiate("PlasmaExplosionEffect", explosionPos, Quaternion.identity);
            photonView.RPC("DestroyGrenadeGO", RpcTarget.All);
            WaitToExplode(explosionPos);
            Debug.Log(hit.name);
        }
        

    }

    void WaitToExplode(Vector3 pos)
    {
        explodeTimer += Time.deltaTime;
        if (explodeTimer > delay)
        {
            explodeTimer = 0;
        }
    }
    void InstantiateFBX(Vector3 _position)
    {
        Instantiate(effect, _position, Quaternion.identity);
    }

    [PunRPC]
    void DestroyGrenadeGO()
    {
        Destroy(gameObject, delay);
    }

}