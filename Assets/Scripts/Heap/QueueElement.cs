using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QueueElement<TValue> : System.IComparable {
	// A very simple helper class for working with heaps.
	// Combines an arbitrary value with a priority.
	public float priority;
	public TValue value;

	public QueueElement(float priority, TValue value) {
		this.priority = priority;
		this.value = value;
	}

	public int CompareTo(object obj) {
		if (obj == null || GetType() != obj.GetType()) {
			return 0;
		} else {
			QueueElement<TValue> b = (QueueElement<TValue>)obj;
			return priority.CompareTo(b.priority);
		}
	}
}
