using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : Photon.MonoBehaviour
{
    public bool showDebug = false;

    public Transform playerPrefab;
    //public PlayersSpawnManager spawnManager;
    public CameraController thirdPersonCamera;

    public static Transform localPlayer;
    //public static List<Health> players = new List<Health>();

    public Transform[] spawnPoints = null;

    public static void AddPlayer(Transform tra)
    {
        //players.Add(tra.GetComponent<Health>());
    }
    public static void RemovePlayer(Transform tra)
    {
        //players.Remove(tra.GetComponent<Health>());
    }

    void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex < 1)
        {
            Debug.LogError("Configuration error: You have not yet added any scenes to your buildsettings. The current scene should be preceded by the mainmenu scene. Please see the README file for instructions on setting up the buildsettings.");
            return;
        }

        PhotonNetwork.sendRateOnSerialize = 20;
        //PhotonNetwork.logLevel = NetworkLogLevel.Full;

        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected || PhotonNetwork.room == null)
        {
            SceneManager.LoadScene(0);
            return;
        }

        PhotonNetwork.isMessageQueueRunning = true;
        //Spawn our local player
        Vector3 spawnPosition = spawnPoints[PhotonNetwork.playerList.Length - 1].position;
        GameObject GO = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity, 0);
        GO.GetComponent<PlayerMoveController>().SetInverted(PhotonNetwork.playerList.Length == 2);
        localPlayer = GO.transform;
        thirdPersonCamera.SetTarget(GO.transform);
    }

    void OnGUI()
    {
        GameGUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            showDebug = !showDebug;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Check if the game is allowed to click here (for starting FIRE etc.)
    /// </summary>
    /// <returns></returns>
    public static bool GameCanClickHere()
    {

        List<Rect> rects = new List<Rect>();
        rects.Add(new Rect(0, 0, 110, 55));//Topleft Button
        rects.Add(new Rect(0, Screen.height - 35, 275, 35));//Chat

        Vector2 pos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        foreach (Rect re in rects)
        {
            if (re.Contains(pos))
                return false;
        }
        return true;

    }

    void GameGUI()
    {
        if (GUILayout.Button("Leave", GUILayout.Height(50), GUILayout.Width(150)))
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (showDebug)
        {
            GUILayout.Space(32);
            GUILayout.Label("isMasterClient: " + PhotonNetwork.isMasterClient);
        }
    }
}
