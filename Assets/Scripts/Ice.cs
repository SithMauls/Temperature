using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : Temperature {
	[Header("Ice Properties")]
	[SerializeField] private float m_FreezePoint;
	[SerializeField] private float m_BoilPoint;
	[SerializeField] private Condition m_Condition = Condition.Neutral;

	private Transform m_ThisTransform;
	private enum Condition { Freezing, Neutral, Boiling };
	private float m_MeltMultiplier = 0.1f;

	protected override void Start() {
		base.Start();

		m_ThisTransform = GetComponent<Transform>();
	}

	protected override void Update() {
		base.Update();

		CheckState();

		if (m_Condition != Condition.Freezing) {
			Melt();

			if (m_ThisTransform.localScale.x < 0.1f) {
				Destroy(gameObject);
			}
		}
	}

	private void CheckState() {
		if (m_Temp <= m_FreezePoint) {
			if (m_Condition != Condition.Freezing) {
				m_Condition = Condition.Freezing;
			}
		}
		else if (m_Temp >= m_BoilPoint) {
			if (m_Condition != Condition.Boiling) {
				m_Condition = Condition.Boiling;
			}
		}
		else {
			if (m_Condition != Condition.Neutral) {
				m_Condition = Condition.Neutral;
			}
		}
	}

	private void Melt() {
		transform.localScale *= 1f - (((m_Temp - m_FreezePoint) / (m_BoilPoint - m_FreezePoint)) * m_MeltMultiplier * Time.deltaTime);

		CalculateMeasurements();
	}
}
