using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float universalSpeed;
    [SerializeField] bool canSpin = true;
    [Space(25)]
    [SerializeField] float speedX;
    [SerializeField] float speedY;
    [SerializeField] float speedZ;
  
    private void Update()
    {
        if (canSpin)
        {
            gameObject.transform.Rotate(universalSpeed * speedX * Time.deltaTime, universalSpeed * speedY * Time.deltaTime, universalSpeed * speedZ * Time.deltaTime);
        }
    }
}