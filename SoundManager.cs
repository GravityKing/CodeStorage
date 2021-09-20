using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;

    public AudioClip hitEnemyBullet;
    public AudioClip enemyBomb;
    public AudioClip hitTankBullet;
    public AudioClip hitBossBullet;
    public AudioClip hitBossMissile;
    public AudioClip laserCharging;
    public AudioClip buttonClick;
    public AudioClip[] bossDie;
    void Start()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

    }
    public void SetClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Stop();
        audioSource.Play();
    }

    public void SetClip(AudioClip clip,float volume)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Stop();
        audioSource.Play();
    }

    public IEnumerator IESetClipBossDie()
    {
        for (int i = 0; i < bossDie.Length; i++)
        {
            audioSource.clip = bossDie[i];
            audioSource.volume = 0.4f;
            audioSource.Stop();
            audioSource.Play();

            yield return new WaitForSeconds(1f);
        }
    }

    public void SetButtonClip()
    {
        audioSource.clip = buttonClick;
        audioSource.Stop();
        audioSource.Play();
    }

}
