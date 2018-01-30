using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Collider))]
public class Volume : MonoBehaviour {
	public float ambientTemp = 20f;
	[SerializeField] private bool m_AlwaysShowCollider = true;

	private Collider m_ThisCollider;


	private void Awake() {
		gameObject.tag = "Volume";
		m_ThisCollider = GetComponent<Collider>();
	}

	private void OnDrawGizmos() {
		if (m_AlwaysShowCollider) {
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(m_ThisCollider.bounds.center, m_ThisCollider.bounds.size);
		}
	}
}
