using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class newNPCDialogue : MonoBehaviour
{
    // This script should be attached to an NPC in the hierarchy.
    // It enables the display of speech balloons (using the NewUIInventoryManager.cs) with item request dialogue before and after interaction.
    // NPCs can request items in specific quantities and control the activation/deactivation of GameObjects upon fulfilling item requests.
    // Stretch goals: Implement multiple player choices, integrate with the quest system, and activate quests based on NPC interactions.

    [Header("Dialogue Settings")]
    public string[] requestDialogue;  // Lines for item request
    public string[] fulfilledDialogue;  // Lines after request is fulfilled

    [Header("Item Requirements")]
    public bool requiresItems;  // Does this NPC request items?
    [System.Serializable]
    public class ItemRequirement
    {
        public newItemDefinition item;  // The item type
        public int quantity;  // The required quantity
    }
    public List<ItemRequirement> requiredItemsList = new List<ItemRequirement>();  // List of required items

    [Header("Fulfillment Actions")]
    public List<GameObject> objectsToActivate = new List<GameObject>();  // Objects to activate when fulfilled
    public List<GameObject> objectsToDeactivate = new List<GameObject>();  // Objects to deactivate when fulfilled

    [Header("UI References")]
    public GameObject speechBalloonPrefab;  // Speech balloon prefab
    public Canvas speechBalloonCanvas;  // Reference to the speech balloon canvas (must be in the scene)
    public Transform speechAnchor;  // Anchor where the speech balloon should appear
    private GameObject currentBalloon;  // Active speech balloon instance
    private TextMeshProUGUI dialogueText;  // Text component of the speech balloon
    private TextMeshProUGUI advanceLegend;  // Reference to the legend text

    public KeyCode keyToAdvance = KeyCode.Space;  // Key to advance dialogue
    private int currentLineIndex = 0;  // Tracks the current dialogue line
    private bool isFulfilled = false;  // Tracks dialogue state

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartDialogue();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EndDialogue();
        }
    }

    private void StartDialogue()
    {
        currentLineIndex = 0;

        // Check if NPC is already fulfilled or can be fulfilled now
        if (requiresItems && HasAllRequiredItems())
        {
            RemoveAllRequiredItems();  // Remove items right away
            FulfillActions();         // Trigger fulfillment actions
            isFulfilled = true;       // Mark as fulfilled
        }

        ShowDialogue();
    }

    private void ShowDialogue()
    {
        if (currentBalloon == null)
        {
            // Create the speech balloon
            currentBalloon = Instantiate(speechBalloonPrefab, speechBalloonCanvas.transform);
            currentBalloon.transform.position = speechAnchor.position;

            dialogueText = currentBalloon.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
            advanceLegend = currentBalloon.transform.Find("AdvanceLegend").GetComponent<TextMeshProUGUI>();
        }

        string[] dialogueLines = isFulfilled ? fulfilledDialogue : requestDialogue;

        if (dialogueLines.Length > 0)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
            currentBalloon.SetActive(true);

            // Show legend only if multiple lines
            advanceLegend.text = $"Press <b>{keyToAdvance}</b> ...";
            advanceLegend.gameObject.SetActive(dialogueLines.Length > 1);
        }

    }

    private void Update()
    {
        if (currentBalloon != null && Input.GetKeyDown(keyToAdvance))
        {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        string[] dialogueLines = isFulfilled ? fulfilledDialogue : requestDialogue;

        if (currentLineIndex < dialogueLines.Length - 1)
        {
            currentLineIndex++;
            dialogueText.text = dialogueLines[currentLineIndex];
        }
        else
        {

            if (!isFulfilled && requiresItems && HasAllRequiredItems())
            {
                RemoveAllRequiredItems();  // Ensure items are removed here
                FulfillActions();         // Execute actions
                isFulfilled = true;       // Mark as fulfilled
                ResetDialogueToFulfilled();  // Reset dialogue to fulfilled state
                return;
            }

            EndDialogue();  // End the dialogue if no more lines or actions needed
        }

        if (currentLineIndex >= dialogueLines.Length - 1)
        {
            advanceLegend.gameObject.SetActive(false);  // Hide legend at the last line
        }
    }

    private void FulfillActions()
    {

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"Activated: {obj.name}");
            }
        }

        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Debug.Log($"Deactivated: {obj.name}");
            }
        }
    }

    private void EndDialogue()
    {
        if (currentBalloon != null)
        {
            Destroy(currentBalloon);
            currentBalloon = null;
        }

    }

    private bool HasAllRequiredItems()
    {
        foreach (var requirement in requiredItemsList)
        {
            if (!newInventory.Instance.HasItem(requirement.item, requirement.quantity))
            {
                return false;
            }
        }

        return true;
    }

    private void RemoveAllRequiredItems()
    {

        foreach (var requirement in requiredItemsList)
        {
            newInventory.Instance.RemoveItem(requirement.item, requirement.quantity);
        }

    }

    private void ResetDialogueToFulfilled()
    {
        currentLineIndex = 0;
        ShowDialogue();
    }
}