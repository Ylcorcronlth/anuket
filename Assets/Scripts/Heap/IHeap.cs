using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeap<TKey> where TKey : System.IComparable {
	TKey GetRoot();
	TKey Pop();
	void Push(TKey item);
	// Should also handle taking an array or list and making it into a heap.
}
