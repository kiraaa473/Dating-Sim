using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryReset : MonoBehaviour
{
    // This script offers a public method to reset the current inventory instance upon request.
    // Leverage Unity's Playground system to trigger inventory reset via collisions, triggers, or keypresses using Conditions.
    // Attach this script to the InventoryReset GameObject (see the provided prefab).
    // To use, drag the InventoryReset into the Custom Actions option in Playground's Conditions and select ResetInventory().

    public void ResetInventory()
    {
        if (newInventory.Instance != null)
        {
            newInventory.Instance.ClearInventory();
        }
        else
        {
            Debug.LogWarning("newInventory instance not found.");
        }
    }

}
