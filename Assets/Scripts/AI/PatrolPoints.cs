using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PatrolPoints : MonoBehaviour
{
    [SerializeField] Color patrolPointColor = Color.green;
    [SerializeField] float sphereRadius = 0.15f;
    [SerializeField] Color patrolLineColor = Color.red;
    [SerializeField] float lineThickness = 0.1f;
    [SerializeField] bool showAlways = false;
    [SerializeField] bool loop = true;

    private int currentPatrolIndex = 0;

    private void OnDrawGizmosSelected()
    {
        if (!showAlways)
        {
            RenderVisuals();
        }
    }

    private void OnDrawGizmos()
    {
        if (showAlways)
        {
            RenderVisuals();
        }
    }

    private void RenderVisuals()
    {
        foreach (Transform child in transform)
        {
            Gizmos.color = patrolPointColor;
            Gizmos.DrawWireSphere(child.position, sphereRadius);
            Gizmos.color = patrolLineColor;
#if UNITY_EDITOR
            if (child.GetSiblingIndex() < transform.childCount - 1)
            {
                Handles.color = patrolLineColor;
                Handles.DrawAAPolyLine(lineThickness, new Vector3[] { child.position, transform.GetChild(child.GetSiblingIndex() + 1).position });
            }
            else if (loop)
            {
                Handles.color = patrolLineColor;
                Handles.DrawAAPolyLine(lineThickness, new Vector3[] { child.position, transform.GetChild(0).position });
#endif
            }
        }
    }

    public void GetNextPatrolPoint()
    {
        if (transform.childCount == 0) return;
        currentPatrolIndex = (currentPatrolIndex + 1) % transform.childCount;
    }

    public Transform GetCurrentPatrolPoint()
    {
        if (transform.childCount == 0) return null;
        return transform.GetChild(currentPatrolIndex);
    }
}