using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//演出效果（攻击动画、音效）
public class CS_Effect : MonoBehaviour 
{
    [SerializeField] float myKillTime = -1f;
    private float myTimer = -1f;
    [SerializeField] AudioSource myAudioSource = null;
    [SerializeField] float myAudioDelay = 0f;

    private void OnEnable () {
        if (myKillTime > 0) {
            myTimer = myKillTime;
        }

        if (myAudioSource != null) {
            myAudioSource.PlayDelayed (myAudioDelay);
        }
    }

    private void Update () {
        if (myTimer < 0) {
            return;
        }

        myTimer -= Time.deltaTime;
        if (myTimer <= 0) {
            myTimer = -1;
            Kill ();
        }
    }

    public void Kill () {
        this.gameObject.SetActive (false);
    }
}
