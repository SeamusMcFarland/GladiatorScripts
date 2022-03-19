using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffectScript : MonoBehaviour
{
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = this.GetComponent<ParticleSystem>();
        ps.Stop();
    }

    public void Bleed(Vector3 posVec)
    {
        transform.position = posVec;
        ps.Play();
        StartCoroutine("EndBleed");
    }

    IEnumerator EndBleed()
    {
        yield return new WaitForSeconds(0.2f);
        ps.Stop();
    }

}
