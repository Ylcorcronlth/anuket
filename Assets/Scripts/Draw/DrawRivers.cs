using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateIsland))]
public class DrawRivers : MonoBehaviour {

	public Material material;
	public int VerticesPerMesh = 2000;
	public float LineWidth = 0.05f;
	public Color RiverColor;

	private IslandData Data;

	private static List<Vector3> vertices = new List<Vector3>();
	private static List<int> triangles = new List<int>();
	private static List<Color> colors = new List<Color>();

	void Awake() {
		Data = GetComponent<IslandData>();
	}

	void Start() {
		GetComponent<GenerateIsland>().Generate();
		int start = 0;
		while (start <= Data.Map.Vertices.Count) {
			TriangulateMesh(start, Mathf.Min(Data.Map.Vertices.Count, start + VerticesPerMesh));
			start += VerticesPerMesh;
		}
	}

	void TriangulateMesh(int start, int end) {
		for (int i = start; i < end; i++) {
			TriangulateLine(Data.Map.Vertices[i]);
		}
		FinalizeMesh();
	}

	void TriangulateLine(Vertex vertex) {
		if (Data.Downslope[vertex.index] >= 0) {
			Vector2 start = vertex.position;
			Vector2 end = Data.Map.Vertices[Data.Downslope[vertex.index]].position;
			CreateLine(start, end);
		}
	}

	private void CreateLine(Vector2 start, Vector2 end) {
		Vector2 diff = end - start;
		Vector2 offset = (new Vector2(-diff.y, diff.x)).normalized*LineWidth;
		CreateTriangle(start + offset, RiverColor, end - offset, RiverColor, start - offset, RiverColor);
		CreateTriangle(start + offset, RiverColor, end + offset, RiverColor, end - offset, RiverColor);
	}

	private void CreateTriangle(Vector3 r1, Color c1, Vector3 r2, Color c2, Vector3 r3, Color c3) {
		// Create a triangle with the given vertex positions and colors.
		triangles.Add(vertices.Count);
		triangles.Add(vertices.Count + 1);
		triangles.Add(vertices.Count + 2);

		vertices.Add(r1);
		vertices.Add(r2);
		vertices.Add(r3);

		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}

	void FinalizeMesh() {
		// Attach the mesh to a new GameObject.
		GameObject polygons = new GameObject();
		polygons.name = "Rivers";
		polygons.transform.parent = transform;
		polygons.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
		Mesh mesh = polygons.AddComponent<MeshFilter>().mesh = new Mesh();
		polygons.AddComponent<MeshRenderer>().material = material;

		// Move the data to the mesh and prepare the mesh for rendering.
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.colors = colors.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		// Clear out temporary data.
		vertices.Clear();
		triangles.Clear();
		colors.Clear();
	}
}
