using UnityEngine;
public class Direction {
    public static Direction A {
        get {
            return new Direction(0, new Vector3(1, 0, 0));
        }
    }

    public static Direction B {
        get {
            return new Direction(1, new Vector3(1, 0, -1));
        }
    }

    public static Direction C {
        get {
            return new Direction(2, new Vector3(0, 0, -1));
        }
    }

    public static Direction D {
        get {
            return new Direction(3, new Vector3(-1, 0, -1));
        }
    }

    public static Direction E {
        get {
            return new Direction(4, new Vector3(-1, 0, 0));
        }
    }

    public static Direction F {
        get {
            return new Direction(5, new Vector3(-1, 0, 1));
        }
    }

    public static Direction G {
        get {
            return new Direction(6, new Vector3(0, 0, 1));
        }
    }

    public static Direction H {
        get {
            return new Direction(7, new Vector3(1, 0, 1));
        }
    }

    public int Value;
    public Vector3 Vector;

    private Direction(int v, Vector3 vec) {
        this.Value = v;
        this.Vector = vec;
    }
    public float Rotation() {
        return this.Value * 45;
    }

    public void Next() {
        Value = (Value + 2) % 8;
    }

    public bool IsFlipped() {
        return Value % 4 != 0;
    }
    
};