using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {
	[Header("References")]
	[SerializeField] private Camera m_Camera = null;
	public Transform m_HandTransform = null;
	[SerializeField] private Image m_CursorImage = null;
	[Header("Properties")]
	[SerializeField] private float m_Speed = 5f;
	[SerializeField] private float m_LookSensitivity = 3f;
	[SerializeField] private float m_JumpForce = 200f;
	public float m_ThrowForce = 200f;

	private Transform m_ThisTransform, m_CameraTransform;
	private Rigidbody m_ThisRigidbody;
	private PlayerMotor m_MotorScript;
	private Vector3 m_ViewportCentre = new Vector3(0.5f, 0.5f, 0f);
	private RaycastHit m_RaycastHit;
	private ActionState m_ActionState = ActionState.Unoccupied;
	private float m_PlayerHeight;

	public enum ActionState { Holding, CanInteract, Unoccupied};

	public ActionState p_ActionState {
		get { return m_ActionState; }
		set {
			m_ActionState = value;

			switch (m_ActionState) {
				case ActionState.Holding:
					m_CursorImage.color = Color.cyan;
					break;

				case ActionState.CanInteract:
					m_CursorImage.color = Color.green;
					break;

				case ActionState.Unoccupied:
					m_CursorImage.color = Color.white;
					break;

				default:
					break;
			}
		}
	}


	private void Start() {
		m_ThisTransform = GetComponent<Transform>();
		m_ThisRigidbody = GetComponent<Rigidbody>();
		m_MotorScript = GetComponent<PlayerMotor>();
		m_Camera = GetComponentInChildren<Camera>();
		m_CameraTransform = m_Camera.transform;

		m_PlayerHeight = GetComponent<Renderer>().bounds.size.y;
	}

	private void Update() {
		PlayerControl();
		CameraControl();
		ObjectDetection();

		if (Input.GetButtonDown("Jump")) {
			Ray jumpRay = new Ray(m_ThisTransform.position, -m_ThisTransform.up);
			if (Physics.Raycast(jumpRay, (m_PlayerHeight / 2f) + 0.1f)) {
				m_ThisRigidbody.AddForce(transform.up * m_JumpForce);
			}
		}

		if (Input.GetButtonDown("Interact")) {
			if (p_ActionState != ActionState.Unoccupied) {
				IInteractable interactComponent = m_RaycastHit.collider.transform.GetComponent<IInteractable>();

				if (interactComponent != null) {
					interactComponent.Interact(this);
				}
			}
		}

		if (Input.GetButtonDown("Action")) {
			if (p_ActionState == ActionState.Holding) {
				IInteractable interactComponent = m_RaycastHit.collider.transform.GetComponent<IInteractable>();

				if (interactComponent != null) {
					interactComponent.Action(this);
				}
			}
		}
	}

	private void PlayerControl() {
		//Calculate movement velocity as a 3D vector
		float xMove = Input.GetAxisRaw("Player X");
		float zMove = Input.GetAxisRaw("Player Y");

		Vector3 moveHorizontal = m_ThisTransform.right * xMove;
		Vector3 moveVertical = m_ThisTransform.forward * zMove;

		//Final movement vector
		Vector3 velocity = (moveHorizontal + moveVertical).normalized * m_Speed;

		//Apply movement
		m_MotorScript.Move(velocity);
	}

	private void CameraControl() {
		//Calculate rotation as a 3D vector (turning around)
		float yRotation = Input.GetAxisRaw("Camera X");

		Vector3 rotation = new Vector3(0f, yRotation, 0f) * m_LookSensitivity;

		//Apply rotation
		m_MotorScript.Rotate(rotation);

		//Calculate camera rotation as a 3D vector (turning around)
		float xRotation = Input.GetAxisRaw("Camera Y");

		float cameraRotationX = xRotation * m_LookSensitivity;

		//Apply camera rotation
		m_MotorScript.RotateCamera(cameraRotationX);
	}

	private void ObjectDetection() {
		Ray ray = new Ray(m_Camera.ViewportToWorldPoint(m_ViewportCentre), m_CameraTransform.forward);

		if (p_ActionState != ActionState.Holding) {
			if (Physics.Raycast(ray, out m_RaycastHit, 3)) {
				string hitTag = m_RaycastHit.collider.transform.tag;

				switch (hitTag) {
					case "Interactable":
						p_ActionState = ActionState.CanInteract;
						break;

					default:
						p_ActionState = ActionState.Unoccupied;
						break;
				}
			}
			else {
				p_ActionState = ActionState.Unoccupied;
			}
		}
	}
}
