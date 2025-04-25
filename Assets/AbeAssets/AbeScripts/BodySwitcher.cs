using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySwitcher : MonoBehaviour
{
    // This script enables player-controlled avatar switching, inspired by the game "Thomas Was Alone."
    // Trigger avatar switching via 2D Collider interactions or keypress events.
    // The script disables the <Jump> and <Move> components upon switching avatars, while Rigidbody and other components remain active.
    // Stretch goals: Implement a dictionary to manage the removal of more MonoBehaviour components for greater flexibility and customization.

    [Header("Bodies and Components")]
    public List<GameObject> bodies;  // List of body GameObjects to switch between
    private int currentBodyIndex = 0;  // Index of the currently active body

    [Header("Camera Reference")]
    public CameraFollow cameraFollow;  // Reference to the CameraFollow script

    [Header("Optional Trigger")]
    public bool useTriggerToSwitch = true;  // Enable switching via trigger
    public KeyCode switchKey = KeyCode.E;  // Key to manually switch bodies (if triggers are not used)

    private void Start()
    {
        // Ensure only the first body is active at the start
        ActivateBody(currentBodyIndex);
    }

    private void Update()
    {
        // Allow manual switching if enabled
        if (!useTriggerToSwitch && Input.GetKeyDown(switchKey))
        {
            SwitchBody();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (useTriggerToSwitch && other.CompareTag("Player"))
        {
            SwitchBody();
        }
    }

    public void SwitchBody()
    {
        // Deactivate the current body
        DeactivateBody(currentBodyIndex);

        // Increment the body index (wrap around if at the last body)
        currentBodyIndex = (currentBodyIndex + 1) % bodies.Count;

        // Activate the new body
        ActivateBody(currentBodyIndex);
    }

    private void ActivateBody(int index)
    {
        GameObject body = bodies[index];

        // Enable movement and jump scripts for the active body
        EnableComponent<Move>(body, true);
        EnableComponent<Jump>(body, true);

        // Update the camera target
        if (cameraFollow != null)
        {
            cameraFollow.target = body.transform;
            Debug.Log($"Camera target set to: {body.name}");
        }

        Debug.Log($"Activated body: {body.name}");
    }

    private void DeactivateBody(int index)
    {
        GameObject body = bodies[index];

        // Disable movement and jump scripts for the inactive body
        EnableComponent<Move>(body, false);
        EnableComponent<Jump>(body, false);

        Debug.Log($"Deactivated body: {body.name}");
    }

    private void EnableComponent<T>(GameObject obj, bool enable) where T : MonoBehaviour
    {
        T component = obj.GetComponent<T>();
        if (component != null)
        {
            component.enabled = enable;
        }
    }
}