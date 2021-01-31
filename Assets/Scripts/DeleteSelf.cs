using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSelf : MonoBehaviour
{

    public void Kill(float time) // In seconds
    {
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        Invoke("Disable", time);
    }

    public void Disable() {
        if (gameObject.activeInHierarchy) gameObject.SetActive(false);
    }
}
