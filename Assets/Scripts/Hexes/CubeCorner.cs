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
		new CubeCorner(2, -1, CornerDirection.L),
		new CubeCorner(1, 0, CornerDirection.L),
		new CubeCorner(1, -1, CornerDirection.L),
		new CubeCorner(-1, 1, CornerDirection.R),
		new CubeCorner(-2, 1, CornerDirection.R),
		new CubeCorner(-1, 0, CornerDirection.R)
    };

	private CubeHex _hex;
	private CornerDirection _direction;

    // # Constructors
	public CubeCorner(CubeHex hex, CornerDirection direction) {
		_hex = hex;
		_direction = direction;
	}

    public CubeCorner(int q, int r, CornerDirection direction) {
        _hex = new CubeHex(q, r);
        _direction = direction;
    }

    // # Properties
	public CubeHex hex {
		get {
            return _hex;
        }
	}

    public int q {
        get {
            return _hex.q;
        }
    }

    public int r {
        get {
            return _hex.r;
        }
    }

    public int s {
        get {
            return _hex.s;
        }
    }

	public CornerDirection direction {
		get {
            return _direction;
        }
	}

	public Vector2 position {
		get {
            return GetPosition();
        }
	}

    public List<CubeHex> touches {
        get {
            return GetTouches();
        }
    }

    public List<CubeCorner> adjacent {
        get {
            return GetAdjacent();
        }
    }

    // # Methods
    public static bool operator==(CubeCorner a, CubeCorner b) {
        return a._hex == b._hex && a._direction == b._direction;
    }

    public static bool operator!=(CubeCorner a, CubeCorner b) {
        return a._hex != b._hex || a._direction != b._direction;
    }

    public static implicit operator GBTCorner(CubeCorner a) {
        return a.GetGBT();
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

    public Vector2 GetPosition() {
        return _hex.position + _Offsets[(int)_direction];
    }

    public GBTCorner GetGBT() {
        return new GBTCorner(_hex.GetGBT(), _direction);
    }

    public List<CubeCorner> GetAdjacent() {
        var result = new List<CubeCorner>();
        for (int i = 0; i < 3; i++) {
            CubeCorner offset = _Adjacent[i + 3*(int)_direction];
            result.Add(new CubeCorner(_hex + offset._hex, offset._direction));
        }
        return result;
    }

    public List<CubeHex> GetTouches() {
        var result = new List<CubeHex>();
        for (int i = 0; i < 3; i++) {
            CubeHex offset = _Touches[i + 3*(int)_direction];
            result.Add(offset + _hex);
        }
        return result;
    }
}
