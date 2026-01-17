using System.Collections.Generic;
using UnityEngine;

public static class BridgeTemplateGenerator
{
	public static GameObject BridgeTemplate;

	static BridgeTemplateGenerator()
	{
		CreateTemplate();
	}

	public static void CreateTemplate()
	{
		if (BridgeTemplate != null)
		{
			Debug.LogWarning("Bridge Template already created");
			return;
		}

		BridgeTemplate = new GameObject("Bridge Template");
		BridgeTemplate.SetActive(false);

		List<Vector3> bridgeVertices = new List<Vector3>();
		List<int> bridgeTriangles = new List<int>();
		List<Vector2> bridgeUVs = new List<Vector2>();

		HexCellTriangulator.Triangulate(BridgeUtils.BridgeCellsIndexDict, bridgeVertices, bridgeTriangles, HexMetrics.HeightMultiplier / 4f);
		GenerateBridgeUVs(bridgeVertices, bridgeUVs);

		Mesh mesh = new Mesh();
		mesh.vertices = bridgeVertices.ToArray();
		mesh.triangles = bridgeTriangles.ToArray();
		mesh.uv = bridgeUVs.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();

		BridgeTemplate.AddComponent<MeshFilter>().mesh = mesh;
	}

	static void GenerateBridgeUVs(List<Vector3> bridgeVertices, List<Vector2> bridgeUVs)
	{
		for (int i = 0; i < bridgeVertices.Count; i++)
		{
			bridgeUVs.Add(HexMetrics.uvCorners[i % 6]); // each cell has 6 vertices
		}
	}
}