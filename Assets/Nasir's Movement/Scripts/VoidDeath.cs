using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VoidDeath : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.y <= -10f)
        {
            Reset();
        }
    }

    private void Reset()
    {
        transform.position = new Vector3(0, 1.2f, 0);
    }
}
