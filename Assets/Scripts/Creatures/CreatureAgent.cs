using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CreatureAgent : MonoBehaviour
{    
    public GameObject player;
    public NavMeshAgent agent;
    public Image SearchIcon;
    public Image TriggeredIcon;

    private Camera mainCamera;

    private NavMeshPath path;
    private float elapsed = 0.0f;

    public bool isTriggered = false;
    public float TriggerDistance = 15f;
    public float UnTriggerDisance = 18f;
    public float SearchDistance = 22f;

    public float TargetPositionDelta = 0.5f;

    public const float UPDATE_TIME = 0.25f;

    public void Start() {
        mainCamera = Camera.main;
        path = new NavMeshPath();
        elapsed = 0.0f;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update() {
        elapsed += Time.deltaTime;
        if (elapsed > UPDATE_TIME)
        {
            elapsed = 0.0f;
            UpdateAgentDestination();
        }
        UpdateUI();
    }

    private void UpdateAgentDestination(){
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (isTriggered){
            if (distanceToPlayer > UnTriggerDisance){
                agent.SetPath(path); // agent will go to the last position where the player was seen
                isTriggered = false;
            }else{
                MoveTowardsPlayer();
            }
        }else{
            if (distanceToPlayer < TriggerDistance && distanceToPlayer > TargetPositionDelta){
                MoveTowardsPlayer();
                isTriggered = true;
            }
        }
    }

    private void MoveTowardsPlayer(){
        NavMesh.CalculatePath(transform.position, player.transform.position, NavMesh.AllAreas, path);
        agent.SetPath(path);
    }

#region UI
    private void UpdateUI(){
        if (isTriggered){
            TriggeredIcon.enabled = true;
            SearchIcon.enabled = false;
        }else if (Vector3.Distance(transform.position, player.transform.position) < SearchDistance){
            SearchIcon.enabled = true;
            TriggeredIcon.enabled = false;
        }else{
            TriggeredIcon.enabled = false;
            SearchIcon.enabled = false;
        }

        SearchIcon.transform.rotation = mainCamera.transform.rotation;
        TriggeredIcon.transform.rotation = mainCamera.transform.rotation;
    }
#endregion
}