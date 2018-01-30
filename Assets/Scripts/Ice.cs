using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour, IShootable {
	public float m_ScaleRate = 0.1f;

	private Transform m_ThisTransform;


	private void Start() {
		m_ThisTransform = GetComponent<Transform>();
	}

	public void ColdShot() {
		m_ThisTransform.localScale *= (1.01f);
	}

	public void HotShot() {
		m_ThisTransform.localScale *= (0.99f);
	}
}
