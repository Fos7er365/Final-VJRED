using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI gameTimer;
    [SerializeField] Transform[] goalPointSpawnSeeds;

    [SerializeField] float timeLeft = 200;
    float syncTimer = 0, timeToSync = 3f;

    bool isGameStarted;
    bool isVictory = false, isDefeat = false;

    //Instantiator charInstantiator;

    #region Singleton

    public GameManager GameManagerInstance { get; private set; }

    #endregion;

    private void Awake()
    {

        if (GameManagerInstance != null && GameManagerInstance != this) Destroy(this);
        else
        {
            if (PhotonNetwork.IsMasterClient && Camera.main.gameObject != null) Destroy(Camera.main.gameObject);
            GameManagerInstance = this;
            gameTimer.enabled = false;
            //charInstantiator = GetComponent<Instantiator>();
        }
    }

    private void Update()
    {
        if (isGameStarted)
        {
            UpdateGameTimer();

            if (PhotonNetwork.IsMasterClient)
            {
                WaitToSync();
                CheckDefeatByTimeOut(timeLeft);
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("Cant de players: " + playerCount);
        gameTimer.text = timeLeft.ToString();
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Chequeo juego iniciado");
            if (!isGameStarted && playerCount > PhotonNetwork.CurrentRoom.MaxPlayers - 1)
            {
                Debug.Log("Inicio juego");
                photonView.RPC("StartGame", RpcTarget.All);
                var sp = GetRandomSpawnpoint();
                MasterManager.Instance.RPCMaster("RequestInstantiateGoal", PhotonNetwork.LocalPlayer, sp.position);//Instanciar directamente desde el servidor
            }
        }


    }
  
    public Transform GetRandomSpawnpoint()
    {
        return goalPointSpawnSeeds[UnityEngine.Random.Range(0, goalPointSpawnSeeds.Length - 1)];
    }
    public void SetManager(CharacterModel model)
    {
        model.SetManager = this;
    }
    [PunRPC]
    public void StartGame()
    {
        isGameStarted = true;
        gameTimer.enabled = true;

    }

    #region Timer handling methods

    void UpdateGameTimer()
    {

        gameTimer.enabled = true;
        timeLeft -= Time.deltaTime;
        
        HandleGameTimer(timeLeft);
    }
    public void HandleGameTimer(float currentTime)
    {
        currentTime += 1;
        var minutes = Mathf.FloorToInt(currentTime / 60);
        var seconds = Mathf.FloorToInt(currentTime % 60);
        gameTimer.text = String.Format("{0:00}:{1:00} ", minutes, seconds);
    }
    void WaitToSync()
    {
        syncTimer += Time.deltaTime;
        if (syncTimer >= timeToSync)
        {
            photonView.RPC("SetTimerFix", RpcTarget.Others, timeLeft);
            syncTimer = 0;
        }
    }
    [PunRPC]
    public void SetTimerFix(float _timer)
    {
        timeLeft = _timer;
    }
    void CheckDefeatByTimeOut(float _timeLeft)
    {
        if(_timeLeft <= 0)
        {
            MasterManager.Instance.HandleTimerGameOverEvent();
        }

    }
    #endregion

    #region RPC's

    #endregion
}
