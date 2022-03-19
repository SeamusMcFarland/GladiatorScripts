using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodyPuddleScript : MonoBehaviour
{
    MeshRenderer mr;
    bool bleeding;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (bleeding && transform.localScale.x < 1f)
        {
            transform.localScale = new Vector3(transform.localScale.x + 0.01f / (transform.localScale.x * 150f), 0.05f, transform.localScale.z + 0.01f / (transform.localScale.x * 150f));
        }
    }

    public void BleedOut(Vector3 pos)
    {
        transform.localScale = new Vector3(0.01f, 0.05f, 0.01f);
        transform.position = new Vector3(pos.x + Random.Range(-0.6f, 0.6f), transform.position.y, pos.z + Random.Range(-0.6f, 0.6f));
        mr.enabled = true;
        bleeding = true;
    }

    
}
