//https://catlikecoding.com/unity/tutorials/hex-map/part-1/
[System.Serializable]
public struct HexChunkCoordinates {

	public int X { get; private set; }
	public int Z { get; private set; }

	public HexChunkCoordinates (int x, int z) { 
		X = x;
		Z = z;
	}
}