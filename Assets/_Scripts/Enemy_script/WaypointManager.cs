using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    private Transform[] waypoints;

    void Awake()
    {
        // Tự động tìm tất cả GameObject có tag "WayPoints"
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("WayPoints");
        waypoints = new Transform[waypointObjects.Length];
       
        // Sắp xếp waypoint theo tên (ví dụ: Waypoint1, Waypoint2, ...)
        System.Array.Sort(waypointObjects, (a, b) => a.name.CompareTo(b.name));

        for (int i = 0; i < waypointObjects.Length; i++)
        {
            waypoints[i] = waypointObjects[i].transform;
        }
        
    }

    public Transform[] GetWaypoints()
    {
        return waypoints;
    }



    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        Gizmos.color = Color.yellow;
        for (int i = 1; i < waypoints.Length; i++)
        {
            if (waypoints[i - 1] != null && waypoints[i] != null)
            {
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
            }
        }
    }
}
