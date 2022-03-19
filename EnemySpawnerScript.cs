using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject[] allEnemyPrefabs;

    int numSpawn;
    int numType;
    SceneManagerScript smS; 

    private void Awake()
    {
        smS = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManagerScript>();
        numSpawn = (int)(Random.Range(1f, 3f) * ((float)smS.GetLevelsWon() + 1f));
        for (int i = 0; i < numSpawn; i++)
        {
            numType = (int)Random.Range(0, allEnemyPrefabs.Length);
            Instantiate(allEnemyPrefabs[numType], new Vector3(Random.Range(-23f, 23f), 0.51f, Random.Range(-23f, 23f)), Quaternion.identity);
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().SetEnemiesRemaining(numSpawn);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
