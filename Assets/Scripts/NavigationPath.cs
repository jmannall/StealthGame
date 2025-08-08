using System.IO;
using UnityEngine;

public class NavigationPath : MonoBehaviour
{
    public bool switchDirection = false;

    Vector3[] waypoints;
    private int idx = -1;

    private void Awake()
    {
        waypoints = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            waypoints[i] = transform.GetChild(i).position;
    }

    public Vector3 GetCurrentWaypoint()
    {
        return waypoints[idx];
    }

    public Vector3 GetNextWaypoint()
    {
        if (switchDirection)
        {
            idx--;
            if (idx < 0)
                idx = waypoints.Length - 1;
            return waypoints[idx];
        }
        else
        {
            idx++;
            if (idx >= waypoints.Length)
                idx = 0;
            return waypoints[idx];
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = transform.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(t.position, 0.2f);
            Gizmos.DrawLine(previousPosition, t.position);
            previousPosition = t.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);
    }
}
