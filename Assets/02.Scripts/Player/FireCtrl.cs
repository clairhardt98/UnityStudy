using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private Shake shake;

    public Image magazineImage;
    public Text magazineText;

    public int maxBullet = 10;
    public int remainingBullet = 10;

    public float reloadTime = 2.0f;
    private bool isReloading = false;
    void Start()
    {
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            --remainingBullet;
            Fire();

            if (remainingBullet == 0)
            {
                StartCoroutine(Reloading());
            }
        }
    }

    void Fire()
    {
        StartCoroutine(shake.ShakeCamera());
        Instantiate(bullet, firePos.position, firePos.rotation);
        cartidge.Play();
        muzzleFlash.Play();
        Firesfx();

        magazineImage.fillAmount = (float)remainingBullet / (float)maxBullet;
        UpdateBulletText();
    }
    void Firesfx()
    {
        var _sfx = playersfx.fire[(int)currWeapon];
        _audio.PlayOneShot(_sfx, 1.0f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playersfx.reload[(int)currWeapon],1.0f);

        yield return new WaitForSeconds(playersfx.reload[(int)currWeapon].length + 0.3f);

        isReloading = false;
        magazineImage.fillAmount = 1.0f;
        remainingBullet = maxBullet;
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        magazineText.text = string.Format("<color=#ff0000>{0}</color>/{1}", remainingBullet, maxBullet);
    }
}
