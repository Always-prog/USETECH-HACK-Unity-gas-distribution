using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoxedConvexCollider : MonoBehaviour
{
    [Tooltip("The scale of the boxes used for calculating. The smaller the value the more accurate.")][Range(0.0001f, 65536f)]public float colliderPrecision = 1f;
    [Tooltip("The maximum time that each merge takes.")] [Range(0.01f, 100f)] public float mergePrecision = 0.01f;
    [Tooltip("The minimum size that meshes to be generated.")] [Range(1, 65536)] public int colliderMinSize = 1;
    [HideInInspector] public int colliderMeshCount;
    [HideInInspector] public float colliderMeshPrecision;
    [HideInInspector] public int colliderMeshMinSize;
    [HideInInspector] public bool colliderIsTrigger;
    [HideInInspector] public bool _colliderIsTrigger;
    [HideInInspector] public bool colliderEnableCollider;
    [HideInInspector] public bool _colliderEnableCollider;
    [HideInInspector] public bool colliderShowCollider;

    [HideInInspector] public bool colliderCalculated = false;

    private bool colliderBreak;

    public void CalculateCollider()
    {
        DateTime startTime = DateTime.Now;
        colliderBreak = false;
        RemoveColliders();
        GameObject rbObj = new GameObject();
        Rigidbody objRb = new Rigidbody();
        bool hasRb = false;
        if (GetComponent<Rigidbody>() != null)
        {
            hasRb = true;
            rbObj.AddComponent<Rigidbody>();
            objRb = rbObj.GetComponent<Rigidbody>();
            objRb.mass = GetComponent<Rigidbody>().mass;
            objRb.drag = GetComponent<Rigidbody>().drag;
            objRb.angularDrag = GetComponent<Rigidbody>().angularDrag;
            objRb.useGravity = GetComponent<Rigidbody>().useGravity;
            objRb.isKinematic = GetComponent<Rigidbody>().isKinematic;
            objRb.interpolation = GetComponent<Rigidbody>().interpolation;
            objRb.collisionDetectionMode = GetComponent<Rigidbody>().collisionDetectionMode;
            objRb.constraints = GetComponent<Rigidbody>().constraints;
            DestroyImmediate(GetComponent<Rigidbody>());
        }
        Vector3 objOriPos = transform.position;
        Quaternion objOriRot = transform.rotation;
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        gameObject.AddComponent<MeshCollider>();
        GameObject cubeCollider = Instantiate(Resources.Load<GameObject>("CubeCollider"));
        cubeCollider.name = "CubeCollider";
        cubeCollider.GetComponent<BoxCollider>().size = new Vector3(colliderPrecision, colliderPrecision, colliderPrecision);
        Collider objCol = GetComponent<MeshCollider>();
        float objLength = GetComponent<MeshRenderer>().bounds.size.x;
        float objHeight = GetComponent<MeshRenderer>().bounds.size.y;
        float objWidth = GetComponent<MeshRenderer>().bounds.size.z;
        int objXLength = 0;
        int objYLength = 0;
        int objZLength = 0;
        int cubeCountTotal = 0;
        List<List<List<Vector3>>> positionPoint = new List<List<List<Vector3>>>();
        List<List<List<bool>>> colliderArray = new List<List<List<bool>>>();
        List<ColliderPosition> colliderPositionArray = new List<ColliderPosition>();
        bool finish = false;
        int cubeOverlapCurrent = 0;
        DateTime nextDelay = DateTime.Now;
        for (float x = GetComponent<MeshRenderer>().bounds.center.x - objLength / 2; x < GetComponent<MeshRenderer>().bounds.center.x + objLength / 2; x += colliderPrecision)
        {
            if (DateTime.Now > nextDelay)
            {
                nextDelay = DateTime.Now.AddSeconds(1);
                if (EditorUtility.DisplayCancelableProgressBar("Calculating...", string.Format("Calculating Colliders {0}/{1}", cubeOverlapCurrent, cubeCountTotal), cubeOverlapCurrent / (float)cubeCountTotal * 0.2f))
                {
                    colliderBreak = true;
                    finish = true;
                    break;
                }
            }
            objYLength = 0;
            List<List<Vector3>> yPointTemp = new List<List<Vector3>>();
            List<List<bool>> yTemp = new List<List<bool>>();
            for (float y = GetComponent<MeshRenderer>().bounds.center.y - objHeight / 2; y < GetComponent<MeshRenderer>().bounds.center.y + objHeight / 2; y += colliderPrecision)
            {
                objZLength = 0;
                List<Vector3> zPointTemp = new List<Vector3>();
                List<bool> zTemp = new List<bool>();
                for (float z = GetComponent<MeshRenderer>().bounds.center.z - objWidth / 2; z < GetComponent<MeshRenderer>().bounds.center.z + objWidth / 2; z += colliderPrecision)
                {
                    float xfTemp = x;
                    float yfTemp = y;
                    float zfTemp = z;
                    if(Mathf.Approximately(x, GetComponent<MeshRenderer>().bounds.center.x - objLength / 2))
                    {
                        xfTemp = GetComponent<MeshRenderer>().bounds.center.x - objLength / 2 + colliderPrecision / 2;
                    }
                    if (x + colliderPrecision > GetComponent<MeshRenderer>().bounds.center.x + objLength / 2)
                    {
                        xfTemp = GetComponent<MeshRenderer>().bounds.center.x + objLength / 2 - colliderPrecision / 2;
                    }
                    if (Mathf.Approximately(y, GetComponent<MeshRenderer>().bounds.center.y - objHeight / 2))
                    {
                        yfTemp = GetComponent<MeshRenderer>().bounds.center.y - objHeight / 2 + colliderPrecision / 2;
                    }
                    if (y + colliderPrecision > GetComponent<MeshRenderer>().bounds.center.y + objHeight / 2)
                    {
                        yfTemp = GetComponent<MeshRenderer>().bounds.center.y + objHeight / 2 - colliderPrecision / 2;
                    }
                    if (Mathf.Approximately(z, GetComponent<MeshRenderer>().bounds.center.z - objWidth / 2))
                    {
                        zfTemp = GetComponent<MeshRenderer>().bounds.center.z - objWidth / 2 + colliderPrecision / 2;
                    }
                    if (z + colliderPrecision > GetComponent<MeshRenderer>().bounds.center.z + objWidth / 2)
                    {
                        zfTemp = GetComponent<MeshRenderer>().bounds.center.z + objWidth / 2 - colliderPrecision / 2;
                    }
                    zPointTemp.Add(new Vector3(xfTemp, yfTemp, zfTemp));
                    zTemp.Add(true);
                    objZLength++;
                    cubeCountTotal++;
                }
                yPointTemp.Add(zPointTemp);
                yTemp.Add(zTemp);
                objYLength++;
            }
            positionPoint.Add(yPointTemp);
            colliderArray.Add(yTemp);
            objXLength++;
        }

        for (int x = 0; x < objXLength; x++)
        {
            for (int y = 0; y < objYLength; y++)
            {
                for (int z = 0; z < objZLength; z++)
                {
                    Collider[] cols = Physics.OverlapBox(new Vector3(GetComponent<MeshRenderer>().bounds.center.x - objLength / 2 + x * colliderPrecision, GetComponent<MeshRenderer>().bounds.center.y - objHeight / 2 + y * colliderPrecision, GetComponent<MeshRenderer>().bounds.center.z - objWidth / 2 + z * colliderPrecision), new Vector3(colliderPrecision, colliderPrecision, colliderPrecision) / 2);
                    bool hitObj = false;
                    foreach (Collider col in cols)
                    {
                        if (col.gameObject == gameObject)
                        {
                            hitObj = true;
                            break;
                        }
                    }
                    if (hitObj)
                    {
                        colliderArray[x][y][z] = true;
                        ColliderPosition positionTemp = new ColliderPosition
                        {
                            x = x,
                            y = y,
                            z = z
                        };
                        colliderPositionArray.Add(positionTemp);
                    }
                    else
                    {
                        colliderArray[x][y][z] = false;
                    }
                    cubeOverlapCurrent++;
                    if (DateTime.Now > nextDelay)
                    {
                        nextDelay = DateTime.Now.AddSeconds(1);
                        if (EditorUtility.DisplayCancelableProgressBar("Calculating...", string.Format("Calculating Colliders {0}/{1}", cubeOverlapCurrent, cubeCountTotal), cubeOverlapCurrent / (float)cubeCountTotal * 0.2f))
                        {
                            colliderBreak = true;
                            finish = true;
                            break;
                        }
                    }
                }
                if (finish)
                {
                    break;
                }
            }
            if (finish)
            {
                break;
            }
        }
        List<List<List<bool>>> colliderArrayRecord = new List<List<List<bool>>>(colliderArray);
        int meshCount = 0;
        int colliderMax = 0;
        int lastColliderCombo = 0;
        DateTime nextMergeSkip;
        nextDelay = DateTime.Now;
        while (!finish)
        {
            List<ColliderPosition> colliderCombo = new List<ColliderPosition>();
            List<ColliderPosition> colliderComboTemp = new List<ColliderPosition>();
            ColliderPosition colliderPos = new ColliderPosition();
            if (colliderMax < colliderPositionArray.Count)
            {
                colliderMax = colliderPositionArray.Count;
                lastColliderCombo = colliderPositionArray.Count;
            }
            nextMergeSkip = DateTime.Now.AddSeconds(mergePrecision);
            bool haveCollider = true;
            for (int c = 0; c < colliderPositionArray.Count; c++)
            {
                if (finish || DateTime.Now > nextMergeSkip && mergePrecision != 100)
                {
                    break;
                }
                if (DateTime.Now > nextDelay)
                {
                    nextDelay = DateTime.Now.AddSeconds(1);
                    if (EditorUtility.DisplayCancelableProgressBar("Calculating...", string.Format("Merging Colliders {0}_{1}/{2}", colliderCombo.Count, c, colliderPositionArray.Count), 1 - colliderPositionArray.Count / (float)colliderMax * 0.8f))
                    {
                        colliderBreak = true;
                        finish = true;
                        break;
                    }
                }
                for (int xx = colliderPositionArray[c].x; xx < objXLength; xx++)
                {
                    if (colliderArrayRecord[xx][colliderPositionArray[c].y][colliderPositionArray[c].z])
                    {
                        colliderComboTemp = new List<ColliderPosition>();
                        for (int i = colliderPositionArray[c].x; i <= xx; i++)
                        {
                            colliderPos = new ColliderPosition
                            {
                                x = i,
                                y = colliderPositionArray[c].y,
                                z = colliderPositionArray[c].z
                            };
                            colliderComboTemp.Add(colliderPos);
                        }
                        if (colliderComboTemp.Count > colliderCombo.Count)
                        {
                            colliderCombo = new List<ColliderPosition>(colliderComboTemp);
                            nextMergeSkip = DateTime.Now.AddSeconds(mergePrecision);
                        }
                        for (int yy = colliderPositionArray[c].y; yy < objYLength; yy++)
                        {
                            haveCollider = true;
                            for (int xxx = colliderPositionArray[c].x; xxx <= xx; xxx++)
                            {
                                for (int yyy = colliderPositionArray[c].y; yyy <= yy; yyy++)
                                {
                                    if (!colliderArrayRecord[xxx][yyy][colliderPositionArray[c].z])
                                    {
                                        haveCollider = false;
                                        break;
                                    }
                                }
                                if (!haveCollider)
                                {
                                    break;
                                }
                            }
                            if (haveCollider)
                            {
                                colliderComboTemp = new List<ColliderPosition>();
                                for (int i = colliderPositionArray[c].x; i <= xx; i++)
                                {
                                    for (int o = colliderPositionArray[c].y; o <= yy; o++)
                                    {
                                        colliderPos = new ColliderPosition
                                        {
                                            x = i,
                                            y = o,
                                            z = colliderPositionArray[c].z
                                        };
                                        colliderComboTemp.Add(colliderPos);
                                    }
                                }
                                if (colliderComboTemp.Count > colliderCombo.Count)
                                {
                                    colliderCombo = new List<ColliderPosition>(colliderComboTemp);
                                    nextMergeSkip = DateTime.Now.AddSeconds(mergePrecision);
                                }
                                for (int zz = colliderPositionArray[c].z; zz < objZLength; zz++)
                                {
                                    haveCollider = true;
                                    for (int xxx = colliderPositionArray[c].x; xxx <= xx; xxx++)
                                    {
                                        for (int yyy = colliderPositionArray[c].y; yyy <= yy; yyy++)
                                        {
                                            for (int zzz = colliderPositionArray[c].z; zzz <= zz; zzz++)
                                            {
                                                if (!colliderArrayRecord[xxx][yyy][zzz])
                                                {
                                                    haveCollider = false;
                                                    break;
                                                }
                                            }
                                            if (!haveCollider)
                                            {
                                                break;
                                            }
                                        }
                                        if (!haveCollider)
                                        {
                                            break;
                                        }
                                    }
                                    if (haveCollider)
                                    {
                                        colliderComboTemp = new List<ColliderPosition>();
                                        for (int i = colliderPositionArray[c].x; i <= xx; i++)
                                        {
                                            for (int o = colliderPositionArray[c].y; o <= yy; o++)
                                            {
                                                for (int p = colliderPositionArray[c].z; p <= zz; p++)
                                                {
                                                    colliderPos = new ColliderPosition
                                                    {
                                                        x = i,
                                                        y = o,
                                                        z = p
                                                    };
                                                    colliderComboTemp.Add(colliderPos);
                                                }
                                            }
                                        }
                                        if (colliderComboTemp.Count > colliderCombo.Count)
                                        {
                                            colliderCombo = new List<ColliderPosition>(colliderComboTemp);
                                            nextMergeSkip = DateTime.Now.AddSeconds(mergePrecision);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (colliderCombo.Count > colliderPositionArray.Count - c || colliderCombo.Count >= lastColliderCombo)
                {
                    break;
                }
            }
            if (finish)
            {
                break;
            }
            lastColliderCombo = colliderCombo.Count;
            if(colliderCombo.Count >= colliderMinSize)
            {
                List<GameObject> meshes = new List<GameObject>();
                for (int i = 0; i < colliderCombo.Count; i++)
                {
                    GameObject g = new GameObject();
                    g.name = "Collider_" + i;
                    g.transform.SetParent(transform);
                    g.transform.position = positionPoint[colliderCombo[i].x][colliderCombo[i].y][colliderCombo[i].z];
                    g.transform.localScale = new Vector3(colliderPrecision, colliderPrecision, colliderPrecision);
                    g.AddComponent<MeshFilter>();
                    g.GetComponent<MeshFilter>().sharedMesh = cubeCollider.GetComponent<MeshFilter>().sharedMesh;
                    g.AddComponent<MeshCollider>();
                    g.GetComponent<MeshCollider>().sharedMesh = cubeCollider.GetComponent<MeshCollider>().sharedMesh;
                    g.GetComponent<MeshCollider>().convex = true;
                    meshes.Add(g);
                    colliderArrayRecord[colliderCombo[i].x][colliderCombo[i].y][colliderCombo[i].z] = false;
                    colliderPositionArray.RemoveAt(colliderPositionArray.FindIndex(c => c.x == colliderCombo[i].x && c.y == colliderCombo[i].y && c.z == colliderCombo[i].z));
                }
                CombineInstance[] combineMeshes = new CombineInstance[meshes.Count];
                for (int i = 0; i < meshes.Count; i++)
                {
                    combineMeshes[i].mesh = meshes[i].GetComponent<MeshFilter>().sharedMesh;
                    Matrix4x4 m = meshes[i].GetComponent<MeshFilter>().transform.localToWorldMatrix;
                    m.m00 /= transform.lossyScale.x;
                    m.m03 /= transform.lossyScale.x;
                    m.m11 /= transform.lossyScale.y;
                    m.m13 /= transform.lossyScale.y;
                    m.m22 /= transform.lossyScale.z;
                    m.m23 /= transform.lossyScale.z;
                    combineMeshes[i].transform = m;
                }
                Mesh newMesh = new Mesh();
                newMesh.name = "Mesh_" + meshCount + "_" + colliderCombo.Count;
                newMesh.CombineMeshes(combineMeshes);
                MeshCollider boxCol = gameObject.AddComponent<MeshCollider>();
                boxCol.sharedMesh = newMesh;
                boxCol.convex = true;
                boxCol.hideFlags = HideFlags.HideInInspector;
                for (int i = meshes.Count - 1; i >= 0; i--)
                {
                    DestroyImmediate(meshes[i]);
                }
                meshCount++;
            }
            else
            {
                for (int i = 0; i < colliderCombo.Count; i++)
                {
                    colliderArrayRecord[colliderCombo[i].x][colliderCombo[i].y][colliderCombo[i].z] = false;
                    colliderPositionArray.RemoveAt(colliderPositionArray.FindIndex(c => c.x == colliderCombo[i].x && c.y == colliderCombo[i].y && c.z == colliderCombo[i].z));
                }
            }
            finish = true;
            for (int x = 0; x < objXLength; x++)
            {
                for (int y = 0; y < objYLength; y++)
                {
                    for (int z = 0; z < objZLength; z++)
                    {
                        if (colliderArrayRecord[x][y][z])
                        {
                            finish = false;
                            break;
                        }
                    }
                    if (!finish)
                    {
                        break;
                    }
                }
                if (!finish)
                {
                    break;
                }
            }
        }
        EditorUtility.ClearProgressBar();
        transform.position = objOriPos;
        transform.rotation = objOriRot;
        DestroyImmediate(objCol);
        DestroyImmediate(cubeCollider);
        if (hasRb)
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<Rigidbody>().mass = objRb.mass;
            gameObject.GetComponent<Rigidbody>().drag = objRb.drag;
            gameObject.GetComponent<Rigidbody>().angularDrag = objRb.angularDrag;
            gameObject.GetComponent<Rigidbody>().useGravity = objRb.useGravity;
            gameObject.GetComponent<Rigidbody>().isKinematic = objRb.isKinematic;
            gameObject.GetComponent<Rigidbody>().interpolation = objRb.interpolation;
            gameObject.GetComponent<Rigidbody>().collisionDetectionMode = objRb.collisionDetectionMode;
            gameObject.GetComponent<Rigidbody>().constraints = objRb.constraints;
        }
        DestroyImmediate(rbObj);
        colliderMeshCount = meshCount;
        colliderMeshPrecision = colliderPrecision;
        colliderMeshMinSize = colliderMinSize;
        colliderIsTrigger = false;
        _colliderIsTrigger = false;
        colliderEnableCollider = true;
        _colliderEnableCollider = true;
        colliderShowCollider = false;
        if (colliderBreak)
        {
            RemoveColliders();
            colliderCalculated = false;
        }
        else
        {
            colliderCalculated = true;
            Debug.Log(string.Format("Time spent: {0}.{1}.{2}:{3}", Mathf.FloorToInt((float)(DateTime.Now - startTime).TotalHours), (DateTime.Now - startTime).Minutes, (DateTime.Now - startTime).Seconds, (DateTime.Now - startTime).Milliseconds));
        }
    }
    
    public void RemoveColliders()
    {
        colliderCalculated = false;
        foreach (Collider col in GetComponents<Collider>())
        {
            DestroyImmediate(col);
        }
    }

    public void ReloadTrigger()
    {
        foreach (MeshCollider col in GetComponents<MeshCollider>())
        {
            col.isTrigger = colliderIsTrigger;
        }
    }

    public void ReloadEnable()
    {
        foreach (MeshCollider col in GetComponents<MeshCollider>())
        {
            col.enabled = colliderEnableCollider;
        }
    }

    public void ReloadShow()
    {
        foreach (MeshCollider col in GetComponents<MeshCollider>())
        {
            if (colliderShowCollider)
            {
                col.hideFlags = HideFlags.None;
            }
            else
            {
                col.hideFlags = HideFlags.HideInInspector;
            }
        }
    }

    public void PreviewColliderMinSizeChange()
    {
        colliderEnableCollider = true;
        _colliderEnableCollider = true;
        ReloadEnable();
        int meshCount = colliderMeshCount;
        foreach (MeshCollider col in GetComponents<MeshCollider>())
        {
            if (int.Parse(col.sharedMesh.name.Split('_')[2]) < colliderMinSize)
            {
                col.enabled = false;
                meshCount--;
            }
        }
        Debug.Log(string.Format("Collider minimum size: {0}. Collider count: {1}", colliderMinSize, meshCount));

    }

    public void ExecuteColliderMinSizeChange()
    {
        int meshCount = colliderMeshCount;
        foreach (MeshCollider col in GetComponents<MeshCollider>())
        {
            if (int.Parse(col.sharedMesh.name.Split('_')[2]) < colliderMinSize)
            {
                DestroyImmediate(col);
                meshCount--;
            }
        }
        colliderMeshCount = meshCount;
        colliderMeshMinSize = colliderMinSize;
    }

    public class ColliderPosition
    {
        public int x;
        public int y;
        public int z;
    }
}
