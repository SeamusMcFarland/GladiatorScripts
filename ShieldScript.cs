using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    Collider[] hurtboxes;

    // Start is called before the first frame update
    void Start()
    {
        hurtboxes = GetComponentsInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetGuardPosition(int n) // 0 for neutral, 1 for guarded, 2 for open
    {
        switch(n)
        {
            case 0:
                //foreach(Collider h in hurtboxes)
                    //h.enabled = true;
                transform.localPosition = new Vector3(1.08f, 0.119f, -2.53f);
                transform.localRotation = Quaternion.Euler(0, -20f, 0);
                break;
            case 1:
                //foreach (Collider h in hurtboxes)
                    //h.enabled = true;
                transform.localPosition = new Vector3(-0.71f, 0.119f, -2.53f);
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                //foreach (Collider h in hurtboxes)
                    //h.enabled = false;
                transform.localPosition = new Vector3(0.55f, 0.119f, 1.07f);
                transform.localRotation = Quaternion.Euler(0, -110f, 0);
                break;
        }
    }
}
