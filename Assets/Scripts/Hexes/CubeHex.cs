using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CubeHex {
    // # Constants
    private const float _QX = 0.86602540378443864676372317075294f;
    private const float _QY = 0.5f;
    private const float _RY = 1.0f;
    private const float _XQ = 1.1547005383792515290182975610039f;
    private const float _XR = 1.7320508075688772935274463415059f;
    private const float _YR = 1.0f;

    // # Fields
    private static readonly CubeHex[] _Neighbors = {
        new CubeHex(1, 0, -1),
        new CubeHex(0, 1, -1),
        new CubeHex(-1, 1, 0),
        new CubeHex(-1, 0, 1),
        new CubeHex(0, -1, 1),
        new CubeHex(1, -1, 0)
    };

    private static readonly CubeCorner[] _Corners = {
        new CubeCorner(0, 0, CornerDirection.R),
        new CubeCorner(1, 0, CornerDirection.L),
        new CubeCorner(-1, 1, CornerDirection.R),
        new CubeCorner(0, 0, CornerDirection.L),
        new CubeCorner(-1, 0, CornerDirection.R),
        new CubeCorner(1, -1, CornerDirection.L)
    };

    // Matrix for going Cube -> GBT.
    // Rotates and scales in order to remove the lowest digit from the cube coordinate.
    private static int _Ma = 2, _Mb = -1, _Mc = 1, _Md = 3, _Div = 7;

    // The three coordinates of the hex. q + r + s == 0.
    // The basis vectors are oriented 120 degrees from each other, with
    // the first oriented along the x-axis.
    private int _q, _r, _s;

    // # Constructors
    public CubeHex(int q, int r) {
        _q = q;
        _r = r;
        _s = -q - r;
    }

    public CubeHex(int q, int r, int s) {
        // Assumes that q + r + s == 0.
        _q = q;
        _r = r;
        _s = s;
    }

    // # Properties
    // Accessors for the coordinates.
    public int q {
        get { return _q; }
    }

    public int r {
        get { return _r; }
    }

    public int s {
        get { return _s; }
    }

    public List<CubeHex> neighbors {
        get {
            return GetNeighbors();
        }
    }

    public List<CubeCorner> corners {
        get {
            return GetCorners();
        }
    }

    // The Cartesian position of the hex.
    public Vector2 position {
        get {
            return GetPosition();
        }
    }

    // # Indexers
    public int this[int k] {
        get {
            if (k == 0) {
                return _q;
            } else if (k == 1) {
                return _r;
            } else if (k == 2) {
                return _s;
            } else {
                throw new System.IndexOutOfRangeException("Index was out of range. Must be 0, 1, or 2.");
            }
        }
    }

    // # Methods
    public static CubeHex operator+(CubeHex a, CubeHex b) {
        return new CubeHex(a._q + b._q, a._r + b._r, a._s + b._s);
    }

    public static CubeHex operator-(CubeHex a, CubeHex b) {
        return new CubeHex(a._q - b._q, a._r - b._r, a._s - b._s);
    }

    public static CubeHex operator*(int k, CubeHex a) {
        return new CubeHex(k*a._q, k*a._r, k*a._s);
    }

    public static CubeHex operator*(CubeHex a, int k) {
        return new CubeHex(k*a._q, k*a._r, k*a._s);
    }

    public static bool operator==(CubeHex a, CubeHex b) {
        return a._q == b._q && a._r == b._r;
    }

    public static bool operator!=(CubeHex a, CubeHex b) {
        return a._q != b._q || a._r != b._r;
    }

    public static implicit operator FractionalCube(CubeHex a) {
        return new FractionalCube(a._q, a._r, a._s);
    }

    public static implicit operator GBTHex(CubeHex a) {
        return a.GetGBT();
    }

    public static explicit operator int(CubeHex a) {
        return a.GetGBT().value;
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        } else {
            CubeHex b = (CubeHex)obj;
            return this == b;
        }
    }

    public override int GetHashCode() {
        unchecked {
            int result = 99989;
            result = result*496187739 + _q;
            result = result*496187739 + _r;
            return result;
        }
    }

    public override string ToString() {
        return "(" + _q + ", " + _r + ", " + _s + ")";
    }

    public Vector2 GetPosition() {
        return new Vector2(_QX*q, _QY*q + _RY*_r);
    }

    public GBTHex GetGBT() {
        int value = 0;
        int q = _q, r = _r;
        int magnitude = 1;
        while (q > 0 || r > 0) {
            // Find the value of the current lowest digit.
            value += magnitude*Utils.Mod(q + 3*r, 7);
            // Rotate and scale the cube coords so as to move the next digit into place.
            int qn = q, rn = r;
            q = (_Ma*qn + _Mb*rn)/_Div;
            r = (_Mc*qn + _Md*rn)/_Div;
            magnitude *= 7;
        }
        return new GBTHex(value);
    }

    public static CubeHex FromCartesian(Vector2 position) {
        return FractionalCube.FromCartesian(position).GetRounded();
    }

    public static CubeHex FromGBT(int value) {
        return new GBTHex(value).GetCube();
    }

    public List<CubeHex> GetNeighbors() {
        var result = new List<CubeHex>(6);
        for (int i = 0; i < 6; i++) {
            result.Add(this + _Neighbors[i]);
        }
        return result;
    }

    public List<CubeCorner> GetCorners() {
        var result = new List<CubeCorner>(6);
        for (int i = 0; i < 6; i++) {
            CubeCorner offset = _Corners[i];
            result.Add(new CubeCorner(this + offset.hex, offset.direction));
        }
        return result;
    }
}
