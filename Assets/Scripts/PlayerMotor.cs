using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {
	[SerializeField] private Camera m_Camera;
	[SerializeField] private float m_CameraRotationLimit = 75f;

	private Vector3 m_Velocity = Vector3.zero;
	private Vector3 m_Rotation = Vector3.zero;
	private float m_CameraRotationX = 0f;
	private float m_CurrentCameraRotationX = 0f;
	private Rigidbody m_ThisRigidbody;


	private void Start() {
		m_ThisRigidbody = GetComponent<Rigidbody>();
	}

	//Run every physics iteration
	private void FixedUpdate() {
		PerformMovement();
		PerformRotation();
	}

	//Gets a movement vector
	public void Move(Vector3 velocity) {
		m_Velocity = velocity;
	}

	//Gets a rotational vector
	public void Rotate(Vector3 rotation) {
		m_Rotation = rotation;
	}

	//Gets a rotational vector for the camera
	public void RotateCamera(float cameraRotationX) {
		m_CameraRotationX = cameraRotationX;
	}

	//Perform movement based on velocity variable
	private void PerformMovement() {
		if (m_Velocity != Vector3.zero) {
			m_ThisRigidbody.MovePosition(m_ThisRigidbody.position + m_Velocity * Time.fixedDeltaTime);
		}
	}

	//Perform rotation
	private void PerformRotation() {
		m_ThisRigidbody.MoveRotation(m_ThisRigidbody.rotation * Quaternion.Euler(m_Rotation));

		if (m_Camera != null) {
			//Set and clamp camera rotation
			m_CurrentCameraRotationX -= m_CameraRotationX;
			m_CurrentCameraRotationX = Mathf.Clamp(m_CurrentCameraRotationX, -m_CameraRotationLimit, m_CameraRotationLimit);

			//Apply rotation to camera transform
			m_Camera.transform.localEulerAngles = new Vector3(m_CurrentCameraRotationX, 0f, 0f);
		}
	}
}
