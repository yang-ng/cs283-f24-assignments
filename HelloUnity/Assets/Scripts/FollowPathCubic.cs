using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathCubic : MonoBehaviour
{
    public List<Transform> pathPoints;
    public float duration = 3.0f;
    public bool DeCasteljau = true; 
    private int currentSegmentIndex = 0; 
    private Coroutine movementCoroutine;
    private bool isMoving = false;      

    void Start()
    {
        RestartMovement();
    }


    void Update()
    {
        // press space to restart the movement
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RestartMovement();
        }
    }

    void RestartMovement()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        currentSegmentIndex = 0;
        isMoving = true;
        if (pathPoints.Count > 2)
        {
            movementCoroutine = StartCoroutine(FollowPath());
        }
        else
        {
            Debug.LogError("Path must have at least three points.");
        }
    }

    IEnumerator FollowPath()
    {
        while (currentSegmentIndex < pathPoints.Count - 1)
        {
            Vector3 b0, b1, b2, b3;
            CalculateControlPoints(currentSegmentIndex, out b0, out b1, out b2, out b3);

            for (float t = 0; t < 1; t += Time.deltaTime / duration)
            {
                Vector3 position = DeCasteljau ? ComputeDeCasteljau(b0, b1, b2, b3, t) : ComputePolynomial(b0, b1, b2, b3, t);

                transform.position = position;
                if (t + 0.01f <= 1)
                {
                    Vector3 nextPosition = DeCasteljau ? ComputeDeCasteljau(b0, b1, b2, b3, t + 0.01f) : ComputePolynomial(b0, b1, b2, b3, t + 0.01f);
                    transform.forward = (nextPosition - position).normalized;
                }

                yield return null;
            }

            currentSegmentIndex++;
        }

        isMoving = false;
    }

    void CalculateControlPoints(int index, out Vector3 b0, out Vector3 b1, out Vector3 b2, out Vector3 b3)
    {
        b0 = pathPoints[index].position;
        b3 = pathPoints[(index + 1) % pathPoints.Count].position;

        if (index == 0)
        {
            b1 = b0 + (1.0f / 6.0f) * (b3 - b0);
        }
        else
        {
            Vector3 prevPoint = pathPoints[index - 1].position;
            b1 = b0 + (1.0f / 6.0f) * (pathPoints[index + 1].position - prevPoint);
        }

        if (index == pathPoints.Count - 2)
        {
            b2 = b3 - (1.0f / 6.0f) * (b3 - b0);
        }
        else
        {
            Vector3 nextPoint = pathPoints[index + 2].position;
            b2 = b3 - (1.0f / 6.0f) * (nextPoint - b0);
        }
    }

    Vector3 ComputePolynomial(Vector3 b0, Vector3 b1, Vector3 b2, Vector3 b3, float t)
    {
        float u = 1 - t;
        return (u * u * u) * b0 + 3 * t * (u * u) * b1 + 3 * (t * t) * u * b2 + (t * t * t) * b3;
    }

    Vector3 ComputeDeCasteljau(Vector3 b0, Vector3 b1, Vector3 b2, Vector3 b3, float t)
    {
        Vector3 p0 = Vector3.Lerp(b0, b1, t);
        Vector3 p1 = Vector3.Lerp(b1, b2, t);
        Vector3 p2 = Vector3.Lerp(b2, b3, t);

        Vector3 q0 = Vector3.Lerp(p0, p1, t);
        Vector3 q1 = Vector3.Lerp(p1, p2, t);

        return Vector3.Lerp(q0, q1, t);
    }

    void OnDrawGizmos()
    {
        if (pathPoints.Count < 2) return;

        Gizmos.color = Color.blue;

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 previousPoint = pathPoints[i].position;
            for (float t = 0; t < 1; t += 0.05f)
            {
                Vector3 nextPoint = DeCasteljau ? ComputeDeCasteljau(pathPoints[i].position, pathPoints[i].position + (1.0f / 6.0f) * (pathPoints[i + 1].position - pathPoints[i].position), pathPoints[i + 1].position - (1.0f / 6.0f) * (pathPoints[i + 1].position - pathPoints[i].position), pathPoints[i + 1].position, t) : ComputePolynomial(pathPoints[i].position, pathPoints[i].position + (1.0f / 6.0f) * (pathPoints[i + 1].position - pathPoints[i].position), pathPoints[i + 1].position - (1.0f / 6.0f) * (pathPoints[i + 1].position - pathPoints[i].position), pathPoints[i + 1].position, t);
                Gizmos.DrawLine(previousPoint, nextPoint);
                previousPoint = nextPoint;
            }
        }
    }
}