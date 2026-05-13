using UnityEngine;
using static HexGridUtils;
using static BridgeData;

public class BridgeController
{
	private readonly GameObject[] _bridges;
	private readonly MeshCollider[] _meshColliders;

	public BridgeController()
	{
		_bridges = new GameObject[3];
		_meshColliders = new MeshCollider[3];
	}

	public void UpdateBridges(Island currentIsland)
	{
		for (int i = 0; i < 3; i++)
		{
			Island targetIsland = BridgeUtils.GetNeighbourIsland(currentIsland.Index, (BridgeUtils.BridgesDir)i);
			if (targetIsland == null)
			{   // if there is no island in this direction, skip this iteration
				if (_bridges[i] != null) _bridges[i].SetActive(false);
				continue;
			}

			AxialCoordinates bridgeAnchorStartCell = BridgesAnchorCellCoord[i, BridgeUtils.BridgeWidth / 2];
			Vector3 anchorWorldPos = CellToWorld(bridgeAnchorStartCell);
			Vector3 position = currentIsland.transform.position + anchorWorldPos;
			Quaternion rotation = Quaternion.LookRotation(anchorWorldPos, Vector3.up);
			if (_bridges[i] == null)
			{
				_bridges[i] = Object.Instantiate(BridgeTemplateGenerator.BridgeTemplate, position, rotation, currentIsland.transform);
				_meshColliders[i] = _bridges[i].AddComponent<MeshCollider>();
				MeshRenderer meshRenderer = _bridges[i].AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = currentIsland.BridgeMaterial;
			}

			_bridges[i].transform.position = position;
			_bridges[i].transform.rotation = rotation;


			_bridges[i].SetActive(true);
			_bridges[i].name = $"Bridge {currentIsland.Index} to {targetIsland.Index}";

			Mesh mesh = _bridges[i].GetComponent<MeshFilter>().mesh;
			mesh.vertices = UpdateBridgeVerticesHeight(currentIsland, targetIsland, mesh.vertices, (BridgeUtils.BridgesDir)i);

			// Update collider
			_meshColliders[i].sharedMesh = null;
			_meshColliders[i].sharedMesh = mesh;
		}
	}

	private static Vector3[] UpdateBridgeVerticesHeight(Island currentIsland, Island targetIsland, Vector3[] vertices, BridgeUtils.BridgesDir dir)
	{

		int bridgeSize = BridgeUtils.BridgeWidth;
		float heightMultiplier = HexMetrics.HeightMultiplier;
		int distanceBetweenIslands = HexMetrics.DistanceBetweenIslands;
		float fenceHeight = heightMultiplier * 0.125f;
		int dirInt = (int)dir;
		int oppositeDirInt = (dirInt + 3) % 6;

		float averageStartHeight = 0;
		float averageEndHeight = 0;

		for (int i = 0; i < bridgeSize; i++)
		{
			AxialCoordinates startCell = BridgesAnchorCellCoord[dirInt, i];
			averageStartHeight += currentIsland.GetCellHeightMapValue(startCell);

			AxialCoordinates endCell = BridgesAnchorCellCoord[oppositeDirInt, i];
			averageEndHeight += targetIsland.GetCellHeightMapValue(endCell);
		}

		averageStartHeight = (averageStartHeight / bridgeSize) * heightMultiplier;
		averageEndHeight = (averageEndHeight / bridgeSize) * heightMultiplier;

		int cellCount = BridgeUtils.NbCellInBridge;
		float lerpDivisor = 1f / (distanceBetweenIslands - 1);
		int lastIndex = cellCount - 1;

		for (int i = 0; i < cellCount; i++)
		{
			AxialCoordinates cellCoord = BridgeCellCoords[i];
			int z = cellCoord.R;

			bool isNewRow = i > 0 && BridgeCellCoords[i - 1].R != z;

			// Interpolate height for each cell base on it's z position in the bridge
			float height = Mathf.Lerp(averageStartHeight, averageEndHeight, (z - 1) * lerpDivisor);
			height = NoiseUtils.RoundToNearestHeightStep(height);

			// if the cell is a side cell and raise it for the fence effect
			bool isLastCell = i == lastIndex;
			bool isFirstCell = i == 0;
			bool isEndOfRow = i < lastIndex && BridgeCellCoords[i + 1].R != z;
			bool isSideCell = isNewRow || isEndOfRow || isFirstCell || isLastCell;

			if (isSideCell) height += fenceHeight;

			// Update vertex heights for the current cell
			int vertexIndex = i * 12; // each cell has 12 vertices
			vertices[vertexIndex].y = height;
			vertices[vertexIndex + 1].y = height;
			vertices[vertexIndex + 2].y = height;
			vertices[vertexIndex + 3].y = height;
			vertices[vertexIndex + 4].y = height;
			vertices[vertexIndex + 5].y = height;
		}
		return vertices;
	}
}