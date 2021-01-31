using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{
    float degrees;
    void Update()
    {
        degrees += -500 * Time.deltaTime;
        transform.eulerAngles = Vector3.forward * degrees;
    }
}
