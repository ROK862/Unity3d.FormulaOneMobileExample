using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GamePlayMode gamePlayMode = GamePlayMode.Starting;
    [SerializeField] private float gamePlaySpeed = 1f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera gameCamera;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject playerObject;
    // Start is called before the first frame update

    [SerializeField] private Camera[] gamePlayCameras;
    private int camIndex = 0;
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePlayMode != GamePlayMode.Paused)
            Time.timeScale = gamePlaySpeed;
    }

    public void StartRace() {
        ResumeGame();
    }

    public void ResetPlayer() {
        GameObject spawnPoint = GameObject.Find("Player-Spawn-Point");
        if (playerObject != null && spawnPoint != null) {
            playerObject.transform.position = spawnPoint.transform.position;
            playerObject.transform.rotation = spawnPoint.transform.rotation;
        }
    }

    public void PauseGame()
    {
        gamePlayMode = GamePlayMode.Paused;
        Time.timeScale = 0;
        var cameraRoot = GameObject.Find("Main Camera Root");
        if (cameraRoot != null && mainMenu != null) {
            mainCamera.transform.position = cameraRoot.transform.position;
            mainCamera.transform.rotation = cameraRoot.transform.rotation;
        }
        if (gameCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            gameCamera.gameObject.SetActive(false);
        }

        if (mainMenu != null)
        {
            mainMenu.gameObject.SetActive(true);
            gameMenu.gameObject.SetActive(false);
        }
    }

     IEnumerator LoadAsyncScene(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void LoadScene(string sceneName)
    {
        // Use a coroutine to load the Scene in the background
        StartCoroutine(LoadAsyncScene(sceneName));
    }

    public void ResumeGame()
    {
        gamePlayMode = GamePlayMode.Resume;
        Time.timeScale = 1;
        if (gameCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            gameCamera.gameObject.SetActive(true);
        }
        if (mainMenu != null)
        {
            mainMenu.gameObject.SetActive(false);
            gameMenu.gameObject.SetActive(true);
        }
    }

    public void NexCamera()
    {
        if (gamePlayCameras.Length != 0)
        {
            foreach (var cam in gamePlayCameras)
            {
                cam.gameObject.SetActive(false);
            }
            camIndex++;
            if (gamePlayCameras.Length <= camIndex)
                camIndex = 0;

            gamePlayCameras[camIndex].gameObject.SetActive(true);
        }
    }
}

public enum GamePlayMode
{
    Paused,
    Resume,
    Starting
}
