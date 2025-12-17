using UnityEngine;

[CreateAssetMenu(menuName = "TuTien/Item Definition", fileName = "NewItem")]
public class ItemDefinition : ScriptableObject
{
    public string id;      
    public string itemName; 
    public Sprite icon; 
    
    [TextArea]
    public string description; 
}