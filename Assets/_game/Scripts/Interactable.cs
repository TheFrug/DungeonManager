using UnityEngine;
using Yarn.Unity;

public class Interactable : MonoBehaviour
{
    public enum DefaultFacing { Right, Left }

    [Header("Interaction")]
    [SerializeField] private bool isCharacter = false;

    [Header("Facing (Characters Only)")]
    [SerializeField] private DefaultFacing defaultFacing = DefaultFacing.Right;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Dialogue (Optional)")]
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string startNode = "Start";

    [Header("Yarn Variables")]
    [Tooltip("Optional: variable name to count interactions (int)")]
    [SerializeField] private string interactionCountVariable;

    public bool playerInRange;
    private Transform playerTransform;

    private bool originalFlipX;

    private void Awake()
    {
        if (isCharacter && spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (isCharacter && spriteRenderer == null)
            Debug.LogError($"[Interactable] No SpriteRenderer found on {name}");
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    private void Interact()
    {
        if (isCharacter)
        {
            originalFlipX = spriteRenderer != null && spriteRenderer.flipX;

            if (dialogueRunner != null)
            {
                dialogueRunner.onDialogueComplete.AddListener(RestoreOriginalFacing);
                dialogueRunner.onDialogueComplete.AddListener(IncrementInteractionCount);
            }
        }

        if (dialogueRunner == null || string.IsNullOrEmpty(startNode)) return;
        if (dialogueRunner.IsDialogueRunning) return;

        dialogueRunner.StartDialogue(startNode);
    }

    // ===== YARN COMMAND =====
    [YarnCommand("face_player")]
    public void FacePlayerCommand()
    {
        FacePlayer();
    }

    private void FacePlayer()
    {
        if (!isCharacter) return;
        if (spriteRenderer == null || playerTransform == null) return;

        bool playerIsOnLeft = playerTransform.position.x < transform.position.x;

        if (defaultFacing == DefaultFacing.Right)
            spriteRenderer.flipX = playerIsOnLeft;
        else
            spriteRenderer.flipX = !playerIsOnLeft;
    }

    private void IncrementInteractionCount()
    {
        if (string.IsNullOrEmpty(interactionCountVariable) || dialogueRunner == null)
            return;

        if (dialogueRunner.VariableStorage is InMemoryVariableStorage storage)
        {
            storage.TryGetValue(interactionCountVariable, out float currentValue);
            storage.SetValue(interactionCountVariable, currentValue + 1);
        }

        dialogueRunner.onDialogueComplete.RemoveListener(IncrementInteractionCount);
    }

    private void RestoreOriginalFacing()
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = originalFlipX;

        if (dialogueRunner != null)
            dialogueRunner.onDialogueComplete.RemoveListener(RestoreOriginalFacing);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerTransform = null;
        }
    }
}
