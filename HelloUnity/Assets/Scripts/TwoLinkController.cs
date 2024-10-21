using UnityEngine;

public class TwoLinkController : MonoBehaviour
{
    public Transform target;
    public Transform endEffector;
    public Transform middleJoint;
    public Transform rootJoint;

    void Update()
    {
        if (target == null || endEffector == null || middleJoint == null || rootJoint == null)
        {
            return;
        }

        float distanceToTarget = Vector3.Distance(rootJoint.position, target.position);
        Debug.DrawLine(rootJoint.position, target.position, Color.green);

        float length1 = Vector3.Distance(rootJoint.position, middleJoint.position);
        float length2 = Vector3.Distance(middleJoint.position, endEffector.position);

        if (distanceToTarget > (length1 + length2))
        {
            return;
        }

        Vector3 directionToTarget = (target.position - rootJoint.position).normalized;
        endEffector.position = target.position;

        Debug.DrawLine(rootJoint.position, endEffector.position, Color.red);

        float angleAtRoot = Mathf.Acos((length1 * length1 + distanceToTarget * distanceToTarget - length2 * length2) / (2 * length1 * distanceToTarget)) * Mathf.Rad2Deg;
        Vector3 bendAxis = Vector3.Cross(rootJoint.forward, directionToTarget).normalized;
        Quaternion rotationAtRoot = Quaternion.AngleAxis(angleAtRoot, bendAxis);

        rootJoint.rotation = rotationAtRoot * rootJoint.rotation;
        middleJoint.position = rootJoint.position + (directionToTarget * length1);

        Debug.DrawLine(middleJoint.position, endEffector.position, Color.blue);
    }
}
