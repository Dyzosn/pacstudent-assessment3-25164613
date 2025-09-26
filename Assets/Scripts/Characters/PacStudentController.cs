using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    public float tweenDuration = 1.0f;
    public Vector3[] positions = new Vector3[4];
    public int currentStartPosition = 0;

    public Animator pacAnimator;
    public AudioSource movingAudio;

    private Tweener tweener;
    private int targetIndex = 0;

    void Start()
    {
        // Get the tweener component
        tweener = GetComponent<Tweener>();

        // Set starting position
        transform.position = positions[currentStartPosition];

        // Start the first movement
        MoveToNextPosition();

        // Start playing movement sound
        if (movingAudio != null)
        {
            movingAudio.Play();
        }
    }

    void Update()
    {
        // Check if current tween is finished
        if (!tweener.IsTweening())
        {
            // Move to next waypoint
            MoveToNextPosition();
        }
    }

    void MoveToNextPosition()
    {
        // Figure out where we're going next (clockwise cycle)
        targetIndex = (targetIndex + 1) % positions.Length;

        // Update animation for this direction
        UpdateAnimationDirection();

        // Start the tween to next position
        tweener.StartTween(transform, transform.position, positions[targetIndex], tweenDuration);
    }

    void UpdateAnimationDirection()
    {
        // Work out which direction we're moving
        Vector3 direction = (positions[targetIndex] - transform.position).normalized;

        // Reset all animation parameters
        pacAnimator.SetInteger("left", 0);
        pacAnimator.SetInteger("right", 0);
        pacAnimator.SetInteger("up", 0);
        pacAnimator.SetInteger("down", 0);

        // Set the right animation based on movement direction
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Moving horizontally
            if (direction.x > 0)
            {
                pacAnimator.SetInteger("right", 1);
            }
            else
            {
                pacAnimator.SetInteger("left", 0);
            }
        }
        else
        {
            // Moving vertically
            if (direction.y > 0)
            {
                pacAnimator.SetInteger("up", 2);
            }
            else
            {
                pacAnimator.SetInteger("down", 3);
            }
        }
    }

    void OnDisable()
    {
        if (movingAudio != null && movingAudio.isPlaying)
        {
            movingAudio.Stop();
        }
    }
}