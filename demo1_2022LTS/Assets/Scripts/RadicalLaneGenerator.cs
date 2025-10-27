using UnityEngine;

public class RadialLaneGenerator : MonoBehaviour
{
    // Assign the six vertex GameObjects here in the Inspector
    public Transform[] vertices = new Transform[6];
    
    // Assign the Line Renderer Prefab here
    public GameObject lanePrefab;
    
    // How far the lines should extend (defines the boundary/edge)
    public float lineExtensionDistance = 20f; 

    void Start()
    {
        GenerateLanes();
    }

    void GenerateLanes()
    {
        Vector3 center = transform.position;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i] == null) continue;

            // 1. Instantiate the line object
            GameObject laneObject = Instantiate(lanePrefab, transform);
            LineRenderer lineRenderer = laneObject.GetComponent<LineRenderer>();

            // 2. Get the direction vector from the center to the vertex
            Vector3 vertexPosition = vertices[i].position;
            Vector3 direction = (vertexPosition - center).normalized;

            // 3. Calculate the Start and End points
            // Start Point: The center
            Vector3 startPoint = center;
            
            // End Point: The center + direction * extension distance
            Vector3 endPoint = center + direction * lineExtensionDistance;

            // 4. Apply the positions to the Line Renderer
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);
            
            // Optional: Name the lanes for organization
            laneObject.name = "Lane " + (i + 1);
        }
    }
}
