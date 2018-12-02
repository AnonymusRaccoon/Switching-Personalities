using UnityEngine;

[System.Serializable]
public class Interactable : MonoBehaviour
{
    public int index = -1;
    [TextArea]
    public string[] dialog;
}
