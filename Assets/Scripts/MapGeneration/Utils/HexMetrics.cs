using UnityEngine;

public static class HexMetrics {

	public const int MapSize = 15; // diameter of the map in islands
	public const int MapRadius = MapSize / 2;
	public const int IslandSize = 65; // diameter of an island in cells, must be odd number, max size = 117
	public const int IslandRadius = IslandSize / 2;
	public const float HeightMultiplier = 8f; //multiply the y scale of the hex by this value
	public const float OuterRadius = .8f; // outer radius of hexcells
	public const float InnerRadius = OuterRadius * 0.866025404f; // inner radius of hexcells

	public const int NbHeightSteps = 128; // cells height will be distributed in 'n' steps 

	public const int DistanceBetweenIslands = 15; // must be odd for hex cells to connect correctly (otherwise the cells are not offsetted) 

	public const float IslandInnerRadius = (IslandRadius + 0.5f + // number of cells in radius + half center cell
											DistanceBetweenIslands / 2f) // half the distance between islands
											* OuterRadius * 1.5f; // the inner-radius(vertical) of an island(flat-top) is the sum of all it's cells(pointy-top) outer-radius(vertical). Each cell is separated vertically by 1.5 * outer

	public const float IslandOuterRadius = IslandInnerRadius / 0.866025404f;


	public static Vector3[] corners = {
		new Vector3(0f, 0f, OuterRadius), //top vertex
		new Vector3(InnerRadius, 0f, 0.5f * OuterRadius), // top right vertex
		new Vector3(InnerRadius, 0f, -0.5f * OuterRadius), // bottom right vertex
		new Vector3(0f, 0f, -OuterRadius), // bottom vertex
		new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius), // bottom left vertex
		new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius), // top left vertex
	};

	// public static Vector2[] uvCorners = {
	// 	new Vector2(0.3f, 0.55f), // top
	// 	new Vector2(0.45f, 0.37f), // top right
	// 	new Vector2(0.37f, 0.16f), // bottom right
	// 	new Vector2(0.15f, 0.12f), // bottom
	// 	new Vector2(0f, 0.29f), // bottom left
	// 	new Vector2(0.08f, 0.51f), // top left
	// 	new Vector2(0f, 0.96f), // top left face left vertex
	// 	new Vector2(0.23f, 1f), // top left face right vertex
	// 	new Vector2(0.66f, 0.84f), // top right face left vertex
	// 	new Vector2(0.8f, 0.67f), // top right face right vertex
	// 	new Vector2(0.88f, 0.22f), // right face left vertex
	// 	new Vector2(0.8f, 0f), // right face right vertex
	// };

	public static Vector2[] uvCorners = {
		new Vector2(0f, OuterRadius), //top vertex
		new Vector2(InnerRadius, 0.5f * OuterRadius), // top right vertex
		new Vector2(InnerRadius, -0.5f * OuterRadius), // bottom right vertex
		new Vector2(0f, -OuterRadius), // bottom vertex
		new Vector2(-InnerRadius, -0.5f * OuterRadius), // bottom left vertex
		new Vector2(-InnerRadius, 0.5f * OuterRadius), // top left vertex
	};
}