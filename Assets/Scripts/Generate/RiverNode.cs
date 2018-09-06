using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverNode {
	private GBTCorner vertex;
	private RiverNode downslope, left, right;
	private float width;

	public GBTCorner Vertex {
		get {
			return vertex;
		}
		set {
			vertex = value;
		}
	}

	public RiverNode(GBTCorner vert) {
		vertex = vert;
		width = -1.0f;
		downslope = left = right = null;
	}

	public RiverNode Downslope {
		get {
			return downslope;
		}
		set {
			downslope = value;
		}
	}

	public RiverNode Left {
		get {
			return left;
		}
		set {
			left = value;
		}
	}

	public RiverNode Right {
		get {
			return right;
		}
		set {
			right = value;
		}
	}

	public float Width {
		get {
			return width;
		}
		set {
			width = value;
		}
	}

	public override int GetHashCode() {
		return vertex.GetHashCode();
	}
}
