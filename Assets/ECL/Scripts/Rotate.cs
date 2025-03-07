using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float universalSpeed;
    [Space(25)]
    [SerializeField] float speedX;
    [SerializeField] float speedY;
    [SerializeField] float speedZ;
  
    void Update()
    {
        transform.Rotate(universalSpeed * speedX * Time.deltaTime, universalSpeed * speedY * Time.deltaTime, universalSpeed * speedZ * Time.deltaTime);
    }
}
