using UnityEngine;
using System.Collections.Generic;
using Biome;
using Unity.VisualScripting;

struct HexIslandMeshData
{
	public Vector2[] uvs;
	public Vector3[] baseVertices; // vertices are the sae for each island except for their height
	public List<int> triangles;
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexIsland : MonoBehaviour {

	public int IslandX;
	public int IslandZ; 
    private int _size = HexMetrics.IslandSize;
	private int _radius;
	public BiomeData IslandBiome;

	private Mesh _hexMesh;
	private static HexIslandMeshData _hexIslandMeshData;
	[HideInInspector]
	private Vector3[] _vertices; // needed because vertices height is unique
	
			
    public void Initialize(int x, int z)
    {
		IslandX = x;
		IslandZ = z;
		_radius = _size / 2;

		IslandBiome = BiomeManager.Instance.GetBiome();
		IslandBiome.HeightMapSettings.UpdateIslandMesh += UpdateMeshRunTime; // Update the mesh at runtime when modifying heightmap parameters
    }

	public void GenerateMesh () {
		GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
		_hexMesh.name = "Hex Mesh";

		float[,] heightMap = NoiseGenerator.GenerateHeightMap(IslandBiome.HeightMapSettings, HexMetrics.IslandSize, IslandX + IslandZ * HexMetrics.MapSize);

		if (_hexIslandMeshData.triangles == null && _hexIslandMeshData.baseVertices == null){ // store triangles and vertices in a struct as we need them only once
			_hexIslandMeshData.triangles = new List<int>();
			_hexIslandMeshData.baseVertices = new Vector3[HexGridUtils.ChunkCellsPositions.Length * 6 + (_radius*6*6)]; // * 6 because there are 6 vertices per cell, _radius*6*6 to duplicate the last ring vertices for island edges triangles
			Triangulate(); // create vertices and triangles
		}
		_vertices = (Vector3[])_hexIslandMeshData.baseVertices.Clone(); // HexIslandMeshData vertices are shared for each meshes
		
		if (_hexIslandMeshData.uvs == null ){ // we can reuse UVs for each islands
			_hexIslandMeshData.uvs = new Vector2[HexGridUtils.ChunkCellsPositions.Length * 6 + (_radius*6*6)];
			GenerateUVs();
		}

		GenerateVerticesHeight(heightMap); // since we re-use vertices, we need to update the Height
		ApplyMaterial();

		_hexMesh.vertices = _vertices;
		_hexMesh.triangles = _hexIslandMeshData.triangles.ToArray();
		_hexMesh.uv = _hexIslandMeshData.uvs;
		_hexMesh.RecalculateNormals();
		_hexMesh.RecalculateTangents();
		// _hexMesh.RecalculateUVDistributionMetrics();
		gameObject.AddComponent<MeshCollider>();
	}

	public void ApplyMaterial () {
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = IslandBiome.BiomeMaterial;
		SetMaterialProperties(meshRenderer.sharedMaterial);
	}

	void SetMaterialProperties(Material material)
	{
		material.SetFloat("_MinHeight", 0);
		material.SetFloat("_MaxHeight", HexMetrics.HeightMultiplier);
		material.SetFloat("_IslandSize", HexMetrics.IslandSize);
	}


	#region Triangulation

	public void Triangulate () {
		TriangulateCellTopFaces();
		TriangulateCellSides();
	}

	void TriangulateCellTopFaces(){
		for (int cellIndex = 0; cellIndex < HexGridUtils.ChunkCellsPositions.Length; cellIndex++){
			int vertexIndex = cellIndex * 6; // 6 vertices per cell

			Vector3 cellWorldPos = HexGridUtils.HexToWorld(HexGridUtils.ChunkCellsPositions[cellIndex]);

			Vector3[] corners = HexMetrics.corners;
			for (int i = 0; i < 6; i++) {
				_hexIslandMeshData.baseVertices[vertexIndex + i] = cellWorldPos + corners[i];
			}

			AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 5);
			AddTriangle(vertexIndex + 1, vertexIndex + 4, vertexIndex + 5);	
			AddTriangle(vertexIndex + 1, vertexIndex + 2, vertexIndex + 4);
			AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex + 4);
		}
	}

	void TriangulateCellSides(){
		// re-use the vertices from the top faces to connect the cells by creating the side faces
		// the current cell won't create the side between itself and the cell(s) in next direction
		// for the exterior cells, add new vertices at y=0
		int cellIndex = 1;
		for (int r = 1; r <= _radius; r++) {
			for (int d = 0; d < 6; d++) {  // 6 directions
				for (int i = 0; i < r; i++) {  // Number of cells in the current direction
					int vertexIndex = cellIndex * 6; // index to the first vertex of the cell (there are 6 vertices per cells)
					if (r%2 == 1 || r == _radius){
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

	private void TriangulateBotLeftSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(4,3,1);
		Vector3Int secondTriIndices = new Vector3Int(4,1,0);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.BottomLeft);
	}

	private void TriangulateLeftSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(5,4,2);
		Vector3Int secondTriIndices = new Vector3Int(5,2,1);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.Left);
	}

	private void TriangulateTopLeftSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(0,5,3);
		Vector3Int secondTriIndices = new Vector3Int(0,3,2);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.TopLeft);
	}

	private void TriangulateTopRightSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(1,0,4);
		Vector3Int secondTriIndices = new Vector3Int(1,4,3);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.TopRight);
	}

	private void TriangulateRightSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(2,1,5);
		Vector3Int secondTriIndices = new Vector3Int(2,5,4);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.Right);
	}

	private void TriangulateBotRightSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(3,2,0);
		Vector3Int secondTriIndices = new Vector3Int(3,0,5);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.BottomRight);
	}

	private void TriangulateSide(int cellIndex, int vertexIndex, Vector3Int firstTriIndices, Vector3Int secondTriIndices, HexGridUtils.Dir dir){
		Vector3 sideCellPos = HexGridUtils.ChunkCellsPositions[cellIndex] + HexGridUtils.GetDir[(int)dir];
		int sideCellVertexIndex;
		// TODO optimize this to not create 6 vertices as only two or three are needed
		if(!HexGridUtils.CellIndexDict.ContainsKey(sideCellPos)){ // side cell does not exist
			sideCellVertexIndex = 	(HexGridUtils.GetRingStartIndex(_radius) + // start index after the last cells
			 						cellIndex - HexGridUtils.GetRingStartIndex(_radius - 1)) * 6; // index of the cell in the last ring, * 6 to get vertex index

			Vector3 cellWorldPos = HexGridUtils.HexToWorld(HexGridUtils.ChunkCellsPositions[cellIndex]);
			Vector3[] corners = HexMetrics.corners;
			for (int i = 0; i < 6; i++) {
				_hexIslandMeshData.baseVertices[sideCellVertexIndex + i] = cellWorldPos + corners[i] + new Vector3(0, -HexMetrics.HeightMultiplier * 10f, 0);
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

	private void AddTriangle (int v1, int v2, int v3) {
		_hexIslandMeshData.triangles.Add(v1);
		_hexIslandMeshData.triangles.Add(v2);
		_hexIslandMeshData.triangles.Add(v3);
	}

	#endregion

	public void UpdateMeshRunTime(){
		UpdateVerticesHeight();
		_hexMesh.RecalculateNormals();
		_hexMesh.RecalculateTangents();
	}

	private void UpdateVerticesHeight(){
		float[,] heightMap = NoiseGenerator.GenerateHeightMap(IslandBiome.HeightMapSettings, HexMetrics.IslandSize, IslandX + IslandZ * HexMetrics.MapSize);
		GenerateVerticesHeight(heightMap);
		ApplyMaterial();
		_hexMesh.vertices = _vertices;
	}
	
	public void GenerateVerticesHeight(float[,] heightMap){
		for (int cellIndex = 0; cellIndex < HexGridUtils.ChunkCellsPositions.Length; cellIndex++){
			int vertexIndex = cellIndex * 6; // 6 vertices per cell

			int xHeight = (int)HexGridUtils.ChunkCellsPositions[cellIndex].x + _radius; // + _radius to normalized because cells coord can be negative
			int yHeight = (int)HexGridUtils.ChunkCellsPositions[cellIndex].y + _radius; //TODO change coorddinates system to be able to use heightmap in shaders (right now the x axis is creating problem because of the shift)
																						// Maybe try doing a convertion from axial coord to offset https://www.redblobgames.com/grids/hexagons/
			float height = heightMap[xHeight,yHeight];

			
			for (int i = 0; i < 6; i++) { // for each vertex update the height
				_vertices[vertexIndex + i].y = height * HexMetrics.HeightMultiplier;
			}
		}
	}

	private void GenerateUVs(){
		Vector2[] uvCorners = HexMetrics.uvCorners;

		Vector2 maxCellPos = HexGridUtils.HexToUV(new Vector2(_size / 1.5f, _size / 1.5f));
		// float maxUVX = float.MinValue;
		// float maxUVY = float.MinValue;
		
		for (int cellIndex = 0; cellIndex < HexGridUtils.ChunkCellsPositions.Length; cellIndex++){
			int vertexIndex = cellIndex * 6;
			Vector2 cellPos = HexGridUtils.HexToUV(HexGridUtils.ChunkCellsPositions[cellIndex]);

			for (int i = 0; i < 6; i++) {
				Vector2 uv = uvCorners[i];
				uv /= maxCellPos.x;
				Vector2 cellPosNormalized = cellPos / maxCellPos.x;
				uv += cellPosNormalized;
				uv += new Vector2(0.5f, 0.5f);
				_hexIslandMeshData.uvs[vertexIndex + i] = uv;
				if(uv.x > 1f || uv.y > 1) Debug.Log("UV are incorrect > 1 " + uv);
				

				// if(uv.x > maxUVX) maxUVX = uv.x;
				// if(uv.y > maxUVY) maxUVY = uv.y;
			}

			// Debug.Log(
			// 	// "uv " + cellIndex + " " + cellGridPos + " " + (cellGridPos + posOffset) + " are \n"
			// 	"uv " + cellIndex + " are \n"
			// 	+ _hexIslandMeshData.uvs[vertexIndex].ToString(".0###########") + "\n"
			// 	+ _hexIslandMeshData.uvs[vertexIndex+1].ToString(".0###########") + "\n"
			// 	+ _hexIslandMeshData.uvs[vertexIndex+2].ToString(".0###########") + "\n"
			// 	+ _hexIslandMeshData.uvs[vertexIndex+3].ToString(".0###########") + "\n"
			// 	+ _hexIslandMeshData.uvs[vertexIndex+4].ToString(".0###########") + "\n"
			// 	+ _hexIslandMeshData.uvs[vertexIndex+5].ToString(".0###########")
			// );
		}
		// Debug.Log("maxUVX " + maxUVX + " maxUVY" + maxUVY);
	}
}