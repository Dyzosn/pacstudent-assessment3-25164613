using UnityEngine;

public class Tweener : MonoBehaviour
{
    private Transform target;
    private Vector3 startPos;
    private Vector3 endPos;
    private float duration;
    private float elapsedTime;
    private bool isTweening = false;

    // Start a new tween movement
    public void StartTween(Transform tweenTarget, Vector3 startPosition, Vector3 endPosition, float tweenDuration)
    {
        target = tweenTarget;
        startPos = startPosition;
        endPos = endPosition;
        duration = tweenDuration;
        elapsedTime = 0f;
        isTweening = true;

        // Make sure we start at the right position
        target.position = startPos;
    }

    void Update()
    {
        if (!isTweening) return;

        // Add frame time for smooth movement
        elapsedTime += Time.deltaTime;

        // Check if we're still moving or if we've finished
        if (elapsedTime < duration)
        {
            // Work out how far through the movement we are (0 to 1)
            float t = elapsedTime / duration;

            // Use Lerp to smoothly move between start and end positions
            target.position = Vector3.Lerp(startPos, endPos, t);
        }
        else
        {
            // Movement finished (snap to end position and stop)
            target.position = endPos;
            isTweening = false;
        }
    }

    // Check if we're currently tweening
    public bool IsTweening()
    {
        return isTweening;
    }
}