using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCtrl : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerSfx
    {
        public AudioClip[] fire;
        public AudioClip[] reload;
    }
    public enum WeaponType
    {
        RIFLE = 0,
        SHOTGUN
    }
    public WeaponType currWeapon = WeaponType.RIFLE;
    public GameObject bullet;
    public Transform firePos;
    public ParticleSystem cartidge;
    public PlayerSfx playersfx;


    private ParticleSystem muzzleFlash;
    private AudioSource _audio;




    void Start()
    {
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);
        cartidge.Play();
        muzzleFlash.Play();
        Firesfx();
    }
    void Firesfx()
    {
        var _sfx = playersfx.fire[(int)currWeapon];
        _audio.PlayOneShot(_sfx, 1.0f);
    }
}
