using System;
using System.Collections;
using System.Collections.Generic;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";

    private float hp = 100.0f;
    private float initHp = 100.0f;

    private GameObject bloodEffect;

    public GameObject hpBarPrefab;

    public Vector3 hpBarOffset = new Vector3(0, 2.2f, 0);

    private Canvas uiCanvas;

    private Image hpBarImage;
    // Start is called before the first frame update
    void Start()
    {
        //enemyAI = GetComponent<EnemyAI>();
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");
        SetHpBar();
    }

    void SetHpBar()
    {
        uiCanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
        GameObject hpBar = Instantiate<GameObject>(hpBarPrefab, uiCanvas.transform);
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        var _hpBar = hpBar.GetComponent<EnemyHpBar>();
        _hpBar.targetTr = GetComponent<Transform>();
        _hpBar.offset = hpBarOffset;
    }
    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag(bulletTag))
        {
            ShowBloodEffect(coll);
            //Destroy(coll.gameObject);
            coll.gameObject.SetActive(false);
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;

            hpBarImage.fillAmount = hp / initHp;
            
            if (hp <= 0.0f)
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;
            }
        }
    }

    void ShowBloodEffect(Collision coll)
    {
        Vector3 pos = coll.contacts[0].point;
        Vector3 _normal = coll.contacts[0].normal;
        
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward,_normal);

        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood,1.0f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
