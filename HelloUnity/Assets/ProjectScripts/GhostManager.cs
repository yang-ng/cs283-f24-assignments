using UnityEngine;
using System.Collections.Generic;

public class GhostManager : MonoBehaviour
{
    public List<GameObject> ghosts; // ghosts of the player's team
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

        // set the controlled ghost
        controlledGhost = closestGhost;

        foreach (GameObject ghost in ghosts)
        {
            if (ghost == controlledGhost)
            {
                ghost.GetComponent<GhostMotionController>().enabled = true;
                ghost.GetComponent<TeammatesWander>().enabled = false;
            }
            else
            {
                ghost.GetComponent<GhostMotionController>().enabled = false;
                ghost.GetComponent<TeammatesWander>().enabled = true;
            }
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
                // transfer control to the closest ghost
                controlledGhost.GetComponent<GhostMotionController>().enabled = false;
                controlledGhost.GetComponent<TeammatesWander>().enabled = true;

                controlledGhost = closestGhost;

                if (controlledGhost != null)
                {
                    controlledGhost.GetComponent<GhostMotionController>().enabled = true;
                    controlledGhost.GetComponent<TeammatesWander>().enabled = false;
                    SetPlumbob(controlledGhost);
                }
            }
        }
    }

    void SetPlumbob(GameObject ghost)
    {
        if (currentPlumbob == null)
        {
            // instantiate the plumbob if not existing
            currentPlumbob = Instantiate(plumbobPrefab);
        }

        // attach the plumbob to the ghost and position it above
        currentPlumbob.transform.SetParent(ghost.transform);
        currentPlumbob.transform.localPosition = new Vector3(0, 1.5f, 0);
    }
}
