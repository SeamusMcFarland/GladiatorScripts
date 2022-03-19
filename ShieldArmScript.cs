using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldArmScript : MonoBehaviour
{
    float health;
    bool lost;
    AudioControllerScript acS;

    public GameObject goreO;
    public GameObject goreO2;

    Collider[] allColliders;

    // Start is called before the first frame update
    void Start()
    {
        allColliders = GetComponentsInChildren<Collider>();

        if (goreO)
            goreO.SetActive(false);
        if(goreO2)
            goreO2.SetActive(false);
        health = 4;
        acS = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Wound(float damage, PlayerScript ps)
    {
        health -= damage;
        if (health <= 0)
        {
            foreach (Collider co in allColliders)
                ps.RemoveFromHitbox(co.gameObject);
            Lost();
        }
    }

    public void Wound(float damage, EnemyScript es)
    {
        health -= damage;
        if (health <= 0)
        {
            foreach (Collider co in allColliders)
                es.RemoveFromHitbox(co.gameObject);
            Lost();
        }
    }

    private void Lost()
    {
        if (!lost)
        {
            acS.PlaySound("LostArm");
            if (goreO)
                goreO.SetActive(true);
            if (goreO2)
            {
                goreO2.SetActive(true);
                goreO2.transform.position = transform.position;
            }
            lost = true;
            this.gameObject.SetActive(false);
        }
    }

}
