using UnityEngine;
using System.Collections.Generic;
using Biome;

struct HexIslandMeshData
{
	
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexIsland : MonoBehaviour {

	public int IslandX;
	public int IslandZ; 
    private int _size = HexMetrics.IslandSize;
	private int _radius;
	public BiomeData IslandBiome;

	Mesh hexMesh;
	List<Vector3> vertices; // Todo Reuse vertices and triangles as all islands are the same, just modify vertices height
	List<int> triangles;
	static Vector2[] uvs;

    public void Initialize(int x, int z)
    {
		IslandX = x;
		IslandZ = z;
		_radius = _size / 2;

		IslandBiome = BiomeManager.Instance.GetBiome();
    }

	public void GenerateMesh () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		triangles = new List<int>();
		Triangulate();
		ApplyMaterial();
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

	public void Triangulate () {
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();

		TriangulateCellTopFaces();
		TriangulateCellSides();
		GenerateUVs();

		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();
		hexMesh.uv = uvs;
		gameObject.AddComponent<MeshCollider>();
	}

	void TriangulateCellTopFaces(){
		float[,] heightMap = NoiseGenerator.GenerateIslandHeightMap(this);
		for (int cellIndex = 0; cellIndex < HexGridUtils.ChunkCellsPositions.Length; cellIndex++){
			int vertexIndex = vertices.Count;

			int xHeight = (int)HexGridUtils.ChunkCellsPositions[cellIndex].x + _radius; // + _radius to normalized because cells coord can be negative
			int yHeight = (int)HexGridUtils.ChunkCellsPositions[cellIndex].y + _radius;
			float height = heightMap[xHeight,yHeight];

			Vector3 cellWorldPos = HexGridUtils.HexToWorld(HexGridUtils.ChunkCellsPositions[cellIndex], height);

			Vector3[] corners = HexMetrics.corners;
			for (int i = 0; i < 6; i++) {
				vertices.Add(cellWorldPos + corners[i]);
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
		if(!HexGridUtils.CellIndexDict.ContainsKey(sideCellPos)){
			Debug.LogWarning("Should add vertex to connect toward the bottom " + "cellIndex : "+ cellIndex);
			// sideCellVertexIndex = vertices.Count; // we will add new vertices, so the offset is the actual number of vertices
			// Vector3 firstVertex = vertices[vertexIndex+firstTriIndices.x];
			// firstVertex.y = - HexMetrics.HeightMultiplier;
			// firstTriIndices.z = 0;
			// vertices.Add(firstVertex);

			// Vector3 secondVertex = vertices[vertexIndex+firstTriIndices.y];
			// secondVertex.y = - HexMetrics.HeightMultiplier;
			// secondTriIndices.y = 1;
			// vertices.Add(secondVertex);

			// Vector3 thirdVertex = vertices[vertexIndex+firstTriIndices.z];
			// thirdVertex.y = - HexMetrics.HeightMultiplier;
			// secondTriIndices.z = 2;
			// vertices.Add(thirdVertex);
			return; //TODO manage the uvs (maybe it would be better to add the cells in the TriangulateTopFaces, as an other crown but only for interior vertices )
		}else{
			// Debug.Log("everyting is fine " + "cellIndex : "+ cellIndex);
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
		triangles.Add(v1);
		triangles.Add(v2);
		triangles.Add(v3 );
	}

	private void GenerateUVs(){
		if (uvs == null ){ // we can reuse UVs for each islands
			uvs = new Vector2[HexGridUtils.ChunkCellsPositions.Length * 6]; // * 6 because there are 6 vertices per cell
		}else{
			return;
		}

		Vector2[] uvCorners = HexMetrics.uvCorners;

		float spacing = 0.1f;
		Vector2 maxCellWorldPos = HexGridUtils.HexToWorld(new Vector2(_size, _size));
		maxCellWorldPos += new Vector2(_size * (1+spacing) - spacing, _size  * (1+spacing) - spacing); // also take in count the spaces added between cells in he uvs
		
		for (int cellIndex = 0; cellIndex < HexGridUtils.ChunkCellsPositions.Length; cellIndex++){
			Vector2 cellWorldPos = HexGridUtils.HexToUV(HexGridUtils.ChunkCellsPositions[cellIndex], spacing);

			for (int i = 0; i < 6; i++) {
				Vector2 uv = uvCorners[i];
				uv /= maxCellWorldPos.x; // x is the max value between both coordinates
				Vector2 cellPosNormalized = cellWorldPos / maxCellWorldPos.x;
				uv += cellPosNormalized;
				uv += new Vector2(0.5f, 0.5f);
				uvs[cellIndex * 6 + i] = uv;
				if(uv.x > 1f || uv.y > 1) Debug.Log("UV are incorrect > 1 " + uv);
			}
		}

		// Debug.Log(
			// 		// "uv " + cellIndex + " " + cellGridPos + " " + (cellGridPos + posOffset) + " are \n"
			// 		"uv " + cellIndex + " are \n"
			// 		+ uvs[vertexIndex].ToString(".0###########") + "\n"
			// 		+ uvs[vertexIndex+1].ToString(".0###########") + "\n"
			// 		+ uvs[vertexIndex+2].ToString(".0###########") + "\n"
			// 		+ uvs[vertexIndex+3].ToString(".0###########") + "\n"
			// 		+ uvs[vertexIndex+4].ToString(".0###########") + "\n"
			// 		+ uvs[vertexIndex+5].ToString(".0###########")
			// );
	}
}