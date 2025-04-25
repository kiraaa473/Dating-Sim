using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class newUIInventoryManager : MonoBehaviour
{
    // This script generates an on-screen, real-time inventory that updates automatically when inventory contents change.
    // Attach this script to an InventoryCanvas GameObject in the hierarchy (see the provided prefab).
    // Stretch goal: Allow access to the inventory menu only when the game is paused.

    [Header("UI References")]
    public GameObject itemSlotPrefab;  // Prefab for item slot
    public Transform inventoryPanel;   // Parent panel to hold item slots

    private Dictionary<newItemDefinition, GameObject> itemSlots = new Dictionary<newItemDefinition, GameObject>();

    void Start()
    {
        // Subscribe to inventory updates
        if (newInventory.Instance != null)
        {
            newInventory.Instance.OnInventoryUpdated += UpdateInventoryUI;

            // Ensure UI reflects current inventory on scene load
            UpdateInventoryUI(newInventory.Instance.GetCurrentInventory());
        }
        else
        {
            Debug.LogWarning("No inventory instance found on startup.");
        }
    }

    void Update()
    {
        // Debug inventory state when ESC is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var inventorySummary = newInventory.Instance.GetInventorySummary();

            if (inventorySummary.Count == 0)
            {
                Debug.Log("Inventory is empty.");
            }
            else
            {
                // Construct a single string listing all items
                string inventoryContents = "Inventory contains: " + string.Join(", ", inventorySummary);
                Debug.Log(inventoryContents);
            }
        }
    }

    void UpdateInventoryUI(Dictionary<newItemDefinition, int> items)
    {
        // Clear existing UI
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }
        itemSlots.Clear();

        // Populate UI with updated inventory
        foreach (var item in items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, inventoryPanel);
            slot.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.Key.icon;
            slot.transform.Find("ItemQuantity").GetComponent<TextMeshProUGUI>().text = item.Value.ToString();
            itemSlots[item.Key] = slot;
        }
    }

    void OnDestroy()
    {
        if (newInventory.Instance != null)
        {
            newInventory.Instance.OnInventoryUpdated -= UpdateInventoryUI;
        }
    }
}