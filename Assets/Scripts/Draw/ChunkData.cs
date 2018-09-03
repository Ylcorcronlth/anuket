using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData {
	private int _ChunkSize; // Should be a power of 7.
	private int _ChunkID; // Note that 1 indicates the chunk centered on hex ChunkSize.
	private List<Color[]> ColorData; // Each element should have an array length of 7.
	private List<Vector3[]> VectorData;

	public int ChunkID {
		get {
			return _ChunkID;
		}
	}

	public int ChunkSize {
		get {
			return _ChunkSize;
		}
	}

	public ChunkData(int chunk_id, int chunk_size=343) {
		_ChunkID = chunk_id;
		_ChunkSize = chunk_size;
		ColorData = new List<Color[]>();
		VectorData = new List<Vector3[]>();
	}

	public Color[] GetColors(int id) {
		// Should probably throw an exception if id > ChunkSize.
		if (id < ColorData.Count) {
			return ColorData[id];
		} else {
			return null;
		}
	}

	public void AddColors(Color[] colors) {
		ColorData.Add(colors);
	}

	public void AddColors(Color color) {
		var colors = new Color[7];
		for (int i = 0; i < 7; i++) {
			colors[i] = color;
		}
		ColorData.Add(colors);
	}

	public Vector3[] GetVectors(int id) {
		// Should probably throw an exception if id > ChunkSize.
		if (id < VectorData.Count) {
			return VectorData[id];
		} else {
			return null;
		}
	}

	public void AddVectors(Vector3[] vectors) {
		VectorData.Add(vectors);
	}
}
