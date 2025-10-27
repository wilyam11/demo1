using UnityEngine;

public class MaskToggleController : MonoBehaviour
{
    // Drag your "Hexagon Mask Overlay" panel here in the Inspector
    public GameObject maskObject; 

    // This method can be called by an Input System event, button, or game logic
    public void ToggleMaskVisibility()
    {
        // Toggles the active state of the GameObject
        maskObject.SetActive(!maskObject.activeSelf);
    }

    // Example: Turn the mask on/off completely
    public void SetMaskState(bool isVisible)
    {
        maskObject.SetActive(isVisible);
    }
}