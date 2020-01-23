using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MenuController : MonoBehaviourPunCallbacks 
{
    public CinemachineVirtualCamera menuCam;

    public CinemachineVirtualCamera gameCam;

    public ParticleSystem particles1;

    public ParticleSystem particles2;

    public KartController player;

    public GameObject playerGameObject;

    public GameObject spawnPoint;
    private bool connected = false;

    public List<GameObject> plots;
    private bool myRoom = false;

    private void Start()
    {
        menuCam.gameObject.SetActive(true);
        gameCam.gameObject.SetActive(false);
        player.enabled = false;
        player.SetPlayerMaterial();
        particles1.Play();
        particles2.Play();
        PhotonNetwork.AddCallbackTarget(this);
        Debug.Log(spawnPoint);
        Connect();
    }

    public void Play()
    {
        Debug.Log("Play");
        if (!connected) return;
        Debug.Log("Connected");
        menuCam.gameObject.SetActive(false);
        gameCam.gameObject.SetActive(true);
        player.enabled = true;
        particles1.Stop();
        particles2.Stop();
        Debug.Log("Trying to join room...");
        Connect();
    }
    
    public override void OnConnected()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnected() was called by PUN");
    }
    
    public override void OnConnectedToMaster()
    {
        connected = true;
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions());
        myRoom = true;
    }

    public override void OnJoinedRoom()
    {
        Destroy(playerGameObject);
        playerGameObject = PhotonNetwork.Instantiate(Path.Combine("Player"), spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
        player = playerGameObject.GetComponentInChildren<KartController>();
        player.enabled = true;
        player.SetPlayerMaterial();
        gameCam.Follow = player.transform;
        gameCam.LookAt = player.transform;
        Debug.Log("Player instantiated");
    }
    
    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
