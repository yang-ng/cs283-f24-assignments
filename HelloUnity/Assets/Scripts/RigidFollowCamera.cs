using UnityEngine;

public class RigidFollowCamera : MonoBehaviour
{
    public Transform target;
    public float hDist = 5f;
    public float vDist = 3f;

    private GameObject lastObstructedObject = null;
    private Material originalMaterial;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position - target.forward * hDist + target.up * vDist;
        transform.position = desiredPosition;
        transform.LookAt(target);

        HandleObstructions();
    }

    void HandleObstructions()
    {
        Vector3 directionToPlayer = target.position - transform.position;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, directionToPlayer.magnitude))
        {
            if (hit.collider.gameObject != target.gameObject)
            {
                MakeObjectTransparent(hit.collider.gameObject);
            }
        }
        else if (lastObstructedObject != null)
        {
            RestoreObjectTransparency();
        }
    }

    void MakeObjectTransparent(GameObject obj)
    {
        if (lastObstructedObject != null && lastObstructedObject != obj)
        {
            RestoreObjectTransparency();
        }

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (renderer.material != originalMaterial)
            {
                originalMaterial = renderer.material;
                Material transparentMaterial = new Material(originalMaterial);
                transparentMaterial.color = new Color(transparentMaterial.color.r, transparentMaterial.color.g, transparentMaterial.color.b, 0.3f);
                renderer.material = transparentMaterial;

                lastObstructedObject = obj;
            }
        }
    }

    void RestoreObjectTransparency()
    {
        if (lastObstructedObject != null)
        {
            Renderer renderer = lastObstructedObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = originalMaterial;
            }
            lastObstructedObject = null;
        }
    }
}
