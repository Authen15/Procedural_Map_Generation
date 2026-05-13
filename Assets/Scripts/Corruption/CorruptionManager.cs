using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionManager : MonoBehaviour
{
    public static CorruptionManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool isInitialized = false;

    const int _TOTAL_NB_CELLS = HexMetrics.NbCellInMap * HexMetrics.NbCellInIsland;
    const float TICK_DURATION = 0.5f;

    int _totalCorruptedCells = 0;

    readonly float _corruptionPropagationChance = 0.15f;
    readonly float _islandContagionThreshold = 0.5f;

    // TODO, check if memory usage is acceptable, otherwise only store data for island beeing corrupted and realease it when fully corrupted
    readonly uint[] _allCells = new uint[_TOTAL_NB_CELLS];
    readonly BitArray _allIslands = new BitArray(HexMetrics.NbCellInMap);

    readonly List<int> _currentFrontier = new List<int>();
    readonly List<int> _nextFrontier = new List<int>();

    readonly int[] _islandCorruptedCount = new int[HexMetrics.NbCellInMap];


    public void StartCorruption()
    {
        CorruptFirstCell();
        StartCoroutine(PropagationLoop());
        isInitialized = true;
    }

    public void GetIslandCorrutionData(ComputeBuffer islandBuffer, int islandIndex)
    {
        int startIndex = islandIndex * HexMetrics.NbCellInIsland;

        islandBuffer.SetData(
            _allCells,
            startIndex,
            0,
            HexMetrics.NbCellInIsland
        );
    }

    private void CorruptFirstCell()
    {
        int islandIndex = Random.Range( // random cell in the exterior ring
            HexGridUtils.GetNbCellInHexGrid(HexMetrics.MapRadius - 1),
            HexMetrics.NbCellInMap - 1
        );

        int localCellIndex = Random.Range(
            HexGridUtils.GetNbCellInHexGrid(HexMetrics.IslandRadius - 1),
            HexMetrics.NbCellInIsland - 1
        );

        int globalIndex = ToGlobalIndex(islandIndex, localCellIndex);

        InfectCell(globalIndex, islandIndex, _currentFrontier);

        _allIslands[islandIndex] = true;

        Debug.Log($"corruption starts on island {islandIndex}");
    }

    private IEnumerator PropagationLoop()
    {
        WaitForSeconds tick = new WaitForSeconds(TICK_DURATION);

        bool useCurrent = true;

        while (_totalCorruptedCells < _TOTAL_NB_CELLS)
        {
            if (useCurrent)
            {
                ProcessFrontier(_currentFrontier, _nextFrontier);
                _currentFrontier.Clear();
            }
            else
            {
                ProcessFrontier(_nextFrontier, _currentFrontier);
                _nextFrontier.Clear();
            }

            useCurrent = !useCurrent;

            yield return tick;
        }
    }

    private void ProcessFrontier(List<int> current, List<int> next)
    {
        PropagateCells(current, next);
        SpreadBetweenIslands(next);
    }

    private void PropagateCells(List<int> current, List<int> next)
    {
        // all cells in current frontier
        foreach (int globalCellIndex in current)
        {
            int islandIndex = GetIslandIndex(globalCellIndex);
            int localIndex = GetLocalIndex(globalCellIndex);

            int neighbourIndexOffset = localIndex * 6;

            bool hasUncorruptedNeighbour = false;

            // all 6 neighbours of the cell
            for (int dir = 0; dir < 6; dir++)
            {
                int neighbourLocal = IslandData.CellNeighbours[neighbourIndexOffset + dir];

                if (neighbourLocal < 0 || neighbourLocal >= HexMetrics.NbCellInIsland)
                    continue;

                int neighbourGlobal = ToGlobalIndex(islandIndex, neighbourLocal);

                if (_allCells[neighbourGlobal] == 0)
                {
                    hasUncorruptedNeighbour = true;

                    if (Random.value < _corruptionPropagationChance)
                    {
                        InfectCell(neighbourGlobal, islandIndex, next);
                    }
                }
            }

            // if the cell has at least one uncorrupted neighbour, we keep it in the frontier for the next tick to give it another chance to propagate
            if (hasUncorruptedNeighbour)
            {
                next.Add(globalCellIndex);
            }
        }
    }

    private void SpreadBetweenIslands(List<int> next)
    {
        for (int islandIndex = 0; islandIndex < HexMetrics.NbCellInMap; islandIndex++)
        {
            if (!_allIslands[islandIndex])
                continue;

            float ratio = _islandCorruptedCount[islandIndex] / (float)HexMetrics.NbCellInIsland;

            if (ratio < _islandContagionThreshold)
                continue;

            int neighbourIndexOffset = islandIndex * 6;

            for (int dir = 0; dir < 6; dir++)
            {
                int neighbourIsland = IslandData.CellNeighbours[neighbourIndexOffset + dir];

                if (neighbourIsland < 0 ||
                    neighbourIsland >= HexMetrics.NbCellInMap ||
                    _allIslands[neighbourIsland])
                {
                    continue;
                }

                int randomCell = Random.Range( // random cell in the exterior ring
                    HexGridUtils.GetNbCellInHexGrid(HexMetrics.IslandRadius - 1),
                    HexMetrics.NbCellInIsland - 1
                );

                int globalIndex = ToGlobalIndex(neighbourIsland, randomCell);

                _allIslands[neighbourIsland] = true;

                InfectCell(globalIndex, neighbourIsland, next);

                Debug.Log($"island {neighbourIsland} has been contaminated");
            }
        }
    }

    private static int GetIslandIndex(int globalIndex)
    {
        return globalIndex / HexMetrics.NbCellInIsland;
    }

    private static int GetLocalIndex(int globalIndex)
    {
        return globalIndex % HexMetrics.NbCellInIsland;
    }

    private static int ToGlobalIndex(int islandIndex, int localIndex)
    {
        return islandIndex * HexMetrics.NbCellInIsland + localIndex;
    }

    private void InfectCell(int globalIndex, int islandIndex, List<int> frontier)
    {
        if (_allCells[globalIndex] == 1)
            return;

        _allCells[globalIndex] = 1;

        frontier.Add(globalIndex);

        _islandCorruptedCount[islandIndex]++;
        _totalCorruptedCells++;
    }
}