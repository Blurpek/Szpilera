using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public Transform firePoint;
	public GameObject bulletPrefab;
	public LineRenderer lineRenderer;
	SpriteRenderer spriteRenderer;
	private WaitForSeconds shotDuration = new WaitForSeconds(.07f);
	// Start is called before the first frame update
	void Start()
    {
		spriteRenderer = GetComponent<SpriteRenderer>();
		lineRenderer.enabled = false;
    }
	
	public IEnumerator Shoot()
	{
		//prefab way
		//Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

		//raycast way
		RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);

		lineRenderer.SetPosition(0, firePoint.position);

		if (hitInfo)
		{
			//Instantiate(impactEffect, hitInfo.point, Quaternion.identity);

			lineRenderer.SetPosition(1, hitInfo.point);
		}
		else
		{
			lineRenderer.SetPosition(1, firePoint.position + firePoint.right * 30);
		}

		lineRenderer.enabled = true;

		yield return shotDuration;

		lineRenderer.enabled = false;
	}

	public void rotateTo(float angle)
	{
		transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle);
	}

	public void flip(Vector3 parentPoint)
	{
		transform.RotateAround(parentPoint, Vector3.up, 180);
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
	}
}
