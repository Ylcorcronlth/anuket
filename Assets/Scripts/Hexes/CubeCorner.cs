using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCorner {
    // # Constants
	private static Vector2[] _Offsets = {
		new Vector2(0.57735f, 0.0f),
		new Vector2(-0.57735f, 0.0f)
	};

    // # Fields
    private static CubeHex[] _Touches = {
		new CubeHex(0, 0, 0),
		new CubeHex(1, 0, -1),
		new CubeHex(1, -1, 0),
		new CubeHex(0, 0, 0),
		new CubeHex(-1, 1, 0),
		new CubeHex(-1, 0, 1)
    };

    private static CubeCorner[] _Adjacent = {
		new CubeCorner(2, -1, Direction.L),
		new CubeCorner(1, 0, Direction.L),
		new CubeCorner(1, -1, Direction.L),
		new CubeCorner(-1, 1, Direction.R),
		new CubeCorner(-2, 1, Direction.R),
		new CubeCorner(-1, 0, Direction.R)
    };

	private CubeHex _hex;
	private Direction _direction;

    // # Constructors
	public CubeCorner(CubeHex hex, Direction direction) {
		_hex = hex;
		_direction = direction;
	}

    public CubeCorner(int q, int r, Direction direction) {
        _hex = new CubeHex(q, r);
        _direction = direction;
    }

	// # Enums
	public enum Direction { R, L };

    // # Properties
	public CubeHex hex {
		get { return _hex; }
	}

    public int q {
        get { return _hex.q; }
    }

    public int r {
        get { return _hex.r; }
    }

    public int s {
        get { return _hex.s; }
    }

	public Direction direction {
		get { return _direction; }
	}

	public Vector2 position {
		get { return _hex.position + _Offsets[(int)_direction]; }
	}

    public List<CubeHex> touches {
        get {
            var list_touches = new List<CubeHex>();
            for (int i = 0; i < 3; i++) {
                CubeHex offset = _Touches[i + 3*(int)_direction];
                list_touches.Add(offset + _hex);
            }
            return list_touches;
        }
    }

    public List<CubeCorner> adjacent {
        get {
            var list_adjacent = new List<CubeCorner>();
            for (int i = 0; i < 3; i++) {
                CubeCorner offset = _Adjacent[i + 3*(int)_direction];
                list_adjacent.Add(new CubeCorner(_hex + offset._hex, offset._direction));
            }
            return list_adjacent;
        }
    }

    // # Methods
    public static bool operator==(CubeCorner a, CubeCorner b) {
        return a._hex == b._hex && a._direction == b._direction;
    }

    public static bool operator!=(CubeCorner a, CubeCorner b) {
        return a._hex != b._hex || a._direction != b._direction;
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        } else {
            CubeCorner b = (CubeCorner)obj;
            return this == b;
        }
    }

    public override int GetHashCode() {
        unchecked {
            int result = 99989;
            result = result*496187739 + _hex.GetHashCode();
            result = result*496187739 + (int)_direction;
            return result;
        }
    }
}
