using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NEW ITEM JACK", menuName = "Item")]
public class Item : ScriptableObject
{
    [SerializeField] ItemType type;

    [TextArea(0, 3)]
    [SerializeField] string description;
    [SerializeField] Sprite sprite;
    [SerializeField] Sound soundOnUse;
    [SerializeField] int cost;
    [SerializeField] int typeId;
    [SerializeField] int id;

    public Sound SoundOnUse => soundOnUse;
    public int Cost => cost;
    public Sprite Sprite => sprite;
    public ItemType Type => type;
    public string Description => description;
    public int Id => id;
    public int TypeId => typeId;

    public string Name => name;
}

public enum ItemType
{
    Gun,
    Sub,
    Special,
    Eyes,
    Mouth
}