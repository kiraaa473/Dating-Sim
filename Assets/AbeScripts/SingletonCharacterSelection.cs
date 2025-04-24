using UnityEngine;

public class SingletonCharacterSelection : MonoBehaviour
{
    // The one and only instance of this class (shared across all scenes)
    public static SingletonCharacterSelection Instance { get; private set; }

    // The index of the character selected by Player 1
    public int selectedCharacterIndexP1 = 0;

    // The index of the character selected by Player 2
    // If this stays at -1, the game assumes it's a 1-player match
    public int selectedCharacterIndexP2 = -1;

    // This function runs before Start(), even if the object is inactive
    private void Awake()
    {
        // If there's already an instance, destroy the duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent multiple singletons
            return;
        }

        // Otherwise, set this as the active singleton
        Instance = this;

        // Keep this object alive when switching scenes
        DontDestroyOnLoad(gameObject);
    }
}