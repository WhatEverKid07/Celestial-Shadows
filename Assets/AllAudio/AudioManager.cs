using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource weaponsAudioSource;
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioSource playerMoveAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    [Header("Weapons")]
    [SerializeField] private AudioClip aRShoot;
    [SerializeField] private AudioClip aRReload;
    [SerializeField] private AudioClip pistolShoot;
    [SerializeField] private AudioClip shotgunShoot;
    [SerializeField] private AudioClip knifeSlash;
    [SerializeField] private AudioClip knifeHit;
    [SerializeField] private AudioClip grenadeExplode;
    [SerializeField] private AudioClip grenadePinPull;

    [Header("Player")]
    [SerializeField] private AudioClip walking;
    [SerializeField] private AudioClip running;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip dash;

    [Header("Music")]
    [SerializeField] private AudioClip normalMusic;
    [SerializeField] private AudioClip intenseMusic;


    private void Awake()
    {
        if (instance == null)
        { instance = this; DontDestroyOnLoad(gameObject); }
        else
        { Destroy(gameObject); }
    }
    public void AssaultRifleShoot()
    { weaponsAudioSource.clip = aRShoot; weaponsAudioSource.Play();}
    public void AssaultRifleReload()
    { weaponsAudioSource.clip = aRReload; weaponsAudioSource.Play(); }
    public void PistolShoot()
    { weaponsAudioSource.clip = pistolShoot; weaponsAudioSource.Play(); }
    public void ShotgunShoot()
    { weaponsAudioSource.clip = shotgunShoot; weaponsAudioSource.Play(); }
    public void KnifeSlash()
    { weaponsAudioSource.clip = knifeSlash; weaponsAudioSource.Play(); }
    public void KnifeHit()
    { weaponsAudioSource.clip = knifeHit; weaponsAudioSource.Play(); }
    public void GrenadeExplode()
    { weaponsAudioSource.clip = grenadeExplode; weaponsAudioSource.Play(); }
    public void GrenadePinPull()
    { weaponsAudioSource.clip = grenadePinPull; weaponsAudioSource.Play(); }
    public void PlayerJump()
    { playerAudioSource.clip = jump; playerAudioSource.Play(); }
    public void PlayerDash()
    { playerAudioSource.clip = dash; playerAudioSource.Play(); }

    public void PlayerWalking(bool isMoving)
    {
        playerMoveAudioSource.clip = walking;

        if (isMoving)
        {
            if (!playerMoveAudioSource.isPlaying)
            {
                playerMoveAudioSource.Play();
            }
        }
        else
        {
            if (playerMoveAudioSource.isPlaying)
            {
                playerMoveAudioSource.Stop();
            }
        }
    }
    public void PlayerRunning(bool isRunning)
    {
        playerMoveAudioSource.clip = running;

        if (isRunning)
        {
            if (!playerMoveAudioSource.isPlaying)
            {
                playerMoveAudioSource.Play();
            }
        }
        else
        {
            if (playerMoveAudioSource.isPlaying)
            {
                playerMoveAudioSource.Stop();
            }
        }
    }
}