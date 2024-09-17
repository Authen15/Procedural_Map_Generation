using UnityEngine;
using System.Collections.Generic;

public static class HexIslandMeshGenerator {
	private static Vector2[] _uvs;
	private static Vector3[] _baseVertices; // vertices are the same for each island except for their height
	private static List<int> _triangles;

	public static void GenerateMesh(Island island) {
		if (_triangles == null && _baseVertices == null){ // store triangles and vertices in a struct as we need them only once
			_triangles = new List<int>();
			_baseVertices = new Vector3[HexGridUtils.IslandCellsPositions.Length * 6 + (HexMetrics.IslandRadius*6*6)]; // * 6 because there are 6 vertices per cell, HexMetrics.IslandRadius*6*6 to duplicate the last ring vertices for island edges triangles
			Triangulate(); // create vertices and triangles
		}
		
		if (_uvs == null ){ // we can reuse UVs for each islands
			_uvs = new Vector2[HexGridUtils.IslandCellsPositions.Length * 6 + (HexMetrics.IslandRadius*6*6)];
			GenerateUVs();
		}
		
		island.Mesh.vertices = (Vector3[])_baseVertices.Clone();
		island.Mesh.triangles = _triangles.ToArray();
		island.Mesh.uv = _uvs;

		UpdateMesh(island);
		ApplyMaterial(island.MeshRenderer, island.Biome.BiomeMaterial);

		// mesh.RecalculateUVDistributionMetrics();
		// gameObject.AddComponent<MeshCollider>();
	}

	public static void UpdateMesh(Island island){
		float[,] heightMap = NoiseGenerator.GenerateHeightMap(island.Biome.HeightMapSettings, HexMetrics.IslandSize, island.IslandX + island.IslandZ * HexMetrics.MapSize);
		Vector3[] vertices = island.Mesh.vertices;

		UpdateVerticesHeight(vertices, heightMap);

		island.Mesh.vertices = vertices;
		island.Mesh.RecalculateNormals();
		island.Mesh.RecalculateTangents();
	}

    private static void ApplyMaterial(MeshRenderer meshRenderer, Material material) {
        meshRenderer.sharedMaterial = material;
        SetMaterialProperties(meshRenderer.sharedMaterial);
	}

	private static void SetMaterialProperties(Material material)
	{
		material.SetFloat("_MinHeight", 0);
		material.SetFloat("_MaxHeight", HexMetrics.HeightMultiplier);
	}
	

#region Triangulation

	private static void Triangulate () {
		TriangulateCellTopFaces();
		TriangulateCellSides();
	}

	private static void TriangulateCellTopFaces(){
		for (int cellIndex = 0; cellIndex < HexGridUtils.IslandCellsPositions.Length; cellIndex++){
			int vertexIndex = cellIndex * 6; // 6 vertices per cell

			Vector3 cellWorldPos = HexGridUtils.CellToWorld(HexGridUtils.IslandCellsPositions[cellIndex]);

			Vector3[] corners = HexMetrics.corners;
			for (int i = 0; i < 6; i++) {
				_baseVertices[vertexIndex + i] = cellWorldPos + corners[i];
			}

			AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 5);
			AddTriangle(vertexIndex + 1, vertexIndex + 4, vertexIndex + 5);	
			AddTriangle(vertexIndex + 1, vertexIndex + 2, vertexIndex + 4);
			AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex + 4);
		}
	}

	private static void TriangulateCellSides(){
		// re-use the vertices from the top faces to connect the cells by creating the side faces
		// the current cell won't create the side between itself and the cell(s) in next direction
		// for the exterior cells, add new vertices at y=0
		int cellIndex = 1;
		for (int r = 1; r <= HexMetrics.IslandRadius; r++) {
			for (int d = 0; d < 6; d++) {  // 6 directions
				for (int i = 0; i < r; i++) {  // Number of cells in the current direction
					int vertexIndex = cellIndex * 6; // index to the first vertex of the cell (there are 6 vertices per cells)
					if (r%2 == 1 || r == HexMetrics.IslandRadius){
						switch(d){
							case (int)HexGridUtils.Dir.BottomLeft :
								// TriangulateBotLeftSide(cellIndex, vertexIndex);
								TriangulateLeftSide(cellIndex, vertexIndex);
								TriangulateTopLeftSide(cellIndex, vertexIndex);
								TriangulateTopRightSide(cellIndex, vertexIndex);
								TriangulateRightSide(cellIndex, vertexIndex);
								TriangulateBotRightSide(cellIndex, vertexIndex);
								break;
							case (int)HexGridUtils.Dir.Left :
								TriangulateBotLeftSide(cellIndex, vertexIndex);
								// TriangulateLeftSide(cellIndex, vertexIndex);
								TriangulateTopLeftSide(cellIndex, vertexIndex);
								TriangulateTopRightSide(cellIndex, vertexIndex);
								TriangulateRightSide(cellIndex, vertexIndex);
								TriangulateBotRightSide(cellIndex, vertexIndex);
								break;
							case (int)HexGridUtils.Dir.TopLeft :
								TriangulateBotLeftSide(cellIndex, vertexIndex);
								TriangulateLeftSide(cellIndex, vertexIndex);
								// TriangulateTopLeftSide(cellIndex, vertexIndex);
								TriangulateTopRightSide(cellIndex, vertexIndex);
								TriangulateRightSide(cellIndex, vertexIndex);
								TriangulateBotRightSide(cellIndex, vertexIndex);
								break;
							case (int)HexGridUtils.Dir.TopRight :
								TriangulateBotLeftSide(cellIndex, vertexIndex);
								TriangulateLeftSide(cellIndex, vertexIndex);
								TriangulateTopLeftSide(cellIndex, vertexIndex);
								// TriangulateTopRightSide(cellIndex, vertexIndex);
								TriangulateRightSide(cellIndex, vertexIndex);
								TriangulateBotRightSide(cellIndex, vertexIndex);
								break;
							case (int)HexGridUtils.Dir.Right :
								TriangulateBotLeftSide(cellIndex, vertexIndex);
								TriangulateLeftSide(cellIndex, vertexIndex);
								TriangulateTopLeftSide(cellIndex, vertexIndex);
								TriangulateTopRightSide(cellIndex, vertexIndex);
								// TriangulateRightSide(cellIndex, vertexIndex);
								TriangulateBotRightSide(cellIndex, vertexIndex);
								break;
							case (int)HexGridUtils.Dir.BottomRight :
								TriangulateBotLeftSide(cellIndex, vertexIndex);
								TriangulateLeftSide(cellIndex, vertexIndex);
								TriangulateTopLeftSide(cellIndex, vertexIndex);
								TriangulateTopRightSide(cellIndex, vertexIndex);
								TriangulateRightSide(cellIndex, vertexIndex);
								// TriangulateBotRightSide(cellIndex, vertexIndex);
								break;
						}
					}else{
						switch(d){
							case (int)HexGridUtils.Dir.BottomLeft :
								TriangulateBotLeftSide(cellIndex, vertexIndex);
								break;
							case (int)HexGridUtils.Dir.Left :
								TriangulateLeftSide(cellIndex, vertexIndex);
								break;
							case (int)HexGridUtils.Dir.TopLeft :
								TriangulateTopLeftSide(cellIndex, vertexIndex);
								break;
							case (int)HexGridUtils.Dir.TopRight :
								TriangulateTopRightSide(cellIndex, vertexIndex);
								break;
							case (int)HexGridUtils.Dir.Right :
								TriangulateRightSide(cellIndex, vertexIndex);
								break;
							case (int)HexGridUtils.Dir.BottomRight :
								TriangulateBotRightSide(cellIndex, vertexIndex);
								break;
						}
					}
					cellIndex ++;
				}
			}
		}
	}	

	private static void TriangulateBotLeftSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(4,3,1);
		Vector3Int secondTriIndices = new Vector3Int(4,1,0);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.BottomLeft);
	}

	private static void TriangulateLeftSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(5,4,2);
		Vector3Int secondTriIndices = new Vector3Int(5,2,1);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.Left);
	}

	private static void TriangulateTopLeftSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(0,5,3);
		Vector3Int secondTriIndices = new Vector3Int(0,3,2);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.TopLeft);
	}

	private static void TriangulateTopRightSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(1,0,4);
		Vector3Int secondTriIndices = new Vector3Int(1,4,3);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.TopRight);
	}

	private static void TriangulateRightSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(2,1,5);
		Vector3Int secondTriIndices = new Vector3Int(2,5,4);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.Right);
	}

	private static void TriangulateBotRightSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(3,2,0);
		Vector3Int secondTriIndices = new Vector3Int(3,0,5);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.BottomRight);
	}

	private static void TriangulateSide(int cellIndex, int vertexIndex, Vector3Int firstTriIndices, Vector3Int secondTriIndices, HexGridUtils.Dir dir){
		Vector3 sideCellPos = HexGridUtils.IslandCellsPositions[cellIndex] + HexGridUtils.GetDir[(int)dir];
		int sideCellVertexIndex;
		// TODO optimize this to not create 6 vertices as only two or three are needed
		if(!HexGridUtils.CellIndexDict.ContainsKey(sideCellPos)){ // side cell does not exist
			sideCellVertexIndex = 	(HexGridUtils.GetIslandCellsNumber(HexMetrics.IslandRadius) + // start index after the last cells
			 						cellIndex - HexGridUtils.GetIslandCellsNumber(HexMetrics.IslandRadius - 1)) * 6; // index of the cell in the last ring, * 6 to get vertex index

			Vector3 cellWorldPos = HexGridUtils.CellToWorld(HexGridUtils.IslandCellsPositions[cellIndex]);
			Vector3[] corners = HexMetrics.corners;
			for (int i = 0; i < 6; i++) {
				_baseVertices[sideCellVertexIndex + i] = cellWorldPos + corners[i] + new Vector3(0, -HexMetrics.HeightMultiplier * 10f, 0);
			}

			switch(dir){
				case HexGridUtils.Dir.BottomLeft :
					firstTriIndices = new Vector3Int(4,3,3);
					secondTriIndices = new Vector3Int(4,3,4);
					break;
				case HexGridUtils.Dir.Left :
					firstTriIndices = new Vector3Int(5,4,4);
					secondTriIndices = new Vector3Int(5,4,5);
					break;
				case HexGridUtils.Dir.TopLeft :
					firstTriIndices = new Vector3Int(0,5,5);
					secondTriIndices = new Vector3Int(0,5,0);
					break;
				case HexGridUtils.Dir.TopRight :
					firstTriIndices = new Vector3Int(1,0,0);
					secondTriIndices = new Vector3Int(1,0,1);
					break;
				case HexGridUtils.Dir.Right :
					firstTriIndices = new Vector3Int(2,1,1);
					secondTriIndices = new Vector3Int(2,1,2);
					break;
				case HexGridUtils.Dir.BottomRight :
					firstTriIndices = new Vector3Int(3,2,2);
					secondTriIndices = new Vector3Int(3,2,3);
					break;
			}
		}else{
			int sideCellIndex = HexGridUtils.CellIndexDict[sideCellPos];
			sideCellVertexIndex = sideCellIndex*6;
		}

		

		AddTriangle(
			vertexIndex + firstTriIndices.x, 
			vertexIndex + firstTriIndices.y, 
			sideCellVertexIndex + firstTriIndices.z);
			
		AddTriangle(
			vertexIndex + secondTriIndices.x, 
			sideCellVertexIndex + secondTriIndices.y, 
			sideCellVertexIndex + secondTriIndices.z);
	}

	private static void AddTriangle (int v1, int v2, int v3) {
		_triangles.Add(v1);
		_triangles.Add(v2);
		_triangles.Add(v3);
	}

#endregion


	private static void UpdateVerticesHeight(Vector3[] vertices, float[,] heightMap){
		for (int cellIndex = 0; cellIndex < HexGridUtils.IslandCellsPositions.Length; cellIndex++){
			int vertexIndex = cellIndex * 6; // 6 vertices per cell

			int xHeight = (int)HexGridUtils.IslandCellsPositions[cellIndex].x + HexMetrics.IslandRadius; // + _radius to normalized because cells coord can be negative
			int yHeight = (int)HexGridUtils.IslandCellsPositions[cellIndex].y + HexMetrics.IslandRadius; //TODO change coorddinates system to be able to use heightmap in shaders (right now the x axis is creating problem because of the shift)
																						// Maybe try doing a convertion from axial coord to offset https://www.redblobgames.com/grids/hexagons/
			float height = heightMap[xHeight,yHeight];

			
			for (int i = 0; i < 6; i++) { // for each vertex update the height
				vertices[vertexIndex + i].y = height * HexMetrics.HeightMultiplier;
			}
		}
	}

	private static void GenerateUVs(){
		Vector2[] uvCorners = HexMetrics.uvCorners;

		Vector2 maxCellPos = HexGridUtils.CellToUV(new Vector2(HexMetrics.IslandSize / 1.5f, HexMetrics.IslandSize / 1.5f));
		// float maxUVX = float.MinValue;
		// float maxUVY = float.MinValue;
		
		for (int cellIndex = 0; cellIndex < HexGridUtils.IslandCellsPositions.Length; cellIndex++){
			int vertexIndex = cellIndex * 6;
			Vector2 cellPos = HexGridUtils.CellToUV(HexGridUtils.IslandCellsPositions[cellIndex]);

			for (int i = 0; i < 6; i++) {
				Vector2 uv = uvCorners[i];
				uv /= maxCellPos.x;
				Vector2 cellPosNormalized = cellPos / maxCellPos.x;
				uv += cellPosNormalized;
				uv += new Vector2(0.5f, 0.5f);
				_uvs[vertexIndex + i] = uv;
				if(uv.x > 1f || uv.y > 1) Debug.Log("UV are incorrect > 1 " + uv);
				

				// if(uv.x > maxUVX) maxUVX = uv.x;
				// if(uv.y > maxUVY) maxUVY = uv.y;
			}

			// Debug.Log(
			// 	// "uv " + cellIndex + " " + cellGridPos + " " + (cellGridPos + posOffset) + " are \n"
			// 	"uv " + cellIndex + " are \n"
			// 	+ _hex_uvs[vertexIndex].ToString(".0###########") + "\n"
			// 	+ _hex_uvs[vertexIndex+1].ToString(".0###########") + "\n"
			// 	+ _hex_uvs[vertexIndex+2].ToString(".0###########") + "\n"
			// 	+ _hex_uvs[vertexIndex+3].ToString(".0###########") + "\n"
			// 	+ _hex_uvs[vertexIndex+4].ToString(".0###########") + "\n"
			// 	+ _hex_uvs[vertexIndex+5].ToString(".0###########")
			// );
		}
		// Debug.Log("maxUVX " + maxUVX + " maxUVY" + maxUVY);
	}
}