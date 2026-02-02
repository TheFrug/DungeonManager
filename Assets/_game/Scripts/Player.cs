using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;

    [SerializeField] private Animator myAnimator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private DialogueRunner dialogueRunner;

    private bool movementLocked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueStart.AddListener(LockMovement);
            dialogueRunner.onDialogueComplete.AddListener(UnlockMovement);
        }
    }

    private void OnDisable()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueStart.RemoveListener(LockMovement);
            dialogueRunner.onDialogueComplete.RemoveListener(UnlockMovement);
        }
    }

    private void Update()
    {
        FinishQuest();

        // Quit game with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        }

        // Restart scene with Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }

    }

    private void FixedUpdate()
    {
        if (movementLocked)
        {
            HaltMovement();
            return;
        }

        Vector2 input = ReadMovementInput();
        HandleMovement(input);
        HandleFlip(input.x);
        HandleAnimation(input);
    }

    // -------------------------
    // INPUT
    // -------------------------

    private Vector2 ReadMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        return new Vector2(x, y);
    }

    private void FinishQuest()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            var storage = dialogueRunner.VariableStorage as InMemoryVariableStorage;
            storage.SetValue("$QuestComplete_BigDemon", true);
        }
    }
    // -------------------------
    // MOVEMENT
    // -------------------------

    private void HandleMovement(Vector2 input)
    {
        Vector2 move = input.normalized;
        rb.velocity = move * speed;
    }

    private void HaltMovement()
    {
        rb.velocity = Vector2.zero;
        myAnimator.SetBool("isRunning", false);
    }

    // -------------------------
    // VISUALS
    // -------------------------

    private void HandleFlip(float horizontalInput)
    {
        if (horizontalInput < 0)
            spriteRenderer.flipX = true;
        else if (horizontalInput > 0)
            spriteRenderer.flipX = false;
    }

    private void HandleAnimation(Vector2 input)
    {
        myAnimator.SetBool("isRunning", input.x != 0 || input.y != 0);
    }

    // -------------------------
    // DIALOGUE CONTROL
    // -------------------------

    private void LockMovement()
    {
        movementLocked = true;
        HaltMovement();
    }

    private void UnlockMovement()
    {
        movementLocked = false;
    }
}
