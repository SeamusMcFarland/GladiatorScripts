using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundColliderScript : MonoBehaviour
{
    PlayerScript pS;

    // Start is called before the first frame update
    void Start()
    {
        pS = GetComponentInParent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
            pS.SetGrounded(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
            pS.SetGrounded(false);
    }
}
