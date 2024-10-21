using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathLinear : MonoBehaviour
{
    public Transform[] pathPoints;
    public float duration = 3.0f; // time to move between points
    private int currentPointIndex = 0; // track the current point in the path
    private Coroutine followPathCoroutine;

    void Start()
    {
        followPathCoroutine = StartCoroutine(FollowPath());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // restart the movement
            if (followPathCoroutine != null)
            {
                StopCoroutine(followPathCoroutine); // stop the currently running coroutine
            }
            currentPointIndex = 0;
            followPathCoroutine = StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        while (currentPointIndex < pathPoints.Length - 1)
        {
            Transform start = pathPoints[currentPointIndex];
            Transform end = pathPoints[currentPointIndex + 1];

            for (float timer = 0; timer < duration; timer += Time.deltaTime)
            {
                float u = timer / duration;

                transform.position = Vector3.Lerp(start.position, end.position, u);

                Vector3 direction = (end.position - start.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, u);

                yield return null;
            }

            // move to the next point
            currentPointIndex++;
        }
    }
}
