using UnityEngine;
using UnityEngine.InputSystem;

public class DisplayAffinityUI : MonoBehaviour
{
    public GameObject AffinityUI;

    public void OnShowLevel(InputAction.CallbackContext context)
    {
        AffinityUI.SetActive(!AffinityUI.activeSelf);
    }

    public void Start()
    {
        AffinityUI.SetActive(false);
    }
}
