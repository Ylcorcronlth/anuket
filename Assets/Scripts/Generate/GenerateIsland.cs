using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DrawEvent : UnityEvent<List<ChunkData>> {}

public class GenerateIsland : MonoBehaviour {

	public int ChunkOrder = 3;
	public float Radius;
	public DrawEvent m_DrawEvent;

	// Use this for initialization
	void Start() {
		var simplex = new Simplex2D(42);

		int chunk_size = Utils.Pow(7, ChunkOrder);
		int gbt_radius = (int)Mathf.Max(ChunkOrder, Mathf.Ceil(2.0f*Mathf.Log(1.0f + Radius)/Mathf.Log(7.0f))) - ChunkOrder + 1;

		int count = 0;
		var chunks = new List<ChunkData>();
		for (int i = 0; i < Utils.Pow(7, gbt_radius); i++) {
			var chunk = new ChunkData(i, chunk_size);
			for (int j = 0; j < chunk.ChunkSize; j++) {
				GBTHex hex = new GBTHex(i*chunk.ChunkSize + j);
				Vector2 pos = hex.position;
				if (pos.sqrMagnitude/(Radius*Radius) < simplex.GetFractalNoise(3.0f*pos/Radius)) {
					chunk.AddColors(Color.white);
					count += 1;
				} else {
					chunk.AddColors(null);
				}
			}
			chunks.Add(chunk);
		}
		Debug.Log(count);
		m_DrawEvent.Invoke(chunks);
	}
}
