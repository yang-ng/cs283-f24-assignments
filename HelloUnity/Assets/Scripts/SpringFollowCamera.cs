using UnityEngine;

public class SpringFollowCamera : MonoBehaviour
{
    public Transform target;
    public float hDist = 5f;
    public float vDist = 2f;
    public float springConstant = 50f;
    public float dampConstant = 10f;

    private Vector3 velocity = Vector3.zero;
    private GameObject lastObstructedObject = null;
    private Material originalMaterial;

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetCameraPosition();
            return;
        }

        Vector3 targetPos = target.position;
        Vector3 targetForward = target.forward;
        Vector3 targetUp = target.up;

        Vector3 idealPosition = targetPos - targetForward * hDist + targetUp * vDist;
        Vector3 displacement = transform.position - idealPosition;
        Vector3 springAcceleration = (-springConstant * displacement) - (dampConstant * velocity);

        velocity += springAcceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        Vector3 cameraForward = targetPos - transform.position;
        transform.rotation = Quaternion.LookRotation(cameraForward);

        HandleObstructions(targetPos);
    }

    void ResetCameraPosition()
    {
        Vector3 targetPos = target.position;
        Vector3 targetForward = target.forward;
        Vector3 targetUp = target.up;

        Vector3 resetPosition = targetPos - targetForward * hDist + targetUp * vDist;
        transform.position = resetPosition;

        Vector3 cameraForward = targetPos - resetPosition;
        transform.rotation = Quaternion.LookRotation(cameraForward);

        velocity = Vector3.zero;
    }

    void HandleObstructions(Vector3 targetPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetPos - transform.position, out hit))
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
