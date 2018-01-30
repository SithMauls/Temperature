using UnityEngine;
using System.Collections;

public class WeaponShoot : MonoBehaviour {
	public float m_LaserTemp = 100f;
	public float m_LaserRange = 50f;
	public Transform m_LaserMuzzle;

	private Camera m_Camera;
	private LineRenderer m_LaserLine;
	private RaycastHit m_RaycastHit;


	private void Start() {
		m_Camera = GetComponentInParent<Camera>();
		m_LaserLine = GetComponent<LineRenderer>();
	}

	private void Update() {
		if (Input.GetButton("Fire 1") || Input.GetAxis("Fire Axis 2") > 0f) {
			Fire(true, m_LaserTemp, Color.red);
		}
		else if (Input.GetButton("Fire 2") || Input.GetAxis("Fire Axis 1") > 0f) {
			Fire(false, -m_LaserTemp, Color.cyan);
		}
		else {
			m_LaserLine.enabled = false;
		}
	}

	private void Fire(bool hotShot, float temp, Color colour) {
		m_LaserLine.enabled = true;
		m_LaserLine.startColor = colour;
		m_LaserLine.SetPosition(0, m_LaserMuzzle.position);

		Vector3 rayOrigin = m_Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

		if (Physics.Raycast(rayOrigin, m_Camera.transform.forward, out m_RaycastHit, m_LaserRange)) {
			m_LaserLine.SetPosition(1, m_RaycastHit.point);

			foreach(IShootable item in m_RaycastHit.transform.GetComponents<IShootable>()) {
				if (hotShot) {
					item.HotShot();
				}
				else if (!hotShot){
					item.ColdShot();
				}
			}
		}
		else {
			m_LaserLine.SetPosition(1, rayOrigin + (m_Camera.transform.forward * m_LaserRange));
		}
	}
}