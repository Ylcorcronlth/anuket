using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GBTCorner {
	// Constants

	private const int CornerMask = 0x10000000;
    private const int HexMask = 0x0fffffff;
	private const int Left = 0x20000000;

	// Fields

	private static GBTHex[] _Touches = {
        new GBTHex(5), new GBTHex(1), new GBTHex(0),
        new GBTHex(0), new GBTHex(2), new GBTHex(6)
	};

    private static GBTCorner[] _Adjacent = {
        new GBTCorner(Left | CornerMask | 13), new GBTCorner(Left | CornerMask | 1), new GBTCorner(Left | CornerMask | 5),
        new GBTCorner(CornerMask | 2), new GBTCorner(CornerMask | 43), new GBTCorner(CornerMask | 6)
    };

    // The integer representing the particular hex in generalized balanced ternary.
    int _value;

    // Properties

    // Accessor for value.
    public int value {
        get {
            return _value;
        }
    }

    public GBTHex hex {
        get {
            return new GBTHex(value & HexMask);
        }
    }

    public Vector2 position {
        get {
            return GetPosition();
        }
    }

    // Constructor
    public GBTCorner(GBTHex hex, CornerDirection direction) {
        // Value needs to be <= CornerMask.
        _value = hex.value | CornerMask | (direction > 0 ? Left : 0);
    }

    public GBTCorner(int value) {
        _value = value;
    }

    // Methods

    public static bool operator==(GBTCorner a, GBTCorner b) {
        return a._value == b._value;
    }

    public static bool operator!=(GBTCorner a, GBTCorner b) {
        return a._value != b._value;
    }

    public static implicit operator CubeCorner(GBTCorner a) {
        return a.GetCube();
    }

    public static explicit operator int(GBTCorner a) {
        return a._value;
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        } else {
            GBTCorner b = (GBTCorner)obj;
            return this == b;
        }
    }

    public override int GetHashCode() {
        return _value;
    }

    public CubeCorner GetCube() {
        var hex = CubeHex.FromGBT(_value & HexMask);
        return new CubeCorner(hex, ((_value & Left) > 0 ? CornerDirection.L : CornerDirection.R));
    }

    public Vector2 GetPosition() {
        return GetCube().GetPosition();
    }

    public List<GBTCorner> GetAdjacent() {
        var result = new List<GBTCorner>();
        for (int i = 0; i < 3; i++) {
            GBTCorner corner = _Adjacent[i + 3*((_value & Left) >> 29)];
            GBTHex new_hex = new GBTHex(_value & HexMask) + new GBTHex(corner._value & HexMask);
            result.Add(new GBTCorner(new_hex, ((corner._value & CornerMask) > 0 ? CornerDirection.L : CornerDirection.R)));
        }
        return result;
    }

    public List<GBTHex> GetTouches() {
        var result = new List<GBTHex>();
        for (int i = 0; i < 3; i++) {
            result.Add(this.hex + _Touches[i + 3*((_value & Left) >> 29)]);
        }
        return result;
    }

    public override string ToString() {
        return (_value & HexMask).ToString() + ((_value & CornerMask) > 0 ? "L" : "R");
    }
}
