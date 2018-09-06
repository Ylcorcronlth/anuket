using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GBTHex {
    // # Constants

    // # Fields

    private static int[] _Neighbors = { 1, 3, 2, 6, 4, 5 };
    private static int[] _Corners = { 0, 1, 2, 0, 6, 5 };

    // Lookup table for going GBT -> Cube.
    // Just a table of cube hex values corresponding to the digit in each possible location.
    // In theory we could go up to 11 with a 32 bit integer, but it might be nice to have the
    // extra bits for e.g. corners.
    private static CubeHex[,] decode = new CubeHex[,] {
        { // 0
            new CubeHex(0, 0, 0),
            new CubeHex(1, 0, -1),
            new CubeHex(-1, 1, 0),
            new CubeHex(0, 1, -1),
            new CubeHex(0, -1, 1),
            new CubeHex(1, -1, 0),
            new CubeHex(-1, 0, 1)
        },
        { // 1
            new CubeHex(0, 0, 0),
            new CubeHex(3, -1, -2),
            new CubeHex(-2, 3, -1),
            new CubeHex(1, 2, -3),
            new CubeHex(-1, -2, 3),
            new CubeHex(2, -3, 1),
            new CubeHex(-3, 1, 2)
        },
        { // 2
            new CubeHex(0, 0, 0),
            new CubeHex(8, -5, -3),
            new CubeHex(-3, 8, -5),
            new CubeHex(5, 3, -8),
            new CubeHex(-5, -3, 8),
            new CubeHex(3, -8, 5),
            new CubeHex(-8, 5, 3)
        },
        { // 3
            new CubeHex(0, 0, 0),
            new CubeHex(19, -18, -1),
            new CubeHex(-1, 19, -18),
            new CubeHex(18, 1, -19),
            new CubeHex(-18, -1, 19),
            new CubeHex(1, -19, 18),
            new CubeHex(-19, 18, 1)
        },
        { // 4
            new CubeHex(0, 0, 0),
            new CubeHex(39, -55, 16),
            new CubeHex(16, 39, -55),
            new CubeHex(55, -16, -39),
            new CubeHex(-55, 16, 39),
            new CubeHex(-16, -39, 55),
            new CubeHex(-39, 55, -16)
        },
        { // 5
            new CubeHex(0, 0, 0),
            new CubeHex(62, -149, 87),
            new CubeHex(87, 62, -149),
            new CubeHex(149, -87, -62),
            new CubeHex(-149, 87, 62),
            new CubeHex(-87, -62, 149),
            new CubeHex(-62, 149, -87)
        },
        { // 6
            new CubeHex(0, 0, 0),
            new CubeHex(37, -360, 323),
            new CubeHex(323, 37, -360),
            new CubeHex(360, -323, -37),
            new CubeHex(-360, 323, 37),
            new CubeHex(-323, -37, 360),
            new CubeHex(-37, 360, -323)
        },
        { // 7
            new CubeHex(0, 0, 0),
            new CubeHex(-249, -757, 1006),
            new CubeHex(1006, -249, -757),
            new CubeHex(757, -1006, 249),
            new CubeHex(-757, 1006, -249),
            new CubeHex(-1006, 249, 757),
            new CubeHex(249, 757, -1006)
        },
        { // 8
            new CubeHex(0, 0, 0),
            new CubeHex(-1504, -1265, 2769),
            new CubeHex(2769, -1504, -1265),
            new CubeHex(1265, -2769, 1504),
            new CubeHex(-1265, 2769, -1504),
            new CubeHex(-2769, 1504, 1265),
            new CubeHex(1504, 1265, -2769)
        },
        { // 9
            new CubeHex(0, 0, 0),
            new CubeHex(-5777, -1026, 6803),
            new CubeHex(6803, -5777, -1026),
            new CubeHex(1026, -6803, 5777),
            new CubeHex(-1026, 6803, -5777),
            new CubeHex(-6803, 5777, 1026),
            new CubeHex(5777, 1026, -6803)
        },
        { // 10
            new CubeHex(0, 0, 0),
            new CubeHex(-18357, 3725, 14632),
            new CubeHex(14632, -18357, 3725),
            new CubeHex(-3725, -14632, 18357),
            new CubeHex(3725, 14632, -18357),
            new CubeHex(-14632, 18357, -3725),
            new CubeHex(18357, -3725, -14632)
        }
    };

    private static int[,,] Addition = {
        {
            {0, 1, 2, 3, 4, 5, 6},
            {1, 9, 3, 25, 5, 13, 0},
            {2, 3, 18, 19, 6, 0, 43},
            {3, 25, 19, 27, 0, 1, 2},
            {4, 5, 6, 0, 29, 37, 31},
            {5, 13, 0, 1, 37, 38, 4},
            {6, 0, 43, 2, 31, 4, 47}
        },
        {
            {1, 9, 3, 25, 5, 13, 0},
            {9, 10, 25, 26, 13, 7, 1},
            {3, 25, 19, 27, 0, 1, 2},
            {25, 26, 27, 21, 1, 9, 3},
            {5, 13, 0, 1, 37, 38, 4},
            {13, 7, 1, 9, 38, 11, 5},
            {0, 1, 2, 3, 4, 5, 6}
        },
        {
            {2, 3, 18, 19, 6, 0, 43},
            {3, 25, 19, 27, 0, 1, 2},
            {18, 19, 20, 14, 43, 2, 45},
            {19, 27, 14, 15, 2, 3, 18},
            {6, 0, 43, 2, 31, 4, 47},
            {0, 1, 2, 3, 4, 5, 6},
            {43, 2, 45, 18, 47, 6, 42}
        },
        {
            {3, 25, 19, 27, 0, 1, 2},
            {25, 26, 27, 21, 1, 9, 3},
            {19, 27, 14, 15, 2, 3, 18},
            {27, 21, 15, 23, 3, 25, 19},
            {0, 1, 2, 3, 4, 5, 6},
            {1, 9, 3, 25, 5, 13, 0},
            {2, 3, 18, 19, 6, 0, 43}
        },
        {
            {4, 5, 6, 0, 29, 37, 31},
            {5, 13, 0, 1, 37, 38, 4},
            {6, 0, 43, 2, 31, 4, 47},
            {0, 1, 2, 3, 4, 5, 6},
            {29, 37, 31, 4, 33, 41, 28},
            {37, 38, 4, 5, 41, 35, 29},
            {31, 4, 47, 6, 28, 29, 30}
        },
        {
            {5, 13, 0, 1, 37, 38, 4},
            {13, 7, 1, 9, 38, 11, 5},
            {0, 1, 2, 3, 4, 5, 6},
            {1, 9, 3, 25, 5, 13, 0},
            {37, 38, 4, 5, 41, 35, 29},
            {38, 11, 5, 13, 35, 36, 37},
            {4, 5, 6, 0, 29, 37, 31}
        },
        {
            {6, 0, 43, 2, 31, 4, 47},
            {0, 1, 2, 3, 4, 5, 6},
            {43, 2, 45, 18, 47, 6, 42},
            {2, 3, 18, 19, 6, 0, 43},
            {31, 4, 47, 6, 28, 29, 30},
            {4, 5, 6, 0, 29, 37, 31},
            {47, 6, 42, 43, 30, 31, 46}
        }
    };

    // The integer representing the particular hex in generalized balanced ternary.
    int _value;

    // Constructors
    public GBTHex(int value) {
        // Value should be less than 0x10000000.
        _value = value;
    }

    // Properties

    // Accessor for value.
    public int value {
        get {
            return _value;
        }
    }

    public Vector2 position {
        get {
            return GetPosition();
        }
    }

    // Methods
    public static GBTHex operator+(GBTHex a, GBTHex b) {
        int carry = 0;
        int result = 0;
        int magnitude = 1;
        int x = a.value, y = b.value;
        while (x > 0 | y > 0 | carry > 0) {
            int s = Addition[carry, Utils.Mod(x, 7), Utils.Mod(y, 7)];
            carry = s/7;
            result += magnitude*(s % 7);
            magnitude *= 7;
            x = x/7;
            y = y/7;
        }
        return new GBTHex(result);
    }

    public static GBTHex operator-(GBTHex a, GBTHex b) {
        return a + b.Negate();
    }

    public static bool operator==(GBTHex a, GBTHex b) {
        return a.value == b.value;
    }

    public static bool operator!=(GBTHex a, GBTHex b) {
        return a.value != b.value;
    }

    public static implicit operator CubeHex(GBTHex a) {
        return a.GetCube();
    }

    public static implicit operator FractionalCube(GBTHex a) {
        return (FractionalCube)a.GetCube();
    }

    public static explicit operator int(GBTHex a) {
        return a._value;
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        } else {
            GBTHex b = (GBTHex)obj;
            return this == b;
        }
    }

    public override int GetHashCode() {
        return _value;
    }

    public GBTHex Negate() {
        int result = 0;
        int magnitude = 1;
        int x = value;
        while (x > 0) {
            result += magnitude * (7 - Utils.Mod(x, 7));
            x = x/7;
        }
        return new GBTHex(result);
    }

    public CubeHex GetCube() {
        CubeHex pos = new CubeHex(0, 0, 0);
        int x = value;
        int i = 0;
        while (x > 0) {
            int k = x % 7;
            x /= 7;
            pos += decode[i, k];
            i += 1;
        }
        return pos;
    }

    public Vector2 GetPosition() {
        return GetCube().GetPosition();
    }

    public List<GBTHex> GetNeighbors() {
        var result = new List<GBTHex>();
        for (int i = 0; i < 6; i++) {
            result.Add(this + new GBTHex(_Neighbors[i]));
        }
        return result;
    }

    public List<GBTCorner> GetCorners() {
        var result = new List<GBTCorner>();
        for (int i = 0; i < 6; i++) {
            result.Add(new GBTCorner(this + new GBTHex(_Corners[i]), (i % 2 == 0 ? CornerDirection.R : CornerDirection.L)));
        }
        return result;
    }

    public override string ToString() {
        return _value.ToString();
    }

    public static GBTHex FromCartesian(Vector2 position) {
        return (GBTHex)CubeHex.FromCartesian(position);
    }

    public bool isValid() {
        return _value >= 0;
    }
}
