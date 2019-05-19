using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PortalManager : MonoBehaviour
{
	public GameObject portalPrefab;
	public float portalLifeTime = 10f;
	
	public void CreatePortal (Vector2 position)
	{
		if (portalPrefab)
		{
			Quaternion rotation = Quaternion.identity;

			GameObject destinationPortalObject = Instantiate(portalPrefab, position + new Vector2(10, 0), rotation);
			GameObject portalObject = Instantiate(portalPrefab, position, rotation);

			Portal portal = portalObject.GetComponent<Portal>();
			Portal destinationPortal = destinationPortalObject.GetComponent<Portal>();

			portal.destinationPortal = destinationPortal;
			destinationPortal.destinationPortal = portal;

			Object.Destroy(portalObject, portalLifeTime);
			Object.Destroy(destinationPortalObject, portalLifeTime);
		}
	}
}
