using UnityEngine;


[System.Serializable]
public struct HexChunkCoordinates {

	public int X { get; private set; }
	public int Z { get; private set; }

    //https://catlikecoding.com/unity/tutorials/hex-map/part-1/
	public HexChunkCoordinates (int x, int z) { 
		X = x;
		Z = z;
	}

    // public static HexCoordinates operator +(HexCoordinates a, HexCoordinates b){
	// 	a.X += b.X;
	// 	a.Z += b.Z;
	// 	return a;
	// }

	
}