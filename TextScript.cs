using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    public float activationDistance;

    MeshRenderer mr;

    public int quadrent;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (quadrent)
        {
            case 1:
                if (0.5f < Input.mousePosition.x / Screen.width && 0.5f < Input.mousePosition.y / Screen.height)
                    mr.enabled = true;
                else
                    mr.enabled = false;
                break;
            case 2:
                if (0.5f > Input.mousePosition.x / Screen.width && 0.5f < Input.mousePosition.y / Screen.height)
                    mr.enabled = true;
                else
                    mr.enabled = false;
                break;
            case 3:
                if (0.5f > Input.mousePosition.x / Screen.width && 0.5f > Input.mousePosition.y / Screen.height)
                    mr.enabled = true;
                else
                    mr.enabled = false;
                break;
            case 4:
                if (0.5f < Input.mousePosition.x / Screen.width && 0.5f > Input.mousePosition.y / Screen.height)
                    mr.enabled = true;
                else
                    mr.enabled = false;
                break;
        }

    }

}
