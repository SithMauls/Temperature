using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJointBreak : MonoBehaviour {
	public void OnJointBreak(float breakForce) {
		PlayerController playerScript = GetComponentInParent<PlayerController>();

		playerScript.p_ActionState = PlayerController.ActionState.Unoccupied;
	}
}
