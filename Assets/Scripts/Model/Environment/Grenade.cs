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
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPos, radius, 3.0f, ForceMode.Impulse);
            }
            Debug.Log(hit.name);
        }
        StartCoroutine(WaitToExplode());
        PhotonNetwork.Instantiate("PlasmaExplosionEffect", explosionPos, Quaternion.identity);
        photonView.RPC("DestroyGrenadeFBX", PhotonNetwork.MasterClient);

        //MasterManager.Instance.HandleRPC("InstantiateGrenadeFBX", explosionPos);

    }

    void InstantiateFBX(Vector3 _position)
    {
        Instantiate(effect, _position, Quaternion.identity);
    }

    IEnumerator WaitToExplode()
    {
        yield return new WaitForSeconds(delay);
    }

    [PunRPC]
    void DestroyGrenadeFBX()
    {
        PhotonNetwork.Destroy(gameObject);
    }

}