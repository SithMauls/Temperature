using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Carry : MonoBehaviour, IInteractable {
	public bool m_Held = false;

	private Rigidbody m_ThisRigidbody = null;
	private PlayerController m_PlayerScript = null;
	private FixedJoint m_HoldJoint = null;


	private void Start() {
		gameObject.tag = "Interactable";
		m_ThisRigidbody = GetComponent<Rigidbody>();
	}

	private void Update() {

		if (m_HoldJoint == null && m_Held == true) {
			Drop();
		}
	}

	public void Interact(PlayerController playerScript) {
		m_PlayerScript = playerScript;

		if (m_Held) {
			Drop();
		}
		else {
			m_Held = true;
			m_ThisRigidbody.useGravity = false;

			m_PlayerScript.p_ActionState = PlayerController.ActionState.Holding;

			m_HoldJoint = m_PlayerScript.m_HandTransform.gameObject.AddComponent<FixedJoint>();
			m_HoldJoint.breakForce = 1000f;
			m_HoldJoint.connectedBody = m_ThisRigidbody;
		}
	}

	public void Action(PlayerController playerScript) {
		m_PlayerScript = playerScript;

		if (m_Held) {
			Drop();

			Vector3 forceDir = transform.position - m_PlayerScript.m_HandTransform.position;
			m_ThisRigidbody.AddForce(forceDir * m_PlayerScript.m_ThrowForce);
		}
	}

	private void Drop() {
		m_Held = false;

		if (m_HoldJoint != null) {
			Destroy(m_HoldJoint);
		}

		m_ThisRigidbody.useGravity = true;
		m_ThisRigidbody.AddForce(Vector3.down); // Force object to respond to gravity

		m_PlayerScript.p_ActionState = PlayerController.ActionState.Unoccupied;
	}
}
