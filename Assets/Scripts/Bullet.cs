using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
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
}
