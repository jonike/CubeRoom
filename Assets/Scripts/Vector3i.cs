
[System.Serializable]
public struct Vector3i {
	public int x;
	public int y;
	public int z;

	public Vector3i(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static Vector3i zero { 
		get {
			return new Vector3i(0,0,0);
		}  
	}
}
