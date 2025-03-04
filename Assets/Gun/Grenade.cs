using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float grenadeFuse = 5;
    public void StartFuse()
    {
        StartCoroutine(Fuse(grenadeFuse));
    }
    IEnumerator Fuse(float fuseLength)
    {
        Debug.Log("Works");
        yield return new WaitForSeconds(fuseLength);
        Destroy(gameObject);
    }
}
