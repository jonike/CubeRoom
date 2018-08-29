using UnityEngine;
using Sorumi.Util;
public class Direction
{

    public static Direction A = new Direction(0, new Vector3(0, 0, 1));

    public static Direction B = new Direction(1, new Vector3(1, 0, 1));


    public static Direction C = new Direction(2, new Vector3(1, 0, 0));


    public static Direction D = new Direction(3, new Vector3(1, 0, -1));


    public static Direction E = new Direction(4, new Vector3(0, 0, -1));

    public static Direction F = new Direction(5, new Vector3(-1, 0, -1));

    public static Direction G = new Direction(6, new Vector3(-1, 0, 0));

    public static Direction H = new Direction(7, new Vector3(-1, 0, 1));

    private static Direction[] directions = { A, B, C, D, E, F, G, H };
    public int Value;
    public Vector3 Vector;

    public static Direction Degree(float degree)
    {
        int index = ((int)degree / 90) * 2;
        return directions[index];
    }

    private Direction(int v, Vector3 vec)
    {
        this.Value = v;
        this.Vector = vec;
    }
    public float Rotation()
    {
        return this.Value * 45;
    }

    public Direction Next()
    {
        return directions[(Value + 2) % 8];
    }

    public bool IsFlipped()
    {
        return Value % 4 != 0;
    }

    public override string ToString()
    {
        return Value + " " + Vector.ToString();
    }
    // override object.Equals
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return Value == ((Direction)obj).Value;
        // return base.Equals(obj);
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return Value;
        // return base.GetHashCode();
    }

};