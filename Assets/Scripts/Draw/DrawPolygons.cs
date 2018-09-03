using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPolygons : MonoBehaviour {
	public Color DefaultColor = Color.magenta;
	public Material material;

	private static List<Vector3> vertices = new List<Vector3>();
	private static List<int> triangles = new List<int>();
	private static List<Color> colors = new List<Color>();

	private Dictionary<int, GameObject> Chunks;

	public void Awake() {
		Chunks = new Dictionary<int, GameObject>();
	}

	public void DrawChunks(List<ChunkData> chunks) {
		foreach (ChunkData chunk in chunks) {
			DrawChunk(chunk);
		}
	}

	private void DrawChunk(ChunkData chunk_data) {
		GameObject chunk;
		if (Chunks.TryGetValue(chunk_data.ChunkID, out chunk)) {
			GameObject.Destroy(chunk);
		}

		for (int i = 0; i < chunk_data.ChunkSize; i++) {
			TriangulateHex(new GBTHex(i), chunk_data.GetColors(i));
		}

		chunk = FinalizeMesh("Chunk (" + (chunk_data.ChunkID).ToString() + ")");
		chunk.transform.localPosition = new GBTHex(chunk_data.ChunkID*chunk_data.ChunkSize).position;
		Chunks[chunk_data.ChunkID] = chunk;
	}

	private void TriangulateHex(GBTHex hex, Color[] colors) {
		if (colors == null) {
			return;
		}
		Vector2 center = hex.position;
		var corners = hex.GetCorners();
		for (int i = 0; i < corners.Count; i++) {
			int inext = (i + 1) % 6;
			GBTCorner first = corners[i];
			GBTCorner second = corners[inext];
			CreateTriangle(center, colors[0], second.position, colors[inext + 1], first.position, colors[i + 1]);
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

	private GameObject FinalizeMesh(string name) {
		// Attach the mesh to a new GameObject.
		GameObject chunk = new GameObject();
		chunk.name = name;
		chunk.transform.parent = transform;
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
		return chunk;
	}
}
