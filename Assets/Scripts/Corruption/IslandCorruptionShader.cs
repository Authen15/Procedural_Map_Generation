using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Island))]
public class IslandCorruptionShader : MonoBehaviour
{
    private MaterialPropertyBlock _mpb;
    private MeshRenderer _renderer;

    private CorruptionManager _corruptionManager;
    private Island _island;

    private ComputeBuffer _islandCorruptionBuffer;

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _mpb = new MaterialPropertyBlock();

        _corruptionManager = CorruptionManager.Instance;
        _island = GetComponent<Island>();

        _islandCorruptionBuffer = new ComputeBuffer(
            HexMetrics.NbCellInIsland,
            sizeof(uint)
        );
    }

    void OnEnable()
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetBuffer("_CorruptionBuffer", _islandCorruptionBuffer);
        _renderer.SetPropertyBlock(_mpb);
    }

    void Update()
    {
        if (!_corruptionManager.isInitialized) return;

        _corruptionManager.GetIslandCorrutionData(_islandCorruptionBuffer, _island.Index);
    }

    void OnDestroy()
    {
        if (_islandCorruptionBuffer != null)
        {
            _islandCorruptionBuffer.Release();
            _islandCorruptionBuffer = null;
        }
    }
}