using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    Vector3 savedLocalPos;
    int typeHacking; // 0 for none, 1 for left, 2 for right, 3 for middle
    int attackType; // 1 for left, 2 for right, 3 for middle
    bool raised;
    public WeaponScript weaponS;

    public bool isPlayer;

    float frameRegulator;

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

        if (goreO != null)
            goreO.SetActive(false);
        if (goreO2 != null)
            goreO2.SetActive(false);
        switch (weaponS.GetWeaponType())
        {
            case 0:
                health = 6f;
                break;
            case 1:
                health = 5f;
                break;
            case 2:
                health = 2.5f;
                break;
            case 3:
                health = 5f;
                break;
        }
       
        savedLocalPos = transform.localPosition;
        acS = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        frameRegulator = Time.deltaTime / 0.015f;
        if (!lost)
        {
            CheckHack();
        }
    }

    private void CheckHack()
    {
        switch (typeHacking)
        {
            case 0:
                break;
            case 1:
                transform.localPosition = new Vector3(transform.localPosition.x + 0.08f * frameRegulator, transform.localPosition.y - 0.012f * frameRegulator, transform.localPosition.z + 0.008f * frameRegulator);
                if (transform.localPosition.x > 0.9f)
                    ResetArm();
                break;
            case 2:
                transform.localPosition = new Vector3(transform.localPosition.x - 0.08f * frameRegulator, transform.localPosition.y - 0.012f * frameRegulator, transform.localPosition.z - 0.008f * frameRegulator);
                if (transform.localPosition.x < -0.7f)
                    ResetArm();
                break;
            case 3:
                transform.localPosition = new Vector3(transform.localPosition.x - 0.002f * frameRegulator, transform.localPosition.y - 0.002f * frameRegulator, transform.localPosition.z + 0.06f * frameRegulator);
                if (transform.localPosition.z > 1.3f)
                    ResetArm();
                break;
        }
    }

    public void RaiseHack(int at) // 1 for left, 2 for right, 3 for middle
    {
        if (!raised && typeHacking == 0)
        {
            attackType = at;
            raised = true;
            switch (at)
            {
                case 1:
                    if(isPlayer)
                        transform.localRotation = Quaternion.Euler(80f, -70f, 0);
                    else
                        transform.localRotation = Quaternion.Euler(80f, -70f, 180f);
                    if (isPlayer)
                        transform.localPosition = new Vector3(-0.3f, -0.135f, 0.6f);
                    else
                        transform.localPosition = new Vector3(-0.3f, 0.32f, 0.6f);
                    break;
                case 2:
                    if(isPlayer)
                        transform.localRotation = Quaternion.Euler(70f, 40f, 0);
                    else
                        transform.localRotation = Quaternion.Euler(70f, 40f, 180f);
                    if (isPlayer)
                        transform.localPosition = new Vector3(0.5f, -0.135f, 0.6f);
                    else
                        transform.localPosition = new Vector3(0.5f, 0.32f, 0.6f);
                    break;
                case 3:
                    if(isPlayer)
                        transform.localRotation = Quaternion.Euler(80f, -10f, 0);
                    else
                        transform.localRotation = Quaternion.Euler(80f, -10f, 180f);
                    if (isPlayer)
                        transform.localPosition = new Vector3(0.37f, -0.267f, 0.224f);
                    else
                        transform.localPosition = new Vector3(0.37f, 0.28f, 0.5f);
                    break;
            }
        }
    }

    public void Hack()
    {
        weaponS.Activate();
        if (attackType == 1)
            typeHacking = 1;
        else if (attackType == 2)
            typeHacking = 2;
        if (attackType == 3)
            typeHacking = 3;
        raised = false;
    }

    public void ResetArm()
    {
        weaponS.Deactivate();
        raised = false;
        typeHacking = 0;
        transform.localPosition = savedLocalPos;
        if(isPlayer)
            transform.localRotation = Quaternion.Euler(90f, 0, 0);
        else
            transform.localRotation = Quaternion.Euler(90f, 0, 180f);
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
            lost = true;
            if (goreO != null)
                goreO.SetActive(true);
            if (goreO2 != null)
            {
                goreO2.SetActive(true);
                goreO2.transform.position = transform.position;
            }
            acS.PlaySound("LostArm");
            this.gameObject.SetActive(false);
        }
    }

    public void ForwardRemoveFromCollider(GameObject o)
    {
        weaponS.ForwardRemoveFromCollider(o);
    }

    public bool GetRaised()
    {
        return raised;
    }

    public int GetTypeHacking()
    {
        return typeHacking;
    }

    public int GetAttackType()
    {
        return attackType;
    }

    public bool GetLost()
    {
        return lost;
    }
}
