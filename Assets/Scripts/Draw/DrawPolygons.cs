using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateIsland))]
public class DrawPolygons : MonoBehaviour {
	public Color LandColor, WaterColor;
	public int PolygonsPerMesh = 2000;
	public Material material;

	private IslandData Data;

	private static List<Vector3> vertices = new List<Vector3>();
	private static List<int> triangles = new List<int>();
	private static List<Color> colors = new List<Color>();

	void Awake() {
		Data = GetComponent<IslandData>();
	}

	// Use this for initialization
	void Start() {
		GetComponent<GenerateIsland>().Generate();
		int start = 0;
		while (start <= Data.Map.Polygons.Count) {
			TriangulateMesh(start, Mathf.Min(Data.Map.Polygons.Count, start + PolygonsPerMesh));
			start += PolygonsPerMesh;
		}
	}

	private void TriangulateMesh(int start, int end) {
		for (int i = start; i < end; i++) {
			TriangulatePolygon(Data.Map.Polygons[i]);
		}

		FinalizeMesh();
		vertices.Clear();
		triangles.Clear();
		colors.Clear();
	}

	private void TriangulatePolygon(Polygon polygon) {
		Color c1, c2, c3;
		var corners = new List<Vertex>();

		foreach (int vertex in polygon.corners) {
			corners.Add(Data.Map.Vertices[vertex]);
		}

		for (int i = 0; i < corners.Count; i++) {
			int inext = Utils.Mod(i + 1, corners.Count);
			if (Data.LandMaskPolygons[polygon.index]) {
				c1 = Grayscale(Data.ElevationPolygons[polygon.index]);
				c2 = Grayscale(Data.ElevationVertices[corners[inext].index]);
				c3 = Grayscale(Data.ElevationVertices[corners[i].index]);
			} else {
				c1 = WaterColor;
				c2 = WaterColor;
				c3 = WaterColor;
			}
			CreateTriangle(polygon.position, c1, corners[inext].position, c2, corners[i].position, c3);
		}
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

	private void FinalizeMesh() {
		// Attach the mesh to a new GameObject.
		GameObject polygons = new GameObject();
		polygons.name = "Hexes";
		polygons.transform.parent = transform;
		Mesh mesh = polygons.AddComponent<MeshFilter>().mesh = new Mesh();
		polygons.AddComponent<MeshRenderer>().material = material;

		// Move the data to the mesh and prepare the mesh for rendering.
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.colors = colors.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}

	private Color Grayscale(float value) {
		return new Color(value, value, value, 1.0f);
	}
}
