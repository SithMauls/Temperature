using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TemperatureController : MonoBehaviour {
	[SerializeField] private float m_Temp = 20f;
	[SerializeField] private float m_FreezePoint = 0f;
	[SerializeField] private float m_BoilPoint = 100f;
	public float m_SurroundTemp = 20f;
	[SerializeField] [Range(0.0f, 0.3f)] private float m_CoolingConstant = 0.15f;
	[SerializeField] private Condition m_Condition = Condition.Neutral;
	[SerializeField] private Gradient m_ColourRange;
	public float m_Volume;
	public float m_SurfaceArea;

	private enum Condition { Freezing, Neutral, Boiling };
	private Material m_ThisMaterial;
	private Mesh m_ThisMesh;

	private float p_Temp {
		get { return m_Temp; }
		set {
			m_Temp = value;

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
	}


	private void Awake() {
		m_ThisMaterial = GetComponent<Renderer>().material;
		m_ThisMesh = GetComponent<MeshFilter>().mesh;

		//Set colour gradient
		GradientColorKey[] colorKeys = new GradientColorKey[3];
		colorKeys[0] = new GradientColorKey(Color.cyan, 0f);
		colorKeys[1] = new GradientColorKey(Color.white, 0.5f);
		colorKeys[2] = new GradientColorKey(Color.red, 1f);

		GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
		alphaKeys[0] = new GradientAlphaKey(1f, 0f);
		alphaKeys[1] = new GradientAlphaKey(1f, 1f);

		m_ColourRange.SetKeys(colorKeys, alphaKeys);

		m_Volume = VolumeOfMesh(m_ThisMesh);
		m_CoolingConstant /= m_Volume;

		m_SurfaceArea = SurfaceAreaOfMesh();
	}

	private void Update() {
		NewtonsLaw();
		UpdateColour();
	}

	private void OnTriggerStay(Collider other) {
		if (other.tag == "Volume")
		{
			m_SurroundTemp = other.GetComponent<Volume>().ambientTemp;
		}
	}

	private void UpdateColour() {
		float colourPoint = (p_Temp - m_FreezePoint) / (m_BoilPoint - m_FreezePoint);
		
		m_ThisMaterial.color = m_ColourRange.Evaluate(colourPoint);
	}

	private void NewtonsLaw() {
		float _coolingRate = Mathf.Pow(2.71828f, -m_CoolingConstant * Time.deltaTime);
		
		p_Temp = m_SurroundTemp + (p_Temp - m_SurroundTemp) * _coolingRate;
	}


	public float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3) {
		float v321 = p3.x * p2.y * p1.z;
		float v231 = p2.x * p3.y * p1.z;
		float v312 = p3.x * p1.y * p2.z;
		float v132 = p1.x * p3.y * p2.z;
		float v213 = p2.x * p1.y * p3.z;
		float v123 = p1.x * p2.y * p3.z;
		return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
	}

	public float VolumeOfMesh(Mesh mesh) {
		float volume = 0;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		for (int i = 0; i < mesh.triangles.Length; i += 3)
     {
			Vector3 p1 = vertices[triangles[i + 0]];
			Vector3 p2 = vertices[triangles[i + 1]];
			Vector3 p3 = vertices[triangles[i + 2]];
			volume += SignedVolumeOfTriangle(p1, p2, p3);
		}
		return Mathf.Abs(volume);
	}

	public float SurfaceAreaOfMesh() {
		Vector3[] vertices = m_ThisMesh.vertices;
		int[] triangles = m_ThisMesh.triangles;

		float result = 0f;

		for (int p = 0; p < triangles.Length; p += 3) {
			result += (Vector3.Cross(vertices[triangles[p + 1]] - vertices[triangles[p]],
						vertices[triangles[p + 2]] - vertices[triangles[p]])).magnitude;
		}

		return result *= 0.5f;
	}
}
