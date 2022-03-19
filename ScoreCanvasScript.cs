using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreCanvasScript : MonoBehaviour
{
    Canvas canvas;
    public Text text;
    int score;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            canvas.enabled = false;
            ResetScore();
        }
        else
            canvas.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetScore()
    {
        score = 0;
        text.text = "" + score;
    }

    public void IncreaseScore(int s)
    {
        score += s;
        text.text = "" + score;
    }

}
