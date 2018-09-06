using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChunkDrawEvent : UnityEvent<List<ChunkData>> {}

[System.Serializable]
public class RiverDrawEvent : UnityEvent<List<RiverNode>> {}

public class GenerateIsland : MonoBehaviour {

	public int ChunkOrder = 3;
	public float Radius;
	public float Alpha;
	public float Threshold;
	public float Exponent = 2.0f;
	public ChunkDrawEvent m_DrawPolygonEvent;
	public RiverDrawEvent m_DrawRiverEvent;

	private float max_noise_value = 0.0f;
	private int ChunkSize;

	// Use this for initialization
	void Start() {
		ChunkSize = Utils.Pow(7, ChunkOrder);
		MapArray<int> bitmask = GenerateLandMask(new MapArray<int>(0));
		List<RiverNode> rivernet = GenerateDownslopes(bitmask);
		//DrawGrayscaleArray(CalculateRiverWidths(rivernet), bitmask, 0.0f, 100.0f);
		CalculateRiverWidths(rivernet);
		DrawLandMask(bitmask);
		m_DrawRiverEvent.Invoke(rivernet);
	}

	private MapArray<float> CalculateRiverWidths(List<RiverNode> rivernet) {
		var river_widths = new MapArray<float>(0.0f);
		foreach (RiverNode node in rivernet) {
			GetRiverWidth(node, river_widths);
		}
		return river_widths;
	}

	private float CombineWidths(float x1, float x2) {
		return Mathf.Pow(1.0f + Mathf.Pow(x1, Exponent) + Mathf.Pow(x2, Exponent), 1.0f/Exponent);
	}

	private float GetRiverWidth(RiverNode node, MapArray<float> river_widths) {
		if (node == null) {
			return 0.0f;
		}
		node.Width = CombineWidths(GetRiverWidth(node.Left, river_widths), GetRiverWidth(node.Right, river_widths));
		river_widths[node.Vertex] = node.Width;
		return node.Width;
	}

	private List<RiverNode> GenerateDownslopes(MapArray<int> bitmask) {
		var simplex = new Simplex2D(52562);

		MapArray<float> weights = new MapArray<float>(1000.0f);
		var rivernet = new List<RiverNode>();
		var node_lookup = new Dictionary<GBTCorner, RiverNode>();
		var queue = new BinaryMinHeap<QueueElement<RiverNode>>();

		// Assign weights to each land vertex and add coast vertices to the queue.
		foreach (KeyValuePair<GBTCorner, int> kvp in bitmask.GetCornerEnumerator()) {
			if ((kvp.Value & 1) > 0) {
				weights[kvp.Key] = simplex.GetFractalNoise(4.0f*kvp.Key.position/Radius);
				if ((kvp.Value & 2) > 0) {
					RiverNode node = new RiverNode(kvp.Key);
					queue.Push(new QueueElement<RiverNode>(weights[kvp.Key], node));
					rivernet.Add(node);
					node_lookup[kvp.Key] = node;
				}
			}
		}

		while (queue.Count > 0) {
			RiverNode node = queue.Pop().value;

			GBTCorner lowest = new GBTCorner(-1);
			float lowest_weight = 999.0f;

			// Find the neighboring land node with the lowest weight which has not already
			// been added to the network.
			foreach (GBTCorner adjacent in node.Vertex.GetAdjacent()) {
				if (CheckValidAdjacent(node, adjacent, bitmask, node_lookup) && weights[adjacent] < lowest_weight) {
					lowest_weight = weights[adjacent];
					lowest = adjacent;
				}
			}

			// Add the lowest node to the network, and push it and the into the queue.
			if (lowest.isValid()) {
				var new_node = new RiverNode(lowest);
				new_node.Downslope = node;
				if (node.Left == null) {
					node.Left = new_node;
					// If the node hasn't been filled, add it to the queue again, but with a lower weight.
					weights[node.Vertex] += 0.05f;
					queue.Push(new QueueElement<RiverNode>(weights[node.Vertex], node));
				} else if (node.Right == null) {
					node.Right = new_node;
				}
				node_lookup[lowest] = new_node;
				queue.Push(new QueueElement<RiverNode>(weights[lowest], new_node));
			}
		}

		return rivernet;
	}

	private bool CheckValidAdjacent(RiverNode node, GBTCorner adjacent, MapArray<int> bitmask, Dictionary<GBTCorner, RiverNode> node_lookup) {
		bool result = true;
		result = result && ((bitmask[adjacent] & 2) < 2);
		result = result && (node.Downslope == null || node.Downslope.Vertex != adjacent);
		result = result && (!node_lookup.ContainsKey(adjacent));
		return result;
	}

	private MapArray<int> GenerateLandMask(MapArray<int> bitmask) {
		var simplex = new Simplex2D(43);

		int gbt_radius = (int)Mathf.Max(ChunkOrder, Mathf.Ceil(2.0f*Mathf.Log(1.0f + 1.00f*Radius)/Mathf.Log(7.0f))) - ChunkOrder + 1;

		for (int i = 0; i < Utils.Pow(7, gbt_radius); i++) {
			for (int j = 0; j < ChunkSize; j++) {
				GBTHex hex = new GBTHex(i*ChunkSize + j);
				Vector2 pos = hex.position;
				if (GetLandMaskValue(pos, simplex) > Threshold) {
					bitmask[hex] |= 1;
					foreach (GBTCorner corner in hex.GetCorners()) {
						bitmask[corner] |= 1;
					}
				} else {
					bitmask[hex] |= 2;
					foreach (GBTCorner corner in hex.GetCorners()) {
						bitmask[corner] |= 2;
					}
				}
			}
		}
		return bitmask;
	}

	private float GetLandMaskValue(Vector2 position, Simplex2D noise) {
		float noise_val = noise.GetFractalNoise(2.0f*position/Radius);
		float r = position.magnitude/Radius;
		if (noise_val > max_noise_value) {
			max_noise_value = noise_val;
		}
		return (noise_val*(1.0f - Alpha) + noise_val*Alpha/r)*(1.0f - Mathf.Pow(Utils.Smoothstep(r/2.0f), 5.0f));
	}

	private void DrawLandMask(MapArray<int> bitmask) {
		int gbt_radius = (int)Mathf.Max(ChunkOrder, Mathf.Ceil(2.0f*Mathf.Log(1.0f + 1.00f*Radius)/Mathf.Log(7.0f))) - ChunkOrder + 1;

		int count = 0;
		var chunks = new List<ChunkData>();
		for (int i = 0; i < Utils.Pow(7, gbt_radius); i++) {
			var chunk = new ChunkData(i, ChunkSize);
			for (int j = 0; j < ChunkSize; j++) {
				var hex = new GBTHex(i*ChunkSize + j);
				Color hex_color = Color.black;
				if ((bitmask[hex] & 1) > 0) {
					count += 1;
					hex_color = Color.white;
				}
				chunk.AddColors(hex_color);
			}
			chunks.Add(chunk);
		}
		Debug.Log(count);
		m_DrawPolygonEvent.Invoke(chunks);
	}

	private void DrawGrayscaleArray(MapArray<float> array, MapArray<int> bitmask, float minimum, float maximum) {
		int gbt_radius = (int)Mathf.Max(ChunkOrder, Mathf.Ceil(2.0f*Mathf.Log(1.0f + 1.00f*Radius)/Mathf.Log(7.0f))) - ChunkOrder + 1;

		int count = 0;
		var chunks = new List<ChunkData>();
		for (int i = 0; i < Utils.Pow(7, gbt_radius); i++) {
			var chunk = new ChunkData(i, ChunkSize);
			for (int j = 0; j < ChunkSize; j++) {
				var hex = new GBTHex(i*ChunkSize + j);
				if ((bitmask[hex] & 1) > 0) {
					var colors = new Color[7];
					var corners = hex.GetCorners();
					float average = 0.0f;
					for (int k = 0; k < 6; k++) {
						colors[1 + k] = GetGrayscale((array[corners[k]] - minimum)/(maximum - minimum));
						average += array[corners[k]]/6.0f;
					}
					colors[0] = GetGrayscale((average - minimum)/(maximum - minimum));
					chunk.AddColors(colors);
				} else {
					chunk.AddColors(null);
				}
			}
			chunks.Add(chunk);
		}
		Debug.Log(count);
		m_DrawPolygonEvent.Invoke(chunks);
	}

	private Color GetGrayscale(float x) {
		x = Utils.Clamp(x);
		return new Color(x, x, x, 1.0f);
	}
}
