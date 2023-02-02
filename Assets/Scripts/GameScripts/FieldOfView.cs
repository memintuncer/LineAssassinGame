using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;



public class FieldOfView : MonoBehaviour
{

    
    [Header("Field of View Settings")]
    [SerializeField] private float viewRadius = 50f;
    [SerializeField, Range(0, 360)] private float viewAngle = 90f;


    [SerializeField] private int meshResolution = 10;
    [SerializeField] private MeshFilter ViewMeshFilter;
    private Mesh viewMesh;
    private List<Vector3> viewPoints = new List<Vector3>(); 
    public bool playerFound;
    EnemyController ParentEnemyController;
    [SerializeField] Material[] FOVMaterials;
    MeshRenderer FovMeshRenderer;
    void Start()
    {
        playerFound = false;
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        ViewMeshFilter.mesh = viewMesh;
        ParentEnemyController = transform.parent.GetComponent<EnemyController>();
        FovMeshRenderer = GetComponent<MeshRenderer>();
        
    }


    void Update()
    {
     
        ViewMeshFilter.mesh = viewMesh;
        DrawFieldOfView();
    }




    void DrawFieldOfView()
    {

      

        viewPoints.Clear();
        FoV oldFoV = new FoV();

        for (int i = 0; i <= Mathf.RoundToInt(viewAngle * meshResolution); i++)
        {
            FoV newFoV = FoVinfo(transform.eulerAngles.y - viewAngle / 2 + (viewAngle / Mathf.RoundToInt(viewAngle * meshResolution)) * i, viewRadius);

            if (i > 0)
            {
                if (oldFoV.hit != newFoV.hit)
                {
                    
                    if (oldFoV.point != Vector3.zero)
                    {
                        viewPoints.Add(oldFoV.point);
                    }
                    if (newFoV.point != Vector3.zero)
                    {
                        viewPoints.Add(newFoV.point);
                    }
                }
            }

            viewPoints.Add(newFoV.point);
            oldFoV = newFoV;
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }


  
    FoV FoVinfo(float globalAngle, float viewRadius)
    {
        Vector3 direction = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        Physics.autoSyncTransforms = false;
        

        if (Physics.Raycast(transform.position, direction, out hit, viewRadius))
        {
            if (hit.collider.transform.tag == "Player" )
            {
                if (!playerFound)
                {
                    playerFound = true;
                   
                    EventParam new_param = new EventParam();
                    new_param.PlayerTransform = hit.collider.transform;
                    Debug.Log("Hitted" + hit.collider.transform.name);
                    new_param.TargetEnemy = transform.parent.gameObject;
                    EventManager.TriggerEvent(GameConstants.GameEvents.PLAYER_IS_SPOTTED, new_param);
                    FovMeshRenderer.material = FOVMaterials[1];
                    
                    
                }
               
            }
            
            Physics.autoSyncTransforms = true;
            return new FoV(true, hit.point, hit.distance, globalAngle);

        }

        
        
        else
        {
            Physics.autoSyncTransforms = true;
            return new FoV(false, transform.position + direction * viewRadius, viewRadius, globalAngle);
        }

    }


    public Vector3 DirFromAngle(float angleInDegrees, bool IsAngleGlobal)
    {
        if (!IsAngleGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }


 
}



public struct FoV
{
    public bool hit; 
    public Vector3 point; 
    public float distance; 
    public float angle;

    public FoV(bool hit, Vector3 point, float distance, float angle)
    {
        this.hit = hit;
        this.point = point;
        this.distance = distance;
        this.angle = angle;
    }
}




