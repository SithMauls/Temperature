using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Carry : MonoBehaviour, IInteractable {
	[SerializeField] private bool m_Held = false;
	[SerializeField] private float m_GripForce = 3000f;

	private Rigidbody m_ThisRigidbody = null;
	private Joint m_HoldJoint = null;


	private void Start() {
		gameObject.tag = "Interactable";
		m_ThisRigidbody = GetComponent<Rigidbody>();
	}

	private void Update() {
		if (m_HoldJoint == null && m_Held) {
			Drop();
		}
	}

	public void Interact(PlayerController playerScript) {
		if (m_Held) {
			Drop();

			playerScript.p_ActionState = PlayerController.ActionState.Unoccupied;
		}
		else {
			PickUp(playerScript.m_HandTransform);

			playerScript.p_ActionState = PlayerController.ActionState.Holding;
		}
	}

	public void Action(PlayerController playerScript) {
		if (m_Held) {
			Throw(playerScript.m_HandTransform, playerScript.m_ThrowForce);

			playerScript.p_ActionState = PlayerController.ActionState.Unoccupied;
		}
	}

	private void PickUp(Transform hands) {
		m_Held = true;

		m_ThisRigidbody.useGravity = false;

		m_HoldJoint = hands.gameObject.AddComponent<FixedJoint>();
		m_HoldJoint.connectedBody = m_ThisRigidbody;
		m_HoldJoint.breakForce = m_GripForce;
	}

	private void Throw(Transform hands, float force) {
		Drop();

		Vector3 forceDir = transform.position - hands.position;
		m_ThisRigidbody.AddForce(forceDir * force);
	}

	private void Drop() {
		m_Held = false;

		if (m_HoldJoint != null) {
			Destroy(m_HoldJoint);
		}

		m_ThisRigidbody.useGravity = true;
		m_ThisRigidbody.AddForce(Vector3.down); // Force object to respond to gravity
	}
}
