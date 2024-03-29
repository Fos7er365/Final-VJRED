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

    private void Awake()
    {
        if (instance != null) Destroy(this);
        else instance = this;

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
                //RemoveModel(character);
                //PhotonNetwork.Destroy(character.gameObject);
            }
        }
    }
    #endregion

    #region RPC'S

    [PunRPC]
    public void RequestConnectPlayer(Player client)
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
    public void RequestSetCameraPointerPosition(Player client, Vector3 position)
    {
        if(charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            character.ShootPosition.position = position;
        }
    }

    [PunRPC]
    public void RequestShoot(Player client)
    {
       if(PhotonNetwork.IsMasterClient)
        {
            RPC("Shoot", client);
        }
    }

    [PunRPC]
    public void Shoot(Player client)
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
        if (PhotonNetwork.IsMasterClient)
        {
            RPC("ShootAnim", client);
        }
    }

    [PunRPC]
    public void ShootAnim(Player client)
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
        if (PhotonNetwork.IsMasterClient)
        {
            RPC("StopShootAnim", client);
        }
    }

    [PunRPC]
    public void StopShootAnim(Player client)
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

    #region Victory and Defeat events RPC

    [PunRPC]
    public void SetWinEvent(Player client)
    {
        if(charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            RPC("LoadWinScene", client);
            Destroy(character.gameObject);
            RemovePlayer(client);
        }
    }

    [PunRPC]
    public void HandleGameOverEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("DestroyGoalPoint", RpcTarget.All);
            //photonView.RPC("LoadGameOverScene", RpcTarget.Others);
            HandleTimerGameOverEvent();
        }
    }

    [PunRPC]
    public void HandleTimerGameOverEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            var playersList = PhotonNetwork.PlayerList;
            var players = PhotonNetwork.CurrentRoom.Players;

            foreach (var p in playersList)
            {
                if (charactersDictionary.ContainsKey(p))
                {
                    var character = charactersDictionary[p];
                    photonView.RPC("LoadGameOverScene", p);
                    Destroy(character.gameObject);
                    RemovePlayer(p);
                }
            }
        }

    }

    [PunRPC]
    public void LoadWinScene()
    {
        PhotonNetwork.LoadLevel("Win");
    }

    [PunRPC]
   public  void LoadGameOverScene()
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
    }
    [PunRPC]
    void InstantiateGoal(Vector3 sp)
    {
        GameObject go = PhotonNetwork.Instantiate("GoalPoint", sp, Quaternion.identity);
    }
   

    [PunRPC]
    void DestroyGoalPoint()
    {
        if(GameObject.FindWithTag("GoalPoint") != null)
        {
            var go = GameObject.FindWithTag("GoalPoint");
            GameObject.Destroy(go, 1f);
        }
        
    }

    [PunRPC]
    public void GetPlayerID(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            Debug.Log("accedo a cheat");
            var character = charactersDictionary[client].gameObject.GetComponent<CharacterModel>();
            character.speed = 10000;
           
        }       
    }

    #region M�todos de Remove Player/Model, hay que setearlos y eliminar al player en todos los clientes al morir
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
            //PhotonNetwork.Destroy(character.gameObject);
            charactersDictionary.Remove(client);
            if (character != null)
                clientsDictionary.Remove(character);
        }
    }
    #endregion

    #region Unused RPC's

    //[PunRPC]
    //void SetGameOverEvent(Player client, int id) //Not used
    //{
    //    if (charactersDictionary.ContainsKey(client))
    //    {
    //        //RPC("LoadGameOverScene", client);

    //        var pv = PhotonView.Find(id);
    //        pv.RPC("LoadGameOverScene", RpcTarget.Others);
    //    }

    //}

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
