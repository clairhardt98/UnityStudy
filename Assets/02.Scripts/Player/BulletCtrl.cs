﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20.0f;
    public float speed = 3000.0f;
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }
}