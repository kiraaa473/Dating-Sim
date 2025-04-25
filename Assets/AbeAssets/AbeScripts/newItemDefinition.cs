using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class newItemDefinition : ScriptableObject
{
    // This script extends ScriptableObject and creates Items that are accessible across scenes.
    // Stretch Goals: Implement item subcategories (e.g., different metal strengths),
    // incorporate sound components, and seperate in-game and inventory art.

    public string itemName;  // Unique name for the item
    public Sprite icon;      // Optional: Icon for UI display
}
