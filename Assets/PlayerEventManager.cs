using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static HexGridUtils;

public class PlayerEventManager : MonoBehaviour
{
    private static PlayerEventManager _instance;
	public static PlayerEventManager Instance{
		get{
			if (_instance == null){
				Debug.LogWarning("PlayerEventManager is null");
			}
			return _instance;
		}
	}

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public UnityEvent<AxialCoordinates> OnIslandChanged; // The player entered a new island


    private const float _tickDelay = 0.05f;
    private float _counter = 0f;

    private AxialCoordinates _previousIslandCoord;

    public void Update()
    {
        if (_counter >= _tickDelay){
            UpdateCurrentIsland();
            _counter = 0;
        }
        _counter += Time.deltaTime;
    }

    void UpdateCurrentIsland(){
        AxialCoordinates currentIslandCoord = WorldToIsland(transform.position);
        if (_previousIslandCoord == null || _previousIslandCoord != currentIslandCoord){
            OnIslandChanged?.Invoke(currentIslandCoord);
            Debug.Log("Player has changed island to " + currentIslandCoord.ToString());

            _previousIslandCoord = currentIslandCoord;
        }
    }
}