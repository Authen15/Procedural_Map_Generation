using UnityEngine;

public static class HexMetrics {

	public const float heightMultiplier = 60f; //multiply the y scale of the hex by this value

	public const float outerRadius = 1f;

	public const float innerRadius = outerRadius * 0.866025404f;

	public const int nbHeightSteps = 128;

	public static Vector3Int WorldPositionToCellPosition(Vector3 position){
		float x = position.x;
		float y = position.y;
		float z = position.z;

        int cellX = (int) ((x / (HexMetrics.innerRadius * 2f)) + z / 2 - z * 0.5f) +1;
        int cellY = (int) (y * HexMetrics.heightMultiplier / 2); 
        int cellZ = (int) (z / (HexMetrics.outerRadius * 1.5f));

        return new Vector3Int(cellX, cellY, cellZ);
	}

	public static Vector3 CellPositionToWorldPosition(Vector3 position)
    {
		int x = (int)position.x;
		float y = position.y;
		int z = (int)position.z;
		

        float posX = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        float posY = y * HexMetrics.heightMultiplier / 2; 
        float posZ = z * (HexMetrics.outerRadius * 1.5f);

        return new Vector3(posX, posY, posZ);
    }

}