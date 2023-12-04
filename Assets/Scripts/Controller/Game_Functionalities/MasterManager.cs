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
    Dictionary<Player, CharacterModel> charactersDictionary = new Dictionary<Player, CharacterModel>();
    Dictionary<CharacterModel, Player> clientsDictionary = new Dictionary<CharacterModel, Player>();
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
    [PunRPC]
    public void RequestMove(Player client, float h, float v)
    {
        if(charactersDictionary.ContainsKey(client))
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
    public void RequestGroundCheck(Player client)
    {
        if (charactersDictionary.ContainsKey(client))
        {
            var character = charactersDictionary[client];
            //character.LookDir(dir);
            character.CheckGround();
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
    public void RPCMaster(string name, params object[] p)
    {
        RPC(name, PhotonNetwork.MasterClient, p);
    }
    public void RPC(string name, Player target, params object[] p) //Nuevo RPC para HYBRID
    {
        photonView.RPC(name, target, p);
    }

    [PunRPC]
    void SetWinEvent(Player client)
    {
            //RPC("LoadWinScene")
    }

    [PunRPC]
    void SetGameOverEvent()
    {
        
    }

    [PunRPC]
    void LoadWinScene()
    {
        SceneManager.LoadScene("Win");
    }

    [PunRPC]
    void LoadGameOverScene()
    {
        SceneManager.LoadScene("Game_Over");
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

    #region Métodos de Remove Player/Model, hay que setearlos y eliminar al player en todos los clientes al morir
    //public void RemoveModel(CharacterModel model)
    //{
    //if (_dicPlayer.ContainsKey(model))
    //{
    //    var player = _dicPlayer[model];
    //    photonView.RPC("RequestRemovePlayer", RpcTarget.All, player);
    //}
    //}
    public void RemovePlayer(Player player)
    {
        //photonView.RPC("RequestRemovePlayer", RpcTarget.All, player);
    }
    [PunRPC]
    public void RequestRemovePlayer(Player client)
    {
        //if (_dicChars.ContainsKey(client))
        //{
        //    var character = _dicChars[client];
        //    _dicChars.Remove(client);
        //    if (character != null)
        //        _dicPlayer.Remove(character);
        //}
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
