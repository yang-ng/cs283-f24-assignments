using UnityEngine;
using System.Collections.Generic;

public class GhostManager : MonoBehaviour
{
    public List<GameObject> ghosts; // ghosts of the player's team
    public GameObject goalkeeper; // goalkeeper of the player's team
    public GameObject ball; // soccer ball
    public GameObject plumbobPrefab; // prefab for the plumbob

    private GameObject controlledGhost; // currently controlled ghost
    private GameObject currentPlumbob; // plumbob instance
    private const float switchThreshold = 0.2f; // threshold for switching control between ghosts

    void Start()
    {
        SetInitialControlledGhost();
    }

    void Update()
    {
        UpdateControlledGhost();
    }

    void SetInitialControlledGhost()
    {
        float closestDistance = Mathf.Infinity;
        GameObject closestGhost = null;

        foreach (GameObject ghost in ghosts)
        {
            float distanceToBall = Vector3.Distance(ghost.transform.position, ball.transform.position);
            if (distanceToBall < closestDistance)
            {
                closestDistance = distanceToBall;
                closestGhost = ghost;
            }
        }
        
        // Set the controlled ghost
        controlledGhost = closestGhost;

        foreach (GameObject ghost in ghosts)
        {
            // Skip goalkeeper in this loop
            if (ghost == goalkeeper)
            {
                continue;
            }

            if (ghost == controlledGhost)
            {
                ghost.GetComponent<GhostMotionController>().enabled = true;

                if (ghost.TryGetComponent<TeammatesWander>(out TeammatesWander wander))
                {
                    wander.enabled = false;
                }
            }
            else
            {
                ghost.GetComponent<GhostMotionController>().enabled = false;

                if (ghost.TryGetComponent<TeammatesWander>(out TeammatesWander wander))
                {
                    wander.enabled = true;
                }
            }
        }

        // handle goalkeeper-specific behavior
        if (controlledGhost == goalkeeper)
        {
            goalkeeper.GetComponent<PlayerGK>().OnPlayerControl();
        }
        else
        {
            goalkeeper.GetComponent<GhostMotionController>().enabled = false;
            goalkeeper.GetComponent<PlayerGK>().OnPlayerRelease();
        }

        if (controlledGhost != null)
        {
            SetPlumbob(controlledGhost);
        }
    }

    void UpdateControlledGhost()
    {
        float closestDistance = Mathf.Infinity;
        GameObject closestGhost = null;

        foreach (GameObject ghost in ghosts)
        {
            float distanceToBall = Vector3.Distance(ghost.transform.position, ball.transform.position);
            if (distanceToBall < closestDistance)
            {
                closestDistance = distanceToBall;
                closestGhost = ghost;
            }
        }

        if (controlledGhost != null)
        {
            float currentDistance = Vector3.Distance(controlledGhost.transform.position, ball.transform.position);

            // Only switch control if the closest ghost is closer than the threshold
            if (closestGhost != controlledGhost && closestDistance < currentDistance - switchThreshold)
            {
                // Transfer control to the closest ghost
                controlledGhost.GetComponent<GhostMotionController>().enabled = false;
                if (controlledGhost == goalkeeper)
                {
                    goalkeeper.GetComponent<PlayerGK>().OnPlayerRelease();
                }
                else
                {
                    controlledGhost.GetComponent<TeammatesWander>().enabled = true;
                }

                controlledGhost = closestGhost;

                if (controlledGhost != null)
                {
                    controlledGhost.GetComponent<GhostMotionController>().enabled = true;
                    if (controlledGhost == goalkeeper)
                    {
                        goalkeeper.GetComponent<PlayerGK>().OnPlayerControl();
                    }
                    else
                    {
                        controlledGhost.GetComponent<TeammatesWander>().enabled = false;
                    }

                    SetPlumbob(controlledGhost);
                }
            }
        }
    }

    void SetPlumbob(GameObject ghost)
    {
        if (currentPlumbob == null)
        {
            // Instantiate the plumbob if not existing
            currentPlumbob = Instantiate(plumbobPrefab);
        }

        // Attach the plumbob to the ghost and position it above
        currentPlumbob.transform.SetParent(ghost.transform);
        currentPlumbob.transform.localPosition = new Vector3(0, 1.5f, 0);
    }
}