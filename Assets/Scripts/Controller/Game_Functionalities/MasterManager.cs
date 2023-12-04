using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterManager : MonoBehaviourPunCallbacks
{
    public GameManager gameManager;
    Dictionary<Player, CharacterModel> charactersDictionary = new Dictionary<Player, CharacterModel>(); //dic servidor
    Dictionary<CharacterModel, Player> clientsDictionary = new Dictionary<CharacterModel, Player>(); // dic local
    static MasterManager instance;
    //Instantiator inst;

    public static MasterManager Instance
    {
        get
        {
            return instance;
        }
    }

    //public Dictionary<Player, CharacterModel> CharactersDictionary { get => charactersDictionary; set => charactersDictionary = value; }

    private void Awake()
    {
        if (instance != null) Destroy(this);
        else instance = this;

        //inst = GetComponent<Instantiator>();

    }

    #region Clients/Models Management
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (charactersDictionary.ContainsKey(otherPlayer))
            {
                var character = charactersDictionary[otherPlayer];
                RemovePlayer(otherPlayer);
                PhotonNetwork.Destroy(character.gameObject);
            }
        }
    }
    #endregion

    #region RPC'S

    [PunRPC]
    public void RequestConanectPlayer(Player client)
    {
        GameObject go = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        var character = go.GetComponent<CharacterModel>();
        photonView.RPC("UpdatePlayer", RpcTarget.All, client, character.photonView.ViewID);
    }

    [PunRPC]
    public void UpdatePlayer(Player client, int id)
    {
        PhotonView pv = PhotonView.Find(id);
        Debug.Log("Quien es el owner? " + pv.Owner);
        var character = pv.gameObject.GetComponent<CharacterModel>();
        //Camera.main.transform.position = character.cameraHolder.transform.position;
        //Camera.main.transform.rotation = character.cameraHolder.transform.rotation;
        charactersDictionary[client] = character;
        clientsDictionary[character] = client;
        gameManager.SetManager(character);

    }

    #region Player Movement Actions RPC'S

    [PunRPC]
    public void RequestMove(Player client, float h, float v)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            character.ControlDrag();
            Vector3 dir = new Vector3(h, 0, v);
            //character.LookDir(dir);
            character.Move(h, v);
        }
    }
    [PunRPC]
    public void RequestStopMovingAnim(Player client)
    {

        if (charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            var view = character.gameObject.GetComponent<CharacterView>();
            view.Anim.RunAnimation(false);
        }
    }
    [PunRPC]
    public void RequestJumpAnim(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            var view = character.gameObject.GetComponent<CharacterView>();
            view.Anim.JumpAnimation(true);
        }
    }
    [PunRPC]
    public void RequestStopJumpAnim(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            var view = character.gameObject.GetComponent<CharacterView>();
            view.Anim.JumpAnimation(false);
        }
    }
    [PunRPC]
    public void RequestJump(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            Debug.Log("paso a master manager pidiendo jump");
            var character = charactersDictionary[client];
            //character.LookDir(dir);
            character.Jump();
        }
    }

    [PunRPC]
    public void RequestGroundCheck(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            //character.LookDir(dir);
            character.CheckGround();
        }
    }

    #endregion

    #region Shoot Player Action RPC'S

    [PunRPC]
    public void RequestShoot(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            Debug.Log("Pido shoot");
            var character = charactersDictionary[client];
            character.Shoot();
        }
    }


    [PunRPC]
    public void RequestShootAnim(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            var view = character.gameObject.GetComponent<CharacterView>();
            view.Anim.ShootAnimation(true);
        }
    }

    [PunRPC]
    public void RequestStopShootAnim(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            var view = character.gameObject.GetComponent<CharacterView>();
            view.Anim.ShootAnimation(false);
            character.CanShoot = true;

        }
    }

    [PunRPC]
    public void RequestSpawnGrenade(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            Debug.Log("Pido shoot");
            var character = charactersDictionary[client];
            character.CanShoot = false;
            character.SpawnGrenade();
            //PhotonNetwork.Instantiate("GoalPoint", sp, Quaternion.identity);
        }
    }

    //[PunRPC]
    //void RequestInstantiateGoal(Player client, Vector3 sp)
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        RPC("InstantiateGoal", client, sp);
    //    }
    //    //var sp = GetRandomSpawnpoint();
    //    //GameObject go = PhotonNetwork.Instantiate("GoalPoint", sp, Quaternion.identity);
    //}
    #endregion

    #region RPC'S Handlers

    public void RPCMaster(string name, params object[] p)
    {
        RPC(name, PhotonNetwork.MasterClient, p);
    }
    public void RPC(string name, Player target, params object[] p) //Nuevo RPC para HYBRID
    {
        photonView.RPC(name, target, p);
    }

    #endregion

    //MasterManager.Instance.RPCMaster("SetWinEvent", pv.Owner);
    //            MasterManager.Instance.RPCMaster("SetGameOverEvent", pv.Owner, pv.ViewID);

    #region Victory and Defeat events RPC

    [PunRPC]
    void SetWinEvent(Player client)
    {
        if(charactersDictionary.ContainsKey(client))
        {
            RPC("LoadWinScene", client);
        }
    }

    [PunRPC]
    void SetGameOverEvent(Player client, int id)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //RPC("LoadGameOverScene", client);

            var pv = PhotonView.Find(id);
            photonView.RPC("LoadGameOverScene", RpcTarget.All);
        }

    }

    [PunRPC]
    void LoadWinScene()
    {
        PhotonNetwork.LoadLevel("Win");
    }

    [PunRPC]
    void LoadGameOverScene()
    {
        PhotonNetwork.LoadLevel("Game_Over");
    }

    #endregion

    [PunRPC]
    void RequestInstantiateGoal(Player client, Vector3 sp)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            RPC("InstantiateGoal", client, sp);
        }
        //var sp = GetRandomSpawnpoint();
        //GameObject go = PhotonNetwork.Instantiate("GoalPoint", sp, Quaternion.identity);
    }
    [PunRPC]
    void InstantiateGoal(Vector3 sp)
    {
        GameObject go = PhotonNetwork.Instantiate("GoalPoint", sp, Quaternion.identity);
    }
    [PunRPC]
    void RequestGoalPointDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("DestroyGoalPoint", RpcTarget.All);
            //if (GameObject.FindWithTag("GoalPoint") != null)
            //{
            //    var go = GameObject.FindWithTag("GoalPoint");
            //    PhotonNetwork.Destroy(go);
            //}
        }
    }

    [PunRPC]
    void DestroyGoalPoint()
    {
        if(GameObject.FindWithTag("GoalPoint") != null)
        {
            var go = GameObject.FindWithTag("GoalPoint");
            GameObject.Destroy(go);
        }
        
    }

    #region Métodos de Remove Player/Model, hay que setearlos y eliminar al player en todos los clientes al morir
    public void RemoveModel(CharacterModel model)
    {
        if (clientsDictionary.ContainsKey(model))
        {
            var player = clientsDictionary[model];
            photonView.RPC("RequestRemovePlayer", RpcTarget.All, player);
        }
    }
    public void RemovePlayer(Player player)
    {
        photonView.RPC("RequestRemovePlayer", RpcTarget.All, player);
    }
    [PunRPC]
    public void RequestRemovePlayer(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            charactersDictionary.Remove(client);
            if (character != null)
                clientsDictionary.Remove(character);
        }
    }
    //public Player GetClientFromModel(CharacterModel model)
    //{
    //if (_dicPlayer.ContainsKey(model))
    //{
    //    return _dicPlayer[model];
    //}
    //return null;
    //}
    //public CharacterModel[] GetAllModels()
    //{
    //var characters = new CharacterModel[_dicChars.Count];
    //int count = 0;
    //foreach (var item in _dicChars)
    //{
    //    characters[count] = item.Value;
    //    count++;
    //}
    //return characters;
    //}
    //public int CountCharacters
    //{
    //get
    //{
    //    return _dicChars.Count;
    //}
    //}
    #endregion
    #endregion
}
