public class Direction {
    public static Direction A {
        get {
            return new Direction(0);
        }
    }

    public static Direction B {
        get {
            return new Direction(1);
        }
    }

    public static Direction C {
        get {
            return new Direction(2);
        }
    }

    public static Direction D {
        get {
            return new Direction(3);
        }
    }

    public static Direction E {
        get {
            return new Direction(4);
        }
    }

    public static Direction F {
        get {
            return new Direction(5);
        }
    }

    public static Direction G {
        get {
            return new Direction(6);
        }
    }

    public static Direction H {
        get {
            return new Direction(7);
        }
    }

    private int value;

    private Direction(int v) {
        this.value = v;
    }
    public float Rotation() {
        return this.value * 45;
    }

    public void Next() {
        value = (value + 2) % 8;
    }

    public bool IsFlipped() {
        return value % 4 != 0;
    }
    
};