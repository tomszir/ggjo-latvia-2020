using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public Transform player;
    public Transform projectiles;
    public Transform cam;
    public Transform enemies;
    public AudioClip winSound;
    private bool win = false;
    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        projectiles = GameObject.FindGameObjectWithTag("Projectiles").transform;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        enemies = GameObject.FindGameObjectWithTag("Enemies").transform;
    }

    void Update(){
        if(enemies.childCount <= 1 && !win) {
            StartCoroutine(TurnUp());
            win = true;
        }
    }

    public void Died() {
        StartCoroutine(TurnDown());
    }

    public IEnumerator TurnDown(){
        ColorAdjustments b;
        Vignette v;
        cam.GetComponent<Volume>().profile.TryGet(out b);
        cam.GetComponent<Volume>().profile.TryGet(out v);
        for(int i = 0; i < 10; i++) {
            if(b) b.saturation.value = -10f * (i+1);
            if(v) v.intensity.value = .01f * (i+1);
            GetComponent<AudioSource>().pitch = .04f * (i+1);
            GetComponent<AudioSource>().volume -= .0005f * (i+1);
            yield return new WaitForSecondsRealtime(.1f);
        }
        yield return new WaitForSecondsRealtime(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator TurnUp(){
        Bloom b;
        PaniniProjection p;
        Vignette v;
        cam.GetComponent<Volume>().profile.TryGet(out b);
        cam.GetComponent<Volume>().profile.TryGet(out v);
        cam.GetComponent<Volume>().profile.TryGet(out p);
        for(int i = 0; i < 80; i++) {
            if(b) b.intensity.value = .1f * (i+1);
            if(p) p.distance.value = .01f * (i+1);
            if(v) v.intensity.value = .001f * (i+1);
            GetComponent<AudioSource>().pitch += .0001f * (i+1);
            GetComponent<AudioSource>().volume -= .00001f * (i+1);
            yield return new WaitForSecondsRealtime(.01f);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
