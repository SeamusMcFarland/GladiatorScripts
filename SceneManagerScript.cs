using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    bool mainView;
    GameObject[] mainButtons;
    GameObject[] sideButtons;

    int wonLevels;
    ScoreCanvasScript scS;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        scS = GameObject.FindGameObjectWithTag("ScoreCanvas").GetComponent<ScoreCanvasScript>();
        mainView = true;
        mainButtons = GameObject.FindGameObjectsWithTag("MainViewButton");
        sideButtons = GameObject.FindGameObjectsWithTag("SideViewButton");
        foreach (GameObject o in sideButtons)
            o.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            mainButtons = GameObject.FindGameObjectsWithTag("MainViewButton");
            sideButtons = GameObject.FindGameObjectsWithTag("SideViewButton");
            foreach (GameObject o in sideButtons)
                o.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
        }
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadScene(string s)
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            wonLevels = 0;
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().GetHealth() <= 0)
            {
                scS.ResetScore();
                wonLevels = 0;
            }
            else
                wonLevels++;
        }
        SceneManager.LoadScene(s);
    }

    public void SwitchMenuView()
    {
        if (mainView)
        {
            foreach (GameObject o in sideButtons)
                o.SetActive(true);
            foreach (GameObject o in mainButtons)
                o.SetActive(false);
        }
        else
        {
            foreach (GameObject o in sideButtons)
                o.SetActive(false);
            foreach (GameObject o in mainButtons)
                o.SetActive(true);
        }
        mainView = !mainView;
    }

    public int GetLevelsWon()
    {
        return wonLevels;
    }
}
