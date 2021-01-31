using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    SceneManager scene;
    public float moveSpeed = 1f;
    void Start()
    {
        scene = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>();
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(scene.player.position.x, scene.player.position.y, -10f), moveSpeed);
    }
}
