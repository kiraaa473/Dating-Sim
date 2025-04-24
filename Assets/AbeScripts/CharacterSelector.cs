using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CharacterSelector : MonoBehaviour
{
    [Header("UI Elements")]
    public List<RectTransform> characterSlots;       // All the character icons laid out in a grid
    public RectTransform selectorHighlightP1;        // The highlight box for Player 1
    public RectTransform selectorHighlightP2;        // The highlight box for Player 2

    [Header("Scene Settings")]
    public string nextScene;                         // Name of the scene to load after selection

    [Header("Mode & Input")]
    public bool isTwoPlayer = true;                  // Toggle for 1P vs 2P mode
    public int columns = 3;                          // How many columns in the character grid
    public KeyCode confirmKeyP1 = KeyCode.Return;    // Button to confirm selection (Player 1)
    public KeyCode confirmKeyP2 = KeyCode.Space;     // Button to confirm selection (Player 2)

    [Header("Audio")]
    public AudioSource audioSource;                  // Audio source to play all sounds
    public AudioClip moveSFX;                        // Sound when selector moves
    public AudioClip confirmPlayerSFX;               // Sound when each player confirms
    public AudioClip confirmFanfareSFX;              // Sound that plays before loading the next scene

    private int currentIndexP1 = 0;
    private int currentIndexP2 = 1;

    private bool p1Confirmed = false;
    private bool p2Confirmed = false;
    private bool sceneLoading = false;

    void Start()
    {
        // Disable Player 2's selector if in single-player mode
        if (!isTwoPlayer && selectorHighlightP2 != null)
        {
            selectorHighlightP2.gameObject.SetActive(false);
        }

        // Wait a frame before positioning highlights, so UI layout has time to finish
        StartCoroutine(InitSelectors());
    }

    private IEnumerator InitSelectors()
    {
        yield return new WaitForEndOfFrame();
        MoveSelector(selectorHighlightP1, currentIndexP1);
        if (isTwoPlayer)
            MoveSelector(selectorHighlightP2, currentIndexP2);
    }

    void Update()
    {
        if (sceneLoading) return;

        // Handle input for each player
        if (!p1Confirmed) HandleInputP1();
        if (isTwoPlayer && !p2Confirmed) HandleInputP2();

        // Once both players have confirmed (or just P1 in 1P mode), load next scene
        if (p1Confirmed && (!isTwoPlayer || p2Confirmed))
        {
            StartCoroutine(PlayConfirmThenLoadScene());
        }
    }

    void HandleInputP1()
    {
        // Navigation controls for Player 1 (Arrow Keys)
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(1, ref currentIndexP1, selectorHighlightP1);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(-1, ref currentIndexP1, selectorHighlightP1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Move(columns, ref currentIndexP1, selectorHighlightP1);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Move(-columns, ref currentIndexP1, selectorHighlightP1);

        // Confirm selection for Player 1
        if (Input.GetKeyDown(confirmKeyP1))
        {
            p1Confirmed = true;
            if (audioSource && confirmPlayerSFX) audioSource.PlayOneShot(confirmPlayerSFX);
        }
    }

    void HandleInputP2()
    {
        // Navigation controls for Player 2 (WASD)
        if (Input.GetKeyDown(KeyCode.D)) Move(1, ref currentIndexP2, selectorHighlightP2);
        if (Input.GetKeyDown(KeyCode.A)) Move(-1, ref currentIndexP2, selectorHighlightP2);
        if (Input.GetKeyDown(KeyCode.S)) Move(columns, ref currentIndexP2, selectorHighlightP2);
        if (Input.GetKeyDown(KeyCode.W)) Move(-columns, ref currentIndexP2, selectorHighlightP2);

        // Confirm selection for Player 2
        if (Input.GetKeyDown(confirmKeyP2))
        {
            p2Confirmed = true;
            if (audioSource && confirmPlayerSFX) audioSource.PlayOneShot(confirmPlayerSFX);
        }
    }

    // Moves the selector highlight to the correct character
    void Move(int delta, ref int index, RectTransform selector)
    {
        int newIndex = Mathf.Clamp(index + delta, 0, characterSlots.Count - 1);
        if (newIndex != index)
        {
            index = newIndex;
            MoveSelector(selector, index);
            if (audioSource && moveSFX) audioSource.PlayOneShot(moveSFX);
        }
    }

    void MoveSelector(RectTransform selector, int index)
    {
        selector.position = characterSlots[index].position;
    }

    // Plays the fanfare, saves the selected characters, then loads the fight scene
    IEnumerator PlayConfirmThenLoadScene()
    {
        sceneLoading = true;

        if (audioSource && confirmFanfareSFX)
            audioSource.PlayOneShot(confirmFanfareSFX);

        // Save selection to singleton
        SingletonCharacterSelection.Instance.selectedCharacterIndexP1 = currentIndexP1;
        SingletonCharacterSelection.Instance.selectedCharacterIndexP2 = isTwoPlayer ? currentIndexP2 : -1;

        // Wait for fanfare to finish
        yield return new WaitForSeconds(confirmFanfareSFX.length);

        SceneManager.LoadScene(nextScene);
    }
}