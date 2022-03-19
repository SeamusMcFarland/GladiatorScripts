using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseCanvasScript : MonoBehaviour
{

    public GameObject winButton;
    public GameObject loseButton;
    public GameObject menuButton;

    SceneManagerScript smS;

    // Start is called before the first frame update
    void Start()
    {
        smS = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManagerScript>();

        winButton.SetActive(false);
        loseButton.SetActive(false);
        menuButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateButtons(bool won)
    {
        StartCoroutine("DelayActivateButtons", won);
    }

    IEnumerator DelayActivateButtons(bool w)
    {
        yield return new WaitForSeconds(1f);
        if (w)
            winButton.SetActive(true);
        else
            loseButton.SetActive(true);
        menuButton.SetActive(true);
    }

    public void ForwardRetry()
    {
        smS.LoadScene("Arena");
    }

    public void ForwardContinue()
    {
        smS.LoadScene("Arena");
    }

    public void ForwardMenu()
    {
        smS.LoadScene("Menu");
    }
}
