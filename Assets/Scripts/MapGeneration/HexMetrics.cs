using UnityEngine;

public static class HexMetrics {

	public const int MapSize = 1; // size of the map in chunks
	public const int ChunkSize = 5; // must be odd number
	public const int MapCellGridSize = MapSize * ChunkSize;
	public const float HeightMultiplier = 10f; //multiply the y scale of the hex by this value
	public const float OuterRadius = 1f;
	public const float InnerRadius = OuterRadius * 0.866025404f;

	public const int NbHeightSteps = 40;

	private const float DistanceBetweenIslands = 3f;

	public const float ChunkInnerRadius = ChunkSize/2 * OuterRadius * 1.5f + OuterRadius + DistanceBetweenIslands*OuterRadius;
	public const float ChunkOuterRadius = ChunkInnerRadius / 0.866025404f;



	public static Vector3[] corners = {
		new Vector3(0f, 0f, OuterRadius), //top vertex
		new Vector3(InnerRadius, 0f, 0.5f * OuterRadius), // top right vertex
		new Vector3(InnerRadius, 0f, -0.5f * OuterRadius), // bottom right vertex
		new Vector3(0f, 0f, -OuterRadius), // bottom vertex
		new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius), // bottom left vertex
		new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius), // top left vertex
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