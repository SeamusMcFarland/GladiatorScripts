using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    bool active;

    public int weaponType;
    float weaponDamage;
    float baseWeaponDamage;
    float swingModifier;

    SparkEffectScript seS;

    AudioControllerScript acS;

    List<GameObject> allTargets = new List<GameObject>();
    List<GameObject> allLightArmor = new List<GameObject>();
    List<GameObject> allHeavyArmor = new List<GameObject>();
    List<GameObject> allArms = new List<GameObject>();

    List<GameObject> noHit = new List<GameObject>();
    bool validHit;
    GameObject theArm;
    ArmScript armS;
    ShieldArmScript shieldArmS;

    public GameObject aimObject;

    Rigidbody rb;

    public Transform tempParent;
    public ArmScript ownArmS;
    EnemyScript enemyS;
    PlayerScript playerS;

    bool ignoresHeavy;

    AudioSource eSource;

    // Start is called before the first frame update
    void Start()
    {
        if(tempParent.tag == "Enemy")
            eSource = tempParent.GetComponentInChildren<AudioSource>();

        if (tempParent.tag == "Player")
            playerS = tempParent.GetComponent<PlayerScript>();
        else if (tempParent.tag == "Enemy")
            enemyS = tempParent.GetComponent<EnemyScript>();

        rb = GetComponent<Rigidbody>();
        foreach (Transform child in tempParent)
            noHit.Add(child.gameObject);
        noHit.Add(tempParent.gameObject);


        acS = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioControllerScript>();
        seS = GameObject.FindGameObjectWithTag("SparkEffect").GetComponent<SparkEffectScript>();

        switch (weaponType)
        {
            case 0:  // gladius
                baseWeaponDamage = 1.0f;
                swingModifier = 1.1f;
                break;
            case 1:  // trident
                baseWeaponDamage = 0.5f;
                swingModifier = 0.5f;
                break;
            case 2:  // andabata gladius
                baseWeaponDamage = 0.3f;
                swingModifier = 1.1f;
                break;
            case 3:  // Sica sword
                baseWeaponDamage = 0.8f;
                swingModifier = 1.1f;
                ignoresHeavy = true;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        weaponDamage = baseWeaponDamage * swingModifier;

        //if(transform.parent != null && transform.parent.name == "RetiariusPrefab")
            //print("alltargets: " + allTargets.Count + " light armor: " + allLightArmor.Count + " heavy armor:" + allHeavyArmor.Count);
        if (active && allTargets.Count > 0)
        {
            if (allLightArmor.Count < 1 && allHeavyArmor.Count < 1)
            {
                if (allTargets[0].CompareTag("Enemy"))
                    allTargets[0].GetComponentInParent<EnemyScript>().Wound(weaponDamage);
                else if (allTargets[0].CompareTag("Player"))
                    allTargets[0].GetComponentInParent<PlayerScript>().Wound(weaponDamage);
                else if (allTargets[0].transform.parent.CompareTag("Enemy"))
                    allTargets[0].GetComponentInParent<EnemyScript>().Wound(weaponDamage);
                else if (allTargets[0].transform.parent.CompareTag("Player"))
                    allTargets[0].GetComponentInParent<PlayerScript>().Wound(weaponDamage);

                CheckArm(weaponDamage);

                if(eSource)
                    acS.PlaySound("Slice", eSource);
                else
                    acS.PlaySound("Slice");
            }
            else if (allHeavyArmor.Count < 1 || ignoresHeavy)
            {
                if (allTargets[0].CompareTag("Enemy"))
                    allTargets[0].GetComponentInParent<EnemyScript>().Wound(weaponDamage / 2f);
                else if (allTargets[0].CompareTag("Player"))
                    allTargets[0].GetComponentInParent<PlayerScript>().Wound(weaponDamage / 2f);
                else if (allTargets[0].transform.parent.CompareTag("Enemy"))
                    allTargets[0].GetComponentInParent<EnemyScript>().Wound(weaponDamage / 2f);
                else if (allTargets[0].transform.parent.CompareTag("Player"))
                    allTargets[0].GetComponentInParent<PlayerScript>().Wound(weaponDamage / 2f);

                CheckArm(weaponDamage / 2f);

                if (eSource)
                    acS.PlaySound("HalfHit", eSource);
                else
                    acS.PlaySound("HalfHit");
            }
            else
            {
                seS.Spark(transform.position);
                if (eSource)
                    acS.PlaySound("Cling", eSource);
                else
                    acS.PlaySound("Cling");
            }
            
            Deactivate();
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(new Vector3(aimObject.transform.position.x, aimObject.transform.position.y, aimObject.transform.position.z));
        rb.MoveRotation(Quaternion.Euler(aimObject.transform.rotation.eulerAngles.x, aimObject.transform.rotation.eulerAngles.y, aimObject.transform.rotation.eulerAngles.z));
    }

    /*private void Reposition() // has the weapon hitbox (which this script is attatched to) follow the position of the weapon visual so maintain hitbox triggered integredy
    {
        if (Mathf.Abs(transform.position.x) > 100f || Mathf.Abs(transform.position.y) > 100f || Mathf.Abs(transform.position.z) > 100f)
            transform.position = new Vector3(0, 100f,0);
        rb.velocity = new Vector3((aimObject.transform.position.x - transform.position.x)/Time.deltaTime, (aimObject.transform.position.y - transform.position.y) / Time.deltaTime, (aimObject.transform.position.z - transform.position.z) / Time.deltaTime);
        rb.angularVelocity = new Vector3((aimObject.transform.rotation.x - transform.rotation.x) / Time.deltaTime, (aimObject.transform.rotation.y - transform.rotation.y) / Time.deltaTime, (aimObject.transform.rotation.z - transform.rotation.z) / Time.deltaTime);
    }*/

    private void CheckArm(float dam)
    {
        theArm = null;
        foreach (GameObject at in allTargets)
        {
            if (at.CompareTag("Arm"))
            {
                armS = at.GetComponent<ArmScript>();
                shieldArmS = at.GetComponent<ShieldArmScript>();
                break;
            }
        }
        if (armS)
        {
            if (tempParent.tag == "Player")
                armS.Wound(dam, playerS);
            else if (tempParent.tag == "Enemy")
                armS.Wound(dam, enemyS);
            else
                print("ERROR! INVALID TEMPPARENT TAG!!!");
        }
        else if (shieldArmS)
        {
            if (tempParent.tag == "Player")
                shieldArmS.Wound(dam, playerS);
            else if (tempParent.tag == "Enemy")
                shieldArmS.Wound(dam, enemyS);
            else
                print("ERROR! INVALID TEMPPARENT TAG!!!");
        }
    }

    public void Activate()
    {
        if(!ownArmS.GetLost())
            active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        validHit = GetValidHit(other.gameObject);
        if (validHit)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Player") || other.CompareTag("Arm"))
                allTargets.Add(other.gameObject);
            else if (other.CompareTag("LightArmor"))
                allLightArmor.Add(other.gameObject);
            else if (other.CompareTag("HeavyArmor"))
                allHeavyArmor.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        validHit = GetValidHit(other.gameObject);
        if (validHit)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Player") || other.CompareTag("Arm"))
                allTargets.Remove(other.gameObject);
            else if (other.CompareTag("LightArmor"))
                allLightArmor.Remove(other.gameObject);
            else if (other.CompareTag("HeavyArmor"))
                allHeavyArmor.Remove(other.gameObject);
        }
    }

    private bool GetValidHit(GameObject o) // checks if the object matches a nohit object
    {
        foreach (GameObject nh in noHit)
        {
            if(nh == o)
                return false;
        }
        return true;
    }

    public void ForwardRemoveFromCollider(GameObject other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player") || other.CompareTag("Arm"))
            allTargets.Remove(other.gameObject);
        else if (other.CompareTag("LightArmor"))
            allLightArmor.Remove(other.gameObject);
        else if (other.CompareTag("HeavyArmor"))
            allHeavyArmor.Remove(other.gameObject);
    }

    public int GetWeaponType()
    {
        return weaponType;
    }

}
