using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPosition : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Camera fpsCam;

    private void Update()
    {
        Vector3 bOffset = -player.transform.forward * .15f;
        transform.position = new(player.transform.position.x + bOffset.x, player.transform.position.y, player.transform.position.z + bOffset.z);
        transform.rotation = Quaternion.Euler(0f, fpsCam.transform.eulerAngles.y, 0f);
    }
}
