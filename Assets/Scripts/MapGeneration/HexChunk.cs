using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexChunk : MonoBehaviour {

	private int _chunkX;
	private int _chunkZ; 
    private int _size;
	private HeightMapSettings _heightMapSettings; // temporary, then will be contained in Biome data

	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;
	List<Vector2> uvs;

    public void Initialize(int x, int z, int size, HeightMapSettings heightMapSettings)
    {
		_chunkX = x;
		_chunkZ = z;
        _size = size;

		_heightMapSettings = heightMapSettings;
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

		TriangulateCellTopFaces();
		TriangulateCellSides();

		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();
		hexMesh.uv = uvs.ToArray(); //TODO possible optimisation using an array
		gameObject.AddComponent<MeshCollider>();
	}

	void TriangulateCellTopFaces(){
		float[,] heightMap = NoiseGenerator.GenerateChunkHeightMap(_heightMapSettings);

		for (int cellIndex = 0; cellIndex < HexGridUtils.ChunkCellsPositions.Length; cellIndex++){
			int vertexIndex = vertices.Count;

			int xHeight = (int)HexGridUtils.ChunkCellsPositions[cellIndex].x;
			int zHeight = (int)HexGridUtils.ChunkCellsPositions[cellIndex].z;
			float height = heightMap[xHeight,zHeight];

			Vector3 cellPos = HexGridUtils.HexToWorld(HexGridUtils.ChunkCellsPositions[cellIndex] + new Vector3(0,height,0));

			Vector3[] corners = HexMetrics.corners;
			for (int i = 0; i < 6; i++) {
				vertices.Add(cellPos + corners[i]);
			}

			AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 5);
			AddTriangle(vertexIndex + 1, vertexIndex + 4, vertexIndex + 5);	
			AddTriangle(vertexIndex + 1, vertexIndex + 2, vertexIndex + 4);
			AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex + 4);

			Vector2[] uvCorners = HexMetrics.uvCorners;
			for (int i = 0; i < 6; i++) {
				Vector2 uv = uvCorners[i] / _size;
				// Calculate the UV offset based on the cell position
				int column = cellIndex % _size;
				int row = cellIndex / _size;
				float uvOffsetX = column * (1f / _size);
				float uvOffsetY = row * (1f / _size);
				// Apply the UV offset to the UV coordinates
				uv.x += uvOffsetX;
				uv.y += uvOffsetY;
				uvs.Add(uv);
			}
		}
	}

	void TriangulateCellSides(){
		// re-use the vertices from the top faces to connect the cells by creating the side faces
		// the current cell won't create the side between itself and the cell(s) in next direction
		// for the exterior cells, add new vertices at y=0
		int cellIndex = 1;
		int radius = _size / 2;
		for (int r = 1; r <= radius; r++) {
			for (int d = 0; d < 6; d++) {  // 6 directions
				for (int i = 0; i < r; i++) {  // Number of cells in the current direction
					int vertexIndex = cellIndex * 6; // index to the first vertex of the cell (there are 6 vertices per cells)
					if (r%2 == 1 || r == radius){
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
			Debug.Log("Should add vertex to connect toward the bottom " + "cellIndex : "+ cellIndex);
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

	void AddTriangle (int v1, int v2, int v3) {
		triangles.Add(v1);
		triangles.Add(v2);
		triangles.Add(v3 );
	}
}