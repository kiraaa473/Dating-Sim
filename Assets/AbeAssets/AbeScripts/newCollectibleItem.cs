using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newCollectibleItem : MonoBehaviour
{
    // Attach this script to any object that represents a collectible item.
    // Ensure that the item has a 2D Collider component with the "Trigger" option enabled.
    // Stretch goal: Integrate an Animation Controller to play a destruction animation upon item collection.

    public newItemDefinition itemDefinition;  // Reference to the item's definition
    public int quantity = 1;  // Quantity to add to the inventory when collected
    public AudioClip pickupSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            if (pickupSound)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, 0.7f); // If there is a sound defined, play it once
            }

            newInventory.Instance.AddItem(itemDefinition, quantity);  // Add to inventory
            Destroy(gameObject);  // Remove the collectible from the scene
        }
    }


}
