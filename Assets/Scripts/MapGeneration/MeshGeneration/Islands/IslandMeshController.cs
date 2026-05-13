using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static IslandData;

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
        if (_meshFilter.mesh.vertexCount == 0)
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

        for (int cellIndex = 0; cellIndex < CellCoords.Length; cellIndex++)
        {
            int vertexIndex = cellIndex * 12;

            var cellPos = CellCoords[cellIndex];
            int heightMapIndex = (cellPos.R + islandRadius) * mapWidth + (cellPos.Q + islandRadius);
            float height = heightMapFlat[heightMapIndex] * heightMultiplier;

            _vertices[vertexIndex].y = height;
            _vertices[vertexIndex + 1].y = height;
            _vertices[vertexIndex + 2].y = height;
            _vertices[vertexIndex + 3].y = height;
            _vertices[vertexIndex + 4].y = height;
            _vertices[vertexIndex + 5].y = height;
        }

        _meshFilter.mesh.SetVertices(_vertices);

        // // the issue is that the buffer expect a indices based array, but the heightmap is based on coordinates
        _vertexHeightBuffer.SetData(_vertices.Select(v => v.y).ToArray());
    }

    private ComputeBuffer _vertexHeightBuffer;
    public void SetMpbBuffer(MaterialPropertyBlock mpb, MeshRenderer renderer)
    {
        _vertexHeightBuffer = new ComputeBuffer(
           HexMetrics.NbCellInIsland * 12, // 12 vertices per cell
           sizeof(float)
        );

        renderer.GetPropertyBlock(mpb);
        mpb.SetBuffer("_VertexHeightBuffer", _vertexHeightBuffer);
        renderer.SetPropertyBlock(mpb);
    }

    public void ClearBuffers()
    {
        if (_vertexHeightBuffer != null)
        {
            _vertexHeightBuffer.Release();
            _vertexHeightBuffer = null;
        }
    }
}