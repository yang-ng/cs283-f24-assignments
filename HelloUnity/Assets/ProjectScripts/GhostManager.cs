using UnityEngine;
using System.Collections.Generic;

public class GhostManager : MonoBehaviour
{
    public List<GameObject> ghosts;
    public GameObject goalkeeper;
    public GameObject ball;
    public GameObject plumbobPrefab;

    private GameObject controlledGhost;
    private GameObject currentPlumbob;
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
        
        controlledGhost = closestGhost;

        foreach (GameObject ghost in ghosts)
        {
            if (ghost == goalkeeper)
            {
                continue;
            }

            if (ghost == controlledGhost)
            {
                ghost.GetComponent<GhostMotionController>().enabled = true;
                ghost.GetComponent<PlayerTeamBehavior>().enabled = false;
            }
            else
            {
                ghost.GetComponent<GhostMotionController>().enabled = false;
                ghost.GetComponent<PlayerTeamBehavior>().enabled = true;
            }
        }

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

            // only switch control if the closest ghost is closer than the threshold
            if (closestGhost != controlledGhost && closestDistance < currentDistance - switchThreshold)
            {
                controlledGhost.GetComponent<GhostMotionController>().enabled = false;
                if (controlledGhost == goalkeeper)
                {
                    goalkeeper.GetComponent<PlayerGK>().OnPlayerRelease();
                }
                else
                {
                    controlledGhost.GetComponent<PlayerTeamBehavior>().enabled = true;
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
                        controlledGhost.GetComponent<PlayerTeamBehavior>().enabled = false;
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
            currentPlumbob = Instantiate(plumbobPrefab);
        }
        currentPlumbob.transform.SetParent(ghost.transform);
        currentPlumbob.transform.localPosition = new Vector3(0, 1.5f, 0);
    }
}