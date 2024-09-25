using UnityEngine;

public class PlayerGridPosition : MonoBehaviour
{
    
    public float Delay = 0.1f;
    private float _counter = 0f;

    void Update()
    {
        if (_counter >= Delay){
            Debug.Log(HexGridUtils.WorldToCell(transform.position));
            _counter = 0;
        }
        _counter += Time.deltaTime;
    }
}
