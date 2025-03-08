using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPosition : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private void Update()
    {
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
    }
}
