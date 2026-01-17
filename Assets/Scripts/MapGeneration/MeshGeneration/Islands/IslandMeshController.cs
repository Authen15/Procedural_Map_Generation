using UnityEngine;

public class IslandMeshController
{
    private readonly MeshFilter _meshFilter;
    
    public IslandMeshController(MeshFilter meshFilter)
    {
        _meshFilter = meshFilter;
    }
    
    public void UpdateMesh(float[] heightMapFlat)
    {
        // copy template on first update
        if(_meshFilter.mesh.vertexCount == 0)
            _meshFilter.mesh = Object.Instantiate(IslandTemplateGenerator.TemplateMesh);
        
        Mesh mesh = _meshFilter.mesh;
        UpdateVerticesHeight(heightMapFlat);
        
        
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        _meshFilter.mesh = mesh;
    }
    
    private void UpdateVerticesHeight(float[] heightMapFlat)
    {

        Vector3[] _vertices = _meshFilter.mesh.vertices;
        
        int islandRadius = HexMetrics.IslandRadius;
        int mapWidth = HexMetrics.IslandSize;
        float heightMultiplier = HexMetrics.HeightMultiplier;
        
        for (int cellIndex = 0; cellIndex < HexGridUtils.IslandCellsPositions.Length; cellIndex++)
        {
            int vertexIndex = cellIndex * 6;
            
            var cellPos = HexGridUtils.IslandCellsPositions[cellIndex];
            int heightMapIndex = (cellPos.R + islandRadius) * mapWidth + (cellPos.S + islandRadius);
            float height = heightMapFlat[heightMapIndex] * heightMultiplier;
            
            _vertices[vertexIndex].y = height;
            _vertices[vertexIndex + 1].y = height;
            _vertices[vertexIndex + 2].y = height;
            _vertices[vertexIndex + 3].y = height;
            _vertices[vertexIndex + 4].y = height;
            _vertices[vertexIndex + 5].y = height;
        }
        
        _meshFilter.mesh.SetVertices(_vertices);
    }
}