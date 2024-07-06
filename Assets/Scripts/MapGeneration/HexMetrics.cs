using UnityEngine;

public static class HexMetrics {

	public const int MapSize = 1024;
	public const float heightMultiplier = 30f; //multiply the y scale of the hex by this value
	public const float outerRadius = 2f;
	public const float innerRadius = outerRadius * 0.866025404f;
	public const int nbHeightSteps = 40;

	public static Vector3Int WorldPositionToCellPosition(Vector3 position){
		float x = position.x;
		float y = position.y;
		float z = position.z;

        int cellX = (int) ((x / (HexMetrics.innerRadius * 2f)) + z / 2 - z * 0.5f) +1;
        int cellY = (int) (y * HexMetrics.heightMultiplier / 2); 
        int cellZ = (int) (z / (HexMetrics.outerRadius * 1.5f));

        return new Vector3Int(cellX, cellY, cellZ);
	}

	// public static Vector3 CellPositionToWorldPosition(Vector3 position)
    // {
	// 	int x = (int)position.x;
	// 	float y = position.y;
	// 	int z = (int)position.z;
		

    //     float posX = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
    //     float posY = y * HexMetrics.heightMultiplier / 2; 
    //     float posZ = z * (HexMetrics.outerRadius * 1.5f);

    //     return new Vector3(posX, posY, posZ);
    // }

	public static Vector3 CellGridToWorldPos(int x, float y, int z)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (innerRadius * 2f);
		position.y = y * heightMultiplier;
		position.z = z * (outerRadius * 1.5f);
		return position;
	}

	public static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius), //top vertex
		new Vector3(innerRadius, 0f, 0.5f * outerRadius), // top right vertex
		new Vector3(innerRadius, 0f, -0.5f * outerRadius), // bottom right vertex
		new Vector3(0f, 0f, -outerRadius), // bottom vertex
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius), // bottom left vertex
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius), // top left vertex
	};

	public static Vector2[] uvCorners = {
		new Vector2(0.3f, 0.55f), // top
		new Vector2(0.45f, 0.37f), // top right
		new Vector2(0.37f, 0.16f), // bottom right
		new Vector2(0.15f, 0.12f), // bottom
		new Vector2(0f, 0.29f), // bottom left
		new Vector2(0.08f, 0.51f), // top left
		new Vector2(0f, 0.96f), // top left face left vertex
		new Vector2(0.23f, 1f), // top left face right vertex
		new Vector2(0.66f, 0.84f), // top right face left vertex
		new Vector2(0.8f, 0.67f), // top right face right vertex
		new Vector2(0.88f, 0.22f), // right face left vertex
		new Vector2(0.8f, 0f), // right face right vertex
	};

	// public static Vector3[] corners = {
	// 	new Vector3(0f, 0f, outerRadius), //top vertex
	// 	new Vector3(innerRadius, 0f, 0.5f * outerRadius), // top right vertex
	// 	new Vector3(innerRadius, 0f, -0.5f * outerRadius), // bottom right vertex
	// 	new Vector3(0f, 0f, -outerRadius), // bottom vertex
	// 	new Vector3(-innerRadius, 0f, -0.5f * outerRadius), // bottom left vertex
	// 	new Vector3(-innerRadius, 0f, 0.5f * outerRadius), // top left vertex
	// 	new Vector3(0f, 0f, outerRadius), //top vertex

	// };

}