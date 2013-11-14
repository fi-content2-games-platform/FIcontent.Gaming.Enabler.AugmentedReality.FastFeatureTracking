using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Path : MonoBehaviour
{
    public Vector3[] path;
    public float radius = 200f;
    public Color color = Color.magenta;

    void Awake()
    {
        path = new Vector3[this.transform.GetChildCount()];
        GetChildPaths();
    }

    private void GetChildPaths()
    {
        int i = 0;
        foreach (Transform t in this.transform)
        {
            path[i++] = t.position;
        }
    }
        
    void OnDrawGizmosSelected()
    {
        if (path == null || path.Length < 2)
            return;

        GetChildPaths();

        Gizmos.color = color;

        Vector3 curr = path[0];
        Gizmos.DrawWireSphere(path[0], radius);

        for (int i = 1; i < this.path.Length; i++)
        {
            Gizmos.DrawLine(curr, path[i]);
            Gizmos.DrawWireSphere(path[i], radius);
            curr = path[i];
        }

        if (path.Length > 2)
            Gizmos.DrawLine(curr, path[0]);

    }
}
