using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using UnityEngine.Networking;

[System.Serializable] public class _UnityEventTransform : UnityEvent<Vector2> { }
[RequireComponent (typeof (Player))]
public class PlayerInput : NetworkBehaviour
{
	public _UnityEventTransform createPortalEvent = new _UnityEventTransform();


	Player player;
	Animator animator;
	SpriteRenderer spriteRenderer;
	public Weapon weapon;
	public float minTimeBetweenTeleportations;
	float timeOfLastTeleportation;
	public float fireRate = .25f;
	private float nextFire = 0f;
	float angle;
	Vector3 object_pos;

	bool facingRight = true;

	void Start () {
		player = GetComponent<Player> ();
		animator = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		timeOfLastTeleportation = Time.time;
	}

	void FixedUpdate () {


        if (!isLocalPlayer) {
            return;
        }

		Vector3 mousePosition = Input.mousePosition;
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));

		rotateToMouse(mousePosition);

		player.SetDirectionalInput(directionalInput);

		if (Input.GetKeyDown (KeyCode.Space)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}
		if (Input.GetButtonDown("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			if (weapon)
				StartCoroutine(weapon.Shoot());
			else
				Debug.Log("nie szczela ;/");
		}
		if (Input.GetButtonDown("CreatePortalButton")) {
			if (createPortalEvent != null)
			{
				createPortalEvent.Invoke(calculatePortalPosition());
			}
		}
	}

	private Vector2 calculatePortalPosition() {
		return (Vector2) this.transform.position + new Vector2(spriteRenderer.bounds.size.x * (facingRight ? 1 : -1)  / 2, 0);
	}

	private void rotateToMouse(Vector3 mousePosition)
	{
		mousePosition.z = 0f; //The distance between the camera and object
		object_pos = Camera.main.WorldToScreenPoint(transform.position);
		mousePosition.x = mousePosition.x - object_pos.x;
		mousePosition.y = mousePosition.y - object_pos.y;
		angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
		if (facingRight && Mathf.Abs(angle) > 95)
		{
			facingRight = false;
			spriteRenderer.flipX = true;
			weapon.flip(transform.position);
		}
		else if (!facingRight && Mathf.Abs(angle) < 85)
		{
			facingRight = true;
			spriteRenderer.flipX = false;
			weapon.flip(transform.position);
		}
		weapon.rotateTo(angle);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "portal")
		{
			Portal portal = collision.gameObject.GetComponent<Portal>();
			float currentTime = Time.time;
			if (portal && currentTime - timeOfLastTeleportation > minTimeBetweenTeleportations)
			{
				Vector2 destination = portal.destinationPortal.transform.position;

				this.transform.position = destination;
				this.timeOfLastTeleportation = currentTime;
			}
		}
	}
}
