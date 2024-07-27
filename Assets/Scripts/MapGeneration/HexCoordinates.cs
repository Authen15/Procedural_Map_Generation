using UnityEngine;


[System.Serializable]
public struct HexCellCoordinates {

	public int X { get; private set; }
	public int Z { get; private set; }

    //https://catlikecoding.com/unity/tutorials/hex-map/part-1/
	public HexCellCoordinates (int x, int z) {
        
		X = x;
		Z = z;
	}

    public static HexCellCoordinates operator +(HexCellCoordinates a, HexCellCoordinates b){
		a.X += b.X;
		a.Z += b.Z;
		return a;
	}

	
}