using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	Player player;
	Animator animator;
	SpriteRenderer spriteRenderer;

	void Start () {
		player = GetComponent<Player> ();
		animator = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update () {
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
		player.SetDirectionalInput (directionalInput);

		if (directionalInput[0] < 0) {
			spriteRenderer.flipX = true;
		} else if (directionalInput[0] > 0) {
			spriteRenderer.flipX = false;
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}
	}
}
