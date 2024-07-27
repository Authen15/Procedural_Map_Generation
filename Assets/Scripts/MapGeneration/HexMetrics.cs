using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public static class HexMetrics {

	public const int MapSize = 6;
	public const int ChunkSize = 17; //max 119 // must be odd number
	public const float heightMultiplier = 30f; //multiply the y scale of the hex by this value
	public const float outerRadius = 1f;
	public const float innerRadius = outerRadius * 0.866025404f;

	public const int nbHeightSteps = 40;


	private const float DistanceBetweenIslands = 3f;

	public const float ChunkInnerRadius = ChunkSize/2 * outerRadius * 1.5f + outerRadius + DistanceBetweenIslands*outerRadius;
	public const float ChunkOuterRadius = ChunkInnerRadius / 0.866025404f;


	// public static Vector3Int WorldPositionToCellPosition(Vector3 position){
	// 	float x = position.x;
	// 	float y = position.y;
	// 	float z = position.z;

    //     int cellX = (int) ((x / (HexMetrics.innerRadius * 2f)) + z / 2 - z * 0.5f) +1;
    //     int cellY = (int) (y * HexMetrics.heightMultiplier / 2); 
    //     int cellZ = (int) (z / (HexMetrics.outerRadius * 1.5f));

    //     return new Vector3Int(cellX, cellY, cellZ);
	// }

	public static Vector3 HexToWorld(HexCellCoordinates hexCellCoordinates)
	{
		float x = hexCellCoordinates.X;
		float z = hexCellCoordinates.Z;
		Vector3 position;

		position.x = x * (innerRadius * 2f) + z * innerRadius;
		position.y = 0;
		position.z = z * (outerRadius * 1.5f);
		return position;
	}

	public static Vector3 HexChunkToWorld(HexChunkCoordinates hexChunkCoordinates)
	{
		float x = hexChunkCoordinates.X;
		float z = hexChunkCoordinates.Z;
		Vector3 position;

		position.x = x * (ChunkOuterRadius * 1.5f);
		position.y = 0;
		position.z = z * (ChunkInnerRadius * 2f) + x * ChunkInnerRadius;

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