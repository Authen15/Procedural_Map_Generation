using UnityEngine;
using System.Collections.Generic;
using System.IO;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;
	List<Vector2> uvs;

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
	}

	public void ApplyMaterial (Material material, Material grassMaterial, Texture2D grassVisibilityTexture) {
		// Material grassMat = new Material(grassMaterial);
		// grassMat.SetTexture("_GrassMap", grassVisibilityTexture);

		// Material[] materials = new Material[2];
		// materials[0] = grassMat;
		// materials[1] = material;

		// MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

		// meshRenderer.materials = materials;
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material = material;

	}

	public void Triangulate (Vector3[] cellsPositions, int chunkSize, int chunkXIndex, int chunkZIndex, int mapSize, float[,] heightmap) {
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		uvs.Clear();

		for (int i = 0; i < cellsPositions.Length; i++) {

			int cellXGlobalIndex = i % chunkSize + chunkXIndex * chunkSize;
			int cellZGlobalIndex = i / chunkSize + chunkZIndex * chunkSize;

			float rightNeighborHeightOffset = 0;
			float topRightNeighborHeightOffset = 0;
			float topLeftNeighborHeightOffset = 0;

			// right neighbor
			if(cellXGlobalIndex != mapSize-1) // if is not last right cell in map
			{
				//add the right side of the cell
				rightNeighborHeightOffset = heightmap[cellXGlobalIndex + 1, cellZGlobalIndex] * HexMetrics.heightMultiplier - cellsPositions[i].y;

				
			}

			if(cellZGlobalIndex != mapSize-1){ // if is not last line in map
				// EVEN ROW
				if(cellZGlobalIndex % 2 == 0)
				{
					// TOP RIGHT
					topRightNeighborHeightOffset = heightmap[cellXGlobalIndex, cellZGlobalIndex + 1] * HexMetrics.heightMultiplier - cellsPositions[i].y;
					// TOP LEFT
					if(cellXGlobalIndex > 0) // if is not first left cell in line
					{
						topLeftNeighborHeightOffset = heightmap[cellXGlobalIndex -1, cellZGlobalIndex+1] * HexMetrics.heightMultiplier - cellsPositions[i].y;
						
					}
					
				}else{ // ODD ROW
					// TOP RIGHT
					if(cellXGlobalIndex != mapSize-1) // if is not last right cell in line
					{
						topRightNeighborHeightOffset = heightmap[cellXGlobalIndex + 1, cellZGlobalIndex+1] * HexMetrics.heightMultiplier - cellsPositions[i].y;
					}
					// TOP LEFT
					topLeftNeighborHeightOffset = heightmap[cellXGlobalIndex, cellZGlobalIndex+1] * HexMetrics.heightMultiplier - cellsPositions[i].y;
				}
			}


			TriangulateCell(cellsPositions[i], i, rightNeighborHeightOffset, topRightNeighborHeightOffset, topLeftNeighborHeightOffset, chunkSize);
		}

		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();
		hexMesh.uv = uvs.ToArray();
		gameObject.AddComponent<MeshCollider>();

	}

	void TriangulateCell(Vector3 cellPos, int cellIndex, float rightNeighborHeightOffset, float topRightNeighborHeightOffset, float topLeftNeighborHeightOffset, int chunkSize) {

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
		int vertexIndexOffset = 6;
		if(topLeftNeighborHeightOffset != 0){
			vertices.Add(cellPos + corners[5] + new Vector3(0, topLeftNeighborHeightOffset, 0)); // vertex 6  // top left vertex that connect to the TOP LEFT neighbor cell
			vertices.Add(cellPos + corners[0] + new Vector3(0, topLeftNeighborHeightOffset, 0)); // vertex 7 // top vertex that connect to the TOP LEFT neighbor cell

			AddTriangle(vertexIndex + 5, vertexIndex + vertexIndexOffset, vertexIndex + vertexIndexOffset + 1); 
			AddTriangle(vertexIndex + 5, vertexIndex + vertexIndexOffset + 1, vertexIndex + 0);

			vertexIndexOffset += 2;
		}

		if(topRightNeighborHeightOffset != 0){
			vertices.Add(cellPos + corners[0] + new Vector3(0, topRightNeighborHeightOffset, 0)); // vertex 8 // top vertex that connect to the TOP RIGHT neighbor cell
			vertices.Add(cellPos + corners[1] + new Vector3(0, topRightNeighborHeightOffset, 0)); // vertex 9 // top right vertex that connect to the TOP RIGHT neighbor cell

			AddTriangle(vertexIndex + 0, vertexIndex + vertexIndexOffset, vertexIndex + vertexIndexOffset + 1);
			AddTriangle(vertexIndex + 0, vertexIndex + vertexIndexOffset + 1, vertexIndex + 1);

			vertexIndexOffset += 2;
		}

		if(rightNeighborHeightOffset != 0){
			vertices.Add(cellPos + corners[1] + new Vector3(0, rightNeighborHeightOffset, 0)); // vertex 10 // top right vertex that connect to the RIGHT neighbor cell
			vertices.Add(cellPos + corners[2] + new Vector3(0, rightNeighborHeightOffset, 0)); // vertex 11 // bottom right vertex that connect to the RIGHT neighbor cell

			AddTriangle(vertexIndex + 1, vertexIndex + vertexIndexOffset, vertexIndex + vertexIndexOffset + 1);
			AddTriangle(vertexIndex + 1, vertexIndex + vertexIndexOffset + 1, vertexIndex + 2);

			vertexIndexOffset += 2;
		}

		
		Vector2[] uvCorners = HexMetrics.uvCorners;
		for (int i = 0; i < vertexIndexOffset; i++) {
			Vector2 uv = uvCorners[i] / chunkSize;
			// Calculate the UV offset based on the cell position
			int column = cellIndex % chunkSize;
			int row = cellIndex / chunkSize;
			float uvOffsetX = column * (1f / chunkSize);
			float uvOffsetY = row * (1f / chunkSize);
			// Apply the UV offset to the UV coordinates
			uv.x += uvOffsetX;
			uv.y += uvOffsetY;
			uvs.Add(uv);
		}

	}

	void AddTriangle (int v1, int v2, int v3) {
		triangles.Add(v1);
		triangles.Add(v2);
		triangles.Add(v3 );
	}
}