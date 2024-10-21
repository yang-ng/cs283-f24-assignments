using UnityEngine;

public class HeadLookController : MonoBehaviour
{
    public Transform gazeTarget;
    public Transform headJoint;

    void Update()
    {
        if (gazeTarget == null || headJoint == null)
        {
            return;
        }

        Vector3 directionToGaze = (gazeTarget.position - headJoint.position).normalized;

        Debug.DrawLine(headJoint.position, gazeTarget.position, Color.blue);

        Vector3 currentLookDirection = headJoint.forward;

        Vector3 rotationAxis = Vector3.Cross(currentLookDirection, directionToGaze);

        float rotationAngle = Vector3.Angle(currentLookDirection, directionToGaze);

        Quaternion computedRotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);

        if (headJoint.parent != null)
        {
            headJoint.parent.rotation = computedRotation * headJoint.parent.rotation;
        }
    }
}