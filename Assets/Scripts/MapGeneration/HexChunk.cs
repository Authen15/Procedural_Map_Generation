using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexChunk : MonoBehaviour {

	private HexCellCoordinates _hexCellCoord;
    private int _size;
	private int _mapSize;
    private Vector3[] _chunkCellsPositions;

	private float[,] _heightMapValues;


	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;
	List<Vector2> uvs;

    public void Initialize(int x, int z, int size, int mapSize, float[,] heightMapValues)
    {
		_hexCellCoord = new HexCellCoordinates(x, z);
        _size = size;
		_mapSize = mapSize;
		_heightMapValues = heightMapValues;

		InitializeCellsPositions();
    }

	// private void InitializeCellsPositions(float[,] heightMapValues){
	// 	_chunkCellsPositions = new Vector3[_size * _size];
		
	// 	for (int cellZ = 0; cellZ < _size; cellZ++)
	// 	{
	// 		for (int cellX = 0; cellX < _size; cellX++)
	// 		{
	// 			int xSample = cellX + _x * _size;
	// 			int zSample = cellZ + _z * _size;
	// 			float y = heightMapValues[xSample, zSample];
	// 			// _chunkCellsPositions[cellZ * _size + cellX] = HexMetrics.CellGridToWorldPos(xSample, y, zSample);
	// 			_chunkCellsPositions[cellZ * _size + cellX] = HexMetrics.CellGridToWorldPos(cellX, y, cellZ);				
	// 		}
	// 	}
	// }

	// private void InitializeCellsPositions(float[,] heightMapValues){
	// 	_chunkCellsPositions = new Vector3[_size * _size];
		
	// 	for (int cellZ = 0; cellZ < _size; cellZ++)
	// 	{
	// 		for (int cellX = 0; cellX < _size; cellX++)
	// 		{
	// 			int xSample = cellX + _x * _size;
	// 			int zSample = cellZ + _z * _size;
	// 			float y = heightMapValues[xSample, zSample];
	// 			// _chunkCellsPositions[cellZ * _size + cellX] = HexMetrics.CellGridToWorldPos(xSample, y, zSample);
	// 			_chunkCellsPositions[cellZ * _size + cellX] = HexMetrics.CellGridToWorldPos(cellX, y, cellZ);				
	// 		}
	// 	}
	// }

	private void InitializeCellsPositions(){
		int radius = _size / 2;

		//Total cell number is can be calcultated using the radius
		//first the center cell + 6 cells around it
		//then 12(R=2 * 6) cells to make another ring around, then 18(R=3 * 6) ...
		// SUM(1->R : R) = R(R+1)/2
		int totalCells = 1 + 6 * (radius * (radius + 1) / 2);
		_chunkCellsPositions = new Vector3[totalCells];
		
		// Define axial directions
		HexCellCoordinates[] directions = { //TODO use HexCoordinates instead
			new HexCellCoordinates(0, -1), // bottom left
			new HexCellCoordinates(-1, 0), 	// left
			new HexCellCoordinates(-1, 1), 	// top left
			new HexCellCoordinates(0, 1), 	// top right
			new HexCellCoordinates(1, 0), 	// right
			new HexCellCoordinates(1, -1) 	// bottom right
		};

		int index = 0;
		
		// Add center cell
		// float height = _heightMapValues[radius, radius]; // the center of the heightMap
		_chunkCellsPositions[index++] = HexMetrics.HexToWorld(new HexCellCoordinates(radius, radius));
		// _chunkCellsPositions[index++] = new Vector3(radius, 0, radius);

		// Iterate through rings
		for (int r = 1; r <= radius; r++) {
			// Start at (r, 0) for the current ring
			// height = _heightMapValues[_radius+r, _radius];
			// Vector3 current = new Vector3(r, height, 0);
			HexCellCoordinates current = new HexCellCoordinates(radius + r, radius);

			for (int d = 0; d < 6; d++) {  // 6 directions
				for (int i = 0; i < r; i++) {  // Number of cells in the current direction
					// Debug.Log("_radius = " + _radius + " +(int)current.x = " +  +(int)current.x + " (int)current.z = " + (int)current.z);
					// Debug.Log("d= " + d + " r= " + r);
					// Debug.Log("current= " + current + " index = " + index);
					// height = _heightMapValues[_radius+(int)current.x, _radius+(int)current.z];
					// current.y = height;
					current += directions[d];
					_chunkCellsPositions[index++] = HexMetrics.HexToWorld(new HexCellCoordinates(current.X, current.Z));
					// _chunkCellsPositions[index++] = new Vector3(current.X, 0, current.Z);
					// Move to the next cell in the current direction
				}
			}
		}
	}


	public void GenerateMesh () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
		Triangulate();
	}

	public void ApplyMaterial (Material material) {
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material = material;

	}

	public void Triangulate () {
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		uvs.Clear();

		for (int i = 0; i < _chunkCellsPositions.Length; i++) {
			// int cellXGlobalIndex = i % _radius + _x * _radius;
			// int cellZGlobalIndex = i / _radius + _z * _radius;

			// float rightNeighborHeightOffset = 0;
			// float topRightNeighborHeightOffset = 0;
			// float topLeftNeighborHeightOffset = 0;

			// // right neighbor
			// if(cellXGlobalIndex != _mapSize-1) // if is not last right cell in map
			// {
			// 	//add the right side of the cell
			// 	rightNeighborHeightOffset = _heightMapValues[cellXGlobalIndex + 1, cellZGlobalIndex] * HexMetrics.heightMultiplier - _chunkCellsPositions[i].y;

				
			// }

			// if(cellZGlobalIndex != _mapSize-1){ // if is not last line in map
			// 	// EVEN ROW
			// 	if(cellZGlobalIndex % 2 == 0)
			// 	{
			// 		// TOP RIGHT
			// 		topRightNeighborHeightOffset = _heightMapValues[cellXGlobalIndex, cellZGlobalIndex + 1] * HexMetrics.heightMultiplier - _chunkCellsPositions[i].y;
			// 		// TOP LEFT
			// 		if(cellXGlobalIndex > 0) // if is not first left cell in line
			// 		{
			// 			topLeftNeighborHeightOffset = _heightMapValues[cellXGlobalIndex -1, cellZGlobalIndex+1] * HexMetrics.heightMultiplier - _chunkCellsPositions[i].y;
						
			// 		}
					
			// 	}else{ // ODD ROW
			// 		// TOP RIGHT
			// 		if(cellXGlobalIndex != _mapSize-1) // if is not last right cell in line
			// 		{
			// 			topRightNeighborHeightOffset = _heightMapValues[cellXGlobalIndex + 1, cellZGlobalIndex+1] * HexMetrics.heightMultiplier - _chunkCellsPositions[i].y;
			// 		}
			// 		// TOP LEFT
			// 		topLeftNeighborHeightOffset = _heightMapValues[cellXGlobalIndex, cellZGlobalIndex+1] * HexMetrics.heightMultiplier - _chunkCellsPositions[i].y;
			// 	}
			// }


			// TriangulateCell(_chunkCellsPositions[i], i, rightNeighborHeightOffset, topRightNeighborHeightOffset, topLeftNeighborHeightOffset, _radius);
			TriangulateCell(_chunkCellsPositions[i]);
		}

		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();
		hexMesh.uv = uvs.ToArray(); //TODO possible optimisation using an array
		gameObject.AddComponent<MeshCollider>();

	}

	// void TriangulateCell(Vector3 cellPos, int cellIndex, float rightNeighborHeightOffset, float topRightNeighborHeightOffset, float topLeftNeighborHeightOffset, int chunkSize) {
	void TriangulateCell(Vector3 cellPos){
		int vertexIndex = vertices.Count;

		Vector3[] corners = HexMetrics.corners;
		for (int i = 0; i < 6; i++) {
			vertices.Add(cellPos + corners[i]);
		}

		AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 5);
		AddTriangle(vertexIndex + 1, vertexIndex + 4, vertexIndex + 5);
		AddTriangle(vertexIndex + 1, vertexIndex + 2, vertexIndex + 4);
		AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex + 4);

		//each cell needs to compare the right, top right and top left cells to triangulate the side of the cell to the height of the neighbor cell


		// if(topLeftNeighborHeightOffset == 0){
		// 	//TODO optimize using only 1 vertex if two neighbor are lower for vertex [7,8] or/and [9,10]
		// 	vertices.Add(cellPos + corners[5] + new Vector3(0, topLeftNeighborHeightOffset, 0)); // vertex 6  // top left vertex that connect to the TOP LEFT neighbor cell
		// 	vertices.Add(cellPos + corners[0] + new Vector3(0, topLeftNeighborHeightOffset, 0)); // vertex 7 // top vertex that connect to the TOP LEFT neighbor cell

		// 	AddTriangle(vertexIndex + 5, vertexIndex + 6, vertexIndex + 7); 
		// 	AddTriangle(vertexIndex + 5, vertexIndex + 7, vertexIndex + 0);
		// }

		// if(topRightNeighborHeightOffset == 0){
		// 	vertices.Add(cellPos + corners[0] + new Vector3(0, topRightNeighborHeightOffset, 0)); // vertex 8 // top vertex that connect to the TOP RIGHT neighbor cell
		// 	vertices.Add(cellPos + corners[1] + new Vector3(0, topRightNeighborHeightOffset, 0)); // vertex 9 // top right vertex that connect to the TOP RIGHT neighbor cell

		// 	AddTriangle(vertexIndex + 0, vertexIndex + 8, vertexIndex + 9);
		// 	AddTriangle(vertexIndex + 0, vertexIndex + 9, vertexIndex + 1);
		// }

		// if(rightNeighborHeightOffset == 0){
		// 	vertices.Add(cellPos + corners[1] + new Vector3(0, rightNeighborHeightOffset, 0)); // vertex 10 // top right vertex that connect to the RIGHT neighbor cell
		// 	vertices.Add(cellPos + corners[2] + new Vector3(0, rightNeighborHeightOffset, 0)); // vertex 11 // bottom right vertex that connect to the RIGHT neighbor cell



		// 	AddTriangle(vertexIndex + 1, vertexIndex + 10, vertexIndex + 11);
		// 	AddTriangle(vertexIndex + 1, vertexIndex + 11, vertexIndex + 2);
		// }

		//TODO optimize using only 1 vertex if two neighbor are lower for vertex [7,8] or/and [9,10]
		// int vertexIndexOffset = 6;
		// if(topLeftNeighborHeightOffset != 0){
		// 	vertices.Add(cellPos + corners[5] + new Vector3(0, topLeftNeighborHeightOffset, 0)); // vertex 6  // top left vertex that connect to the TOP LEFT neighbor cell
		// 	vertices.Add(cellPos + corners[0] + new Vector3(0, topLeftNeighborHeightOffset, 0)); // vertex 7 // top vertex that connect to the TOP LEFT neighbor cell

		// 	AddTriangle(vertexIndex + 5, vertexIndex + vertexIndexOffset, vertexIndex + vertexIndexOffset + 1); 
		// 	AddTriangle(vertexIndex + 5, vertexIndex + vertexIndexOffset + 1, vertexIndex + 0);

		// 	vertexIndexOffset += 2;
		// }

		// if(topRightNeighborHeightOffset != 0){
		// 	vertices.Add(cellPos + corners[0] + new Vector3(0, topRightNeighborHeightOffset, 0)); // vertex 8 // top vertex that connect to the TOP RIGHT neighbor cell
		// 	vertices.Add(cellPos + corners[1] + new Vector3(0, topRightNeighborHeightOffset, 0)); // vertex 9 // top right vertex that connect to the TOP RIGHT neighbor cell

		// 	AddTriangle(vertexIndex + 0, vertexIndex + vertexIndexOffset, vertexIndex + vertexIndexOffset + 1);
		// 	AddTriangle(vertexIndex + 0, vertexIndex + vertexIndexOffset + 1, vertexIndex + 1);

		// 	vertexIndexOffset += 2;
		// }

		// if(rightNeighborHeightOffset != 0){
		// 	vertices.Add(cellPos + corners[1] + new Vector3(0, rightNeighborHeightOffset, 0)); // vertex 10 // top right vertex that connect to the RIGHT neighbor cell
		// 	vertices.Add(cellPos + corners[2] + new Vector3(0, rightNeighborHeightOffset, 0)); // vertex 11 // bottom right vertex that connect to the RIGHT neighbor cell

		// 	AddTriangle(vertexIndex + 1, vertexIndex + vertexIndexOffset, vertexIndex + vertexIndexOffset + 1);
		// 	AddTriangle(vertexIndex + 1, vertexIndex + vertexIndexOffset + 1, vertexIndex + 2);

		// 	vertexIndexOffset += 2;
		// }

		
		// Vector2[] uvCorners = HexMetrics.uvCorners;
		// for (int i = 0; i < vertexIndexOffset; i++) {
		// 	Vector2 uv = uvCorners[i] / chunkSize;
		// 	// Calculate the UV offset based on the cell position
		// 	int column = cellIndex % chunkSize;
		// 	int row = cellIndex / chunkSize;
		// 	float uvOffsetX = column * (1f / chunkSize);
		// 	float uvOffsetY = row * (1f / chunkSize);
		// 	// Apply the UV offset to the UV coordinates
		// 	uv.x += uvOffsetX;
		// 	uv.y += uvOffsetY;
		// 	uvs.Add(uv);
		// }

		// uv for whole the map
		// Vector2[] uvCorners = HexMetrics.uvCorners;
		// for (int i = 0; i < vertexIndexOffset; i++) {
		// 	Vector2 uv = uvCorners[i] / chunkSize;

		// 	// Calculate the UV offset based on the cell position
		// 	int column = (cellIndex % chunkSize) // cell column in chunk
		// 				 + chunkSize * _x; // add offset to get cell column in whole map
			
		// 	int row = (cellIndex / chunkSize) + // get the row in the chunk
		// 				 + chunkSize * _z; // add offset to get row in whole map

		// 	float uvOffsetX = column * (1f / HexMetrics.MapSize); //divide by mapSize to normalize
		// 	float uvOffsetY = row * (1f / HexMetrics.MapSize);

		// 	// Apply the UV offset to the UV coordinates
		// 	uv.x += uvOffsetX;
		// 	uv.y += uvOffsetY;
		// 	uvs.Add(uv);
		// }

	}

	void AddTriangle (int v1, int v2, int v3) {
		triangles.Add(v1);
		triangles.Add(v2);
		triangles.Add(v3 );
	}
}