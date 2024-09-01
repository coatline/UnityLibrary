using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Control Scheme", fileName = "Keyboard&Mouse")]

public class ControlScheme : ScriptableObject
{
    [SerializeField] string displayName;
    [SerializeField] Sprite icon;

    public string Name => name;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
}
