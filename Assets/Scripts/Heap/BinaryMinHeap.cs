using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryMinHeap<TKey> : IHeap<TKey> where TKey : System.IComparable {
    // # Fields
	// todo: replace with an actual array and handle resizing internally.
	private List<TKey> array;

    // # Constructors
	public BinaryMinHeap() {
		array = new List<TKey>();
	}

    // # Properties
	public int Count {
		get { return array.Count; }
	}

    // # Methods
	public TKey GetRoot() {
		return array[0];
	}

	public string ToNiceString() {
		if (array.Count == 0) {
			return "-";
		}
		string output = array[0].ToString();
		for (int i = 1; i < array.Count; i++) {
			output += ", " + array[i].ToString();
		}
		return output;
	}

	public TKey Pop() {
		TKey root = array[0];

		array[0] = array[array.Count - 1];
		array.RemoveAt(array.Count - 1);
		ReHeapify(0);
		return root;
	}

	public void Push(TKey value) {
		array.Add(value);
		int i = array.Count - 1;
		while (i != 0 && array[GetParent(i)].CompareTo(array[i]) > 0) {
			SwapElements(i, GetParent(i));
			i = GetParent(i);
		}
	}

	private int GetParent(int k) {
		return (k - 1) >> 1;
	}

	private int GetLeftChild(int k) {
		return 2*k + 1;
	}

	private int GetRightChild(int k) {
		return 2*k + 2;
	}

	private void SwapElements(int k1, int k2) {
		TKey value = array[k1];
		array[k1] = array[k2];
		array[k2] = value;
	}

	private void ReHeapify(int i) {
		int left = GetLeftChild(i);
		int right = GetRightChild(i);
		int smallest = i;
		if (left < array.Count && array[left].CompareTo(array[smallest]) < 0) {
			smallest = left;
		}
		if (right < array.Count && array[right].CompareTo(array[smallest]) < 0) {
			smallest = right;
		}
		if (smallest != i) {
			SwapElements(i, smallest);
			ReHeapify(smallest);
		}
	}
}
