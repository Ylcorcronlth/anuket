using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArray<TValue> {
	private Dictionary<GBTHex, TValue> hex_data;
	private Dictionary<GBTCorner, TValue> corner_data;
	private TValue default_value;

	public MapArray(TValue default_value) {
		hex_data = new Dictionary<GBTHex, TValue>();
		corner_data = new Dictionary<GBTCorner, TValue>();
		this.default_value = default_value;
	}

	public TValue GetValue(GBTHex key) {
		TValue output;
		if (hex_data.TryGetValue(key, out output)) {
			return output;
		} else {
			return default_value;
		}
	}

	public void SetValue(GBTHex key, TValue value) {
		hex_data[key] = value;
	}

	public TValue GetValue(GBTCorner key) {
		TValue output;
		if (corner_data.TryGetValue(key, out output)) {
			return output;
		} else {
			return default_value;
		}
	}

	public void SetValue(GBTCorner key, TValue value) {
		corner_data[key] = value;
	}

	public TValue this[GBTHex key] {
		get {
			return GetValue(key);
		}
		set {
			SetValue(key, value);
		}
	}

	public TValue this[GBTCorner key] {
		get {
			return GetValue(key);
		}
		set {
			SetValue(key, value);
		}
	}

	public IEnumerable<KeyValuePair<GBTHex, TValue>> GetHexEnumerator() {
		foreach (KeyValuePair<GBTHex, TValue> kvp in hex_data) {
			yield return kvp;
		}
	}

	public IEnumerable<KeyValuePair<GBTCorner, TValue>> GetCornerEnumerator() {
		foreach (KeyValuePair<GBTCorner, TValue> kvp in corner_data) {
			yield return kvp;
		}
	}
}
