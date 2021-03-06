﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
	public float speed = 40f;
	Rigidbody2D rigidbody;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		if (rigidbody)
		{
			rigidbody.velocity = transform.right * speed;
		}
		else Debug.Log("ale jak to ");
	}

    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(10);
        }
    }
}
