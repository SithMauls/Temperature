using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Temperature : MonoBehaviour {
	[SerializeField] protected float m_Temp = 20f;
	public float m_SurroundTemp = 20f;
	public float placeHolder;

	protected Mesh m_ThisMesh;
	protected float m_Volume;
	protected float m_SurfaceArea;
	protected float m_CoolingConstant;
	protected bool m_BeingShot = false;


	protected virtual void Start() {
		m_ThisMesh = GetComponent<MeshFilter>().mesh;

		CalculateMeasurements();
	}

	protected virtual void Update() {
		NewtonsLaw();
	}

	protected void OnTriggerEnter(Collider other) {
		Volume volumeScript = other.GetComponent<Volume>();
		if (volumeScript != null) {
			placeHolder = m_SurroundTemp;
			m_SurroundTemp = volumeScript.ambientTemp;
		}
	}

	protected void OnTriggerExit(Collider other) {
		Volume volumeScript = other.GetComponent<Volume>();
		if (volumeScript != null) {
			m_SurroundTemp = placeHolder;
		}
	}

	private void NewtonsLaw() {
		float coolingRate = Mathf.Pow(2.71828f, -m_CoolingConstant * Time.deltaTime);

		m_Temp = m_SurroundTemp + (m_Temp - m_SurroundTemp) * coolingRate;
	}

	protected void CalculateMeasurements() {
		CalculateVolume();
		CalculateSurfaceArea();

		m_CoolingConstant = m_SurfaceArea / m_Volume * 0.025f;
	}

	private void CalculateVolume() {
		float volume = 0;
		Vector3[] vertices = m_ThisMesh.vertices;
		int[] triangles = m_ThisMesh.triangles;

		for (int i = 0; i < m_ThisMesh.triangles.Length; i += 3) {
			Vector3 p1 = vertices[triangles[i + 0]];
			Vector3 p2 = vertices[triangles[i + 1]];
			Vector3 p3 = vertices[triangles[i + 2]];
			volume += SignedVolumeOfTriangle(p1, p2, p3);
		}

		float scale = transform.localScale.x * transform.localScale.y * transform.localScale.z;
		volume *= scale;

		m_Volume = Mathf.Abs(volume);
	}

	private float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3) {
		float v321 = p3.x * p2.y * p1.z;
		float v231 = p2.x * p3.y * p1.z;
		float v312 = p3.x * p1.y * p2.z;
		float v132 = p1.x * p3.y * p2.z;
		float v213 = p2.x * p1.y * p3.z;
		float v123 = p1.x * p2.y * p3.z;
		return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
	}

	private void CalculateSurfaceArea() {
		Vector3[] vertices = m_ThisMesh.vertices;
		int[] triangles = m_ThisMesh.triangles;

		float surfaceArea = 0f;

		for (int p = 0; p < triangles.Length; p += 3) {
			surfaceArea += (Vector3.Cross(vertices[triangles[p + 1]] - vertices[triangles[p]],
							vertices[triangles[p + 2]] - vertices[triangles[p]])).magnitude;
		}

		surfaceArea *= Mathf.Pow(transform.localScale.x, 2f);

		m_SurfaceArea = surfaceArea *= 0.5f;
	}

	public void ShotStart(float temp) {

	}
}
