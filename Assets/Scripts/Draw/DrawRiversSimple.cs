using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRiversSimple : MonoBehaviour {

	public Material material;
	public Color RiverColor;
	public float LineWidth = 0.25f;
	public float MinimumWidth = 4.0f;

	private static List<Vector3> vertices = new List<Vector3>();
	private static List<int> triangles = new List<int>();
	private static List<Color> colors = new List<Color>();

	// Use this for initialization
	void Awake() {
	}

	public void TriangulateRiverNet(List<RiverNode> rivernet) {
		foreach (RiverNode node in rivernet) {
			TriangulateRiver(node);
			FinalizeMesh("River");
		}
	}

	private void TriangulateRiver(RiverNode node) {
		if (node == null) {
			return;
		}

		if (node.Downslope != null && node.Width > MinimumWidth) {
			TriangulateRiverSegment(node, node.Downslope);
		}

		TriangulateRiver(node.Left);
		TriangulateRiver(node.Right);
	}

	private void TriangulateRiverSegment(RiverNode start_node, RiverNode end_node) {
		Vector2 start = start_node.Vertex.position;
		Vector2 end = end_node.Vertex.position;
		Vector2 diff = end - start;
		Vector2 offset = (new Vector2(-diff.y, diff.x)).normalized*LineWidth;
		CreateTriangle(start + offset*start_node.Width, RiverColor, end - offset*end_node.Width, RiverColor, start - offset*start_node.Width, RiverColor);
		CreateTriangle(start + offset*start_node.Width, RiverColor, end + offset*end_node.Width, RiverColor, end - offset*end_node.Width, RiverColor);
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

	private void FinalizeMesh(string name) {
		if (triangles.Count > 0) {
			// Attach the mesh to a new GameObject.
			GameObject chunk = new GameObject();
			chunk.name = name;
			chunk.transform.SetParent(transform, false);
			Mesh mesh = chunk.AddComponent<MeshFilter>().mesh = new Mesh();
			chunk.AddComponent<MeshRenderer>().material = material;

			// Move the data to the mesh and prepare the mesh for rendering.
			mesh.Clear();
			mesh.vertices = vertices.ToArray();
			mesh.colors = colors.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.RecalculateNormals();

			// Clear lists.
			vertices.Clear();
			triangles.Clear();
			colors.Clear();

			// Return.
		}
	}
}
