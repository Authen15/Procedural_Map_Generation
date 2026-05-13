using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MapManager : MonoBehaviour
{
	public static MapManager Instance { get; private set; }
	public int MapSeed = 1;

	public Island[] IslandViews;

	private List<int> _activeIndices; //store the indices of currently active islands
	private ObjectPool<Island> _islandPool;

	[SerializeField] private Island _islandPrefab;
	private IslandDisplay _islandDisplay;

	private Coroutine _islandGenerationCoroutine;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	void Start()
	{
		TemplateManager.InitializeTemplates();

		_islandPool = new ObjectPool<Island>(
			createFunc: () => Instantiate(_islandPrefab),
			actionOnGet: island => island.gameObject.SetActive(true),
			actionOnRelease: island => island.gameObject.SetActive(false),
			actionOnDestroy: island => Destroy(island.gameObject),
			defaultCapacity: HexGridUtils.GetNbCellInHexGrid(1),
			maxSize: HexGridUtils.GetNbCellInHexGrid(2)
		);

		IslandViews = new Island[HexMetrics.NbCellInMap];
		_activeIndices = new List<int>();

		_islandDisplay = new IslandDisplay(this);
		CorruptionManager.Instance.StartCorruption();
	}


	public void DestroyIslands(int[] islandIds)
	{
		if (_islandGenerationCoroutine != null)
		{
			StopCoroutine(_islandGenerationCoroutine);
			_islandGenerationCoroutine = null;
		}

		foreach (int islandId in islandIds)
		{
			_islandPool.Release(IslandViews[islandId]);
			_activeIndices.Remove(islandId);
			IslandViews[islandId].Cleanup();
			IslandViews[islandId].gameObject.SetActive(false);
			IslandViews[islandId] = null;
		}
	}

	public void GenerateIslands(int[] islandIds)
	{
		_islandGenerationCoroutine = StartCoroutine(GenerateIslandsCoroutine(islandIds));
	}

	private IEnumerator GenerateIslandsCoroutine(int[] islandIds)
	{
		foreach (int islandId in islandIds)
		{
			_activeIndices.Add(islandId);
			IslandViews[islandId] = _islandPool.Get();
			IslandViews[islandId].gameObject.SetActive(true);
			IslandViews[islandId].Initialize(islandId);
			yield return null; // generate one per frame
		}

		// updating bridges after islands as bridges need to know their neighbours
		foreach (int activeIsland in _activeIndices)
		{
			IslandViews[activeIsland].UpdateBridges();
			yield return null;
		}

		_islandGenerationCoroutine = null;
	}

}
