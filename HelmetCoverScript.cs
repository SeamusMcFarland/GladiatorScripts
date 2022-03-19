using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmetCoverScript : MonoBehaviour
{
    MeshRenderer mr;
    public Material redM;
    public Material holesM;
    int coverType;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        coverType = 0;
        switch(coverType)
        {
            case 0:
                mr.enabled = false;
                break;
            case 1:
                mr.material = holesM;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetToRed()
    {
        mr.enabled = true;
        mr.material = redM;
    }
}
