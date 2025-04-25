using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newInventory : MonoBehaviour
{
    // This script manages the inventory system and should be attached to a prefab called "InventoryManager" in the hierarchy.
    // It tracks the items the player has collected and ensures their persistence.
    // Stretch goal: Implement saving and loading the inventory from a "Save file."

    // Singleton instance
    public static newInventory Instance;

    // Stores item types and their quantities
    private Dictionary<newItemDefinition, int> items = new Dictionary<newItemDefinition, int>();

    // Define an event that notifies subscribers when the inventory is updated
    public event System.Action<Dictionary<newItemDefinition, int>> OnInventoryUpdated;

    // Awake runs when the game starts
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keeps this object persistent across scenes
        }
        else
        {
            Destroy(gameObject);  // Ensure only one instance exists
        }
    }

    // Public function to add an item to the inventory
    public void AddItem(newItemDefinition item, int quantity = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] += quantity;
        }
        else
        {
            items[item] = quantity;
        }
        NotifyInventoryUpdated();  // Trigger the update event
        Debug.Log("Inventory Updated: " + string.Join(", ", GetInventorySummary()));
    }

    // Checks if the inventory has a specific item
    public bool HasItem(newItemDefinition item, int requiredQuantity = 1)
    {
        return items.ContainsKey(item) && items[item] >= requiredQuantity;
    }

    // Removes a specific quantity of an item from the inventory
    public void RemoveItem(newItemDefinition item, int quantity)
    {
        Debug.Log($"RemoveItem called for {item.itemName} with quantity {quantity}.");
        if (items.ContainsKey(item))
        {
            items[item] -= quantity;
            if (items[item] <= 0)
            {
                items.Remove(item);
            }
            NotifyInventoryUpdated();
        }
        else
        {
            Debug.LogWarning($"Cannot remove {item.itemName}: Item not found in inventory.");
        }
    }

    // Remove all items from the inventory
    public void ClearInventory()
    {
        items.Clear();  // Remove all items from the inventory
        NotifyInventoryUpdated();  // Notify subscribers of the update
        Debug.Log("Inventory cleared.");
    }

    // Notify subscribers of an inventory update
    private void NotifyInventoryUpdated()
    {
        OnInventoryUpdated?.Invoke(items);
    }

    // Helper function to summarize inventory for debugging or UI display
    public List<string> GetInventorySummary()
    {
        List<string> summary = new List<string>();
        foreach (var item in items)
        {
            summary.Add($"{item.Key.itemName}: {item.Value}");
        }
        return summary;
    }

    //Get's current inventory
    public Dictionary<newItemDefinition, int> GetCurrentInventory()
    {
        return new Dictionary<newItemDefinition, int>(items);  // Return a copy for safety
    }
}