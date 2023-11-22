using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI gameTimer;

    float timeLeft = 200, syncTimer = 0, timeToSync = 3f;

    bool isGameStarted;
    bool isVictory = false, isDefeat = false;

    GameObject[] goalPointSpawnSeeds;
    //Instantiator charInstantiator;

    #region Singleton

    public GameManager GameManagerInstance { get; private set; }

    #endregion;

    private void Awake()
    {

        if (GameManagerInstance != null && GameManagerInstance != this)
        {
            Destroy(this);
        }
        else
        {
            if (PhotonNetwork.IsMasterClient && Camera.main.gameObject != null) Destroy(Camera.main.gameObject);
            GameManagerInstance = this;
            //gameTimer.enabled = false;
            //charInstantiator = GetComponent<Instantiator>();
        }
    }

    private void Start()
    {
    }
   
    private void Update()
    {
        if (isGameStarted)
        {
            UpdateGameTimer();

            if (PhotonNetwork.IsMasterClient) WaitToSync();
        }
    }
    public void SetManager(CharacterModel model)
    {
        model.SetManager = this;
    }
    [PunRPC]
    public void StartGame()
    {
        //isGameStarted = true;
        //gameTimer.enabled = true;

    }
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

    #region RPC's

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

    #endregion
}
