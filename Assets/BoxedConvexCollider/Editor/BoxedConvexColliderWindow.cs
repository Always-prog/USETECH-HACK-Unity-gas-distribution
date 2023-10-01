using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoxedConvexColliderWindow : EditorWindow
{
    [SerializeField] GameObject targetObj = _targetObj;
    public static GameObject _targetObj;
    [SerializeField] bool objTrigger = _objTrigger;
    public static bool _objTrigger;
    [SerializeField] float objColliderPrecision = _objColliderPrecision;
    public static float _objColliderPrecision;
    [SerializeField] float objMergePrecision = _objMergePrecision;
    public static float _objMergePrecision;
    [SerializeField] int objMinSize = _objMinSize;
    public static int _objMinSize;
    [SerializeField] bool objRewrite = _objRewrite;
    public static bool _objRewrite;
    [SerializeField] int totalObjects = _totalObjects;
    public static int _totalObjects;
    [SerializeField] int currentObject = _currentObject;
    public static int _currentObject;
    [SerializeField] bool executeBreak = _executeBreak;
    public static bool _executeBreak;
    Vector2 sv;

    [MenuItem("Window/Boxed Convex Collider")]
    static void Init()
    {
        BoxedConvexColliderWindow window = (BoxedConvexColliderWindow)GetWindow(typeof(BoxedConvexColliderWindow), true, "Boxed Convex Collider Window");
        window.Show();
    }

    void OnGUI()
    {
        sv = EditorGUILayout.BeginScrollView(sv);
        targetObj = EditorGUILayout.ObjectField("Target Object:", targetObj, typeof(GameObject), true) as GameObject;
        _targetObj = targetObj;
        objTrigger = EditorGUILayout.Toggle("Trigger", objTrigger);
        _objTrigger = objTrigger;
        if(objColliderPrecision == 0)
        {
            objColliderPrecision = 1;
            _objColliderPrecision = objColliderPrecision;
        }
        if (objMergePrecision == 0)
        {
            objMergePrecision = 0.01f;
            _objMergePrecision = objMergePrecision;
        }
        if (objMinSize == 0)
        {
            objMinSize = 1;
            _objMinSize = objMinSize;
        }
        objColliderPrecision = EditorGUILayout.Slider("Collider Precision", objColliderPrecision, 0.0001f, 65536f);
        _objColliderPrecision = objColliderPrecision;
        objMergePrecision = EditorGUILayout.Slider("Merge Precision", objMergePrecision, 0.01f, 100f);
        _objMergePrecision = objMergePrecision;
        objMinSize = EditorGUILayout.IntSlider("Collider Min Size", objMinSize, 1, 65536);
        _objMinSize = objMinSize;
        objRewrite = EditorGUILayout.Toggle("Overwrite Colliders", objRewrite);
        _objRewrite = objRewrite;
        if(GUILayout.Button("Calculate Colliders") && targetObj != null)
        {
            DateTime startTime = DateTime.Now;
            totalObjects = 0;
            _totalObjects = totalObjects;
            currentObject = 0;
            _currentObject = currentObject;
            executeBreak = false;
            _executeBreak = executeBreak;
            CountObjects(targetObj);
            AddColliders(targetObj);
            EditorUtility.ClearProgressBar();
            if (!executeBreak)
            {
                Debug.Log(string.Format("Time spent: {0}.{1}.{2}:{3}", Mathf.FloorToInt((float)(DateTime.Now - startTime).TotalHours), (DateTime.Now - startTime).Minutes, (DateTime.Now - startTime).Seconds, (DateTime.Now - startTime).Milliseconds));
            }
        }
        if(GUILayout.Button("Remove Colliders") && targetObj != null)
        {
            RemoveColliders(targetObj);
        }
        EditorGUILayout.EndScrollView();
    }

    void CountObjects(GameObject obj)
    {
        if (obj.GetComponent<MeshFilter>() != null)
        {
            if(objRewrite || obj.GetComponents<Collider>().Length == 0)
            {
                totalObjects++;
                _totalObjects = totalObjects;
            }
        }
        foreach (Transform child in obj.transform)
        {
            CountObjects(child.gameObject);
        }
    }

    void AddColliders(GameObject obj)
    {
        if(obj.GetComponent<MeshFilter>() != null && (objRewrite || obj.GetComponents<Collider>().Length == 0))
        {
            currentObject++;
            _currentObject = currentObject;
            RemoveCollider(obj);
            if(obj.GetComponent<BoxedConvexCollider>() == null)
            {
                obj.AddComponent<BoxedConvexCollider>();
            }
            GameObject rbObj = new GameObject();
            Rigidbody objRb = new Rigidbody();
            bool hasRb = false;
            if (obj.GetComponent<Rigidbody>() != null)
            {
                hasRb = true;
                rbObj.AddComponent<Rigidbody>();
                objRb = rbObj.GetComponent<Rigidbody>();
                objRb.mass = obj.GetComponent<Rigidbody>().mass;
                objRb.drag = obj.GetComponent<Rigidbody>().drag;
                objRb.angularDrag = obj.GetComponent<Rigidbody>().angularDrag;
                objRb.useGravity = obj.GetComponent<Rigidbody>().useGravity;
                objRb.isKinematic = obj.GetComponent<Rigidbody>().isKinematic;
                objRb.interpolation = obj.GetComponent<Rigidbody>().interpolation;
                objRb.collisionDetectionMode = obj.GetComponent<Rigidbody>().collisionDetectionMode;
                objRb.constraints = obj.GetComponent<Rigidbody>().constraints;
                DestroyImmediate(obj.GetComponent<Rigidbody>());
            }
            Vector3 objOriPos = obj.transform.position;
            Quaternion objOriRot = obj.transform.rotation;
            obj.transform.position = new Vector3(0, 0, 0);
            obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            obj.AddComponent<MeshCollider>();
            GameObject cubeCollider = Instantiate(Resources.Load<GameObject>("CubeCollider"));
            cubeCollider.name = "CubeCollider";
            cubeCollider.GetComponent<BoxCollider>().size = new Vector3(objColliderPrecision, objColliderPrecision, objColliderPrecision);
            Collider objCol = obj.GetComponent<MeshCollider>();
            float objLength = obj.GetComponent<MeshRenderer>().bounds.size.x;
            float objHeight = obj.GetComponent<MeshRenderer>().bounds.size.y;
            float objWidth = obj.GetComponent<MeshRenderer>().bounds.size.z;
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
            for (float x = obj.GetComponent<MeshRenderer>().bounds.center.x - objLength / 2; x < obj.GetComponent<MeshRenderer>().bounds.center.x + objLength / 2; x += objColliderPrecision)
            {
                if (DateTime.Now > nextDelay)
                {
                    nextDelay = DateTime.Now.AddSeconds(1);
                    if (EditorUtility.DisplayCancelableProgressBar(string.Format("Calculating Item {0}/{1}", currentObject, totalObjects), string.Format("Calculating Colliders {0}/{1}", cubeOverlapCurrent, cubeCountTotal), cubeOverlapCurrent / (float)cubeCountTotal * 0.2f))
                    {
                        executeBreak = true;
                        _executeBreak = executeBreak;
                        finish = true;
                        break;
                    }
                }
                objYLength = 0;
                List<List<Vector3>> yPointTemp = new List<List<Vector3>>();
                List<List<bool>> yTemp = new List<List<bool>>();
                for (float y = obj.GetComponent<MeshRenderer>().bounds.center.y - objHeight / 2; y < obj.GetComponent<MeshRenderer>().bounds.center.y + objHeight / 2; y += objColliderPrecision)
                {
                    objZLength = 0;
                    List<Vector3> zPointTemp = new List<Vector3>();
                    List<bool> zTemp = new List<bool>();
                    for (float z = obj.GetComponent<MeshRenderer>().bounds.center.z - objWidth / 2; z < obj.GetComponent<MeshRenderer>().bounds.center.z + objWidth / 2 ; z += objColliderPrecision)
                    {
                        float xfTemp = x;
                        float yfTemp = y;
                        float zfTemp = z;
                        if (Mathf.Approximately(x, obj.GetComponent<MeshRenderer>().bounds.center.x - objLength / 2))
                        {
                            xfTemp = obj.GetComponent<MeshRenderer>().bounds.center.x - objLength / 2 + objColliderPrecision / 2;
                        }
                        if (x + objColliderPrecision > obj.GetComponent<MeshRenderer>().bounds.center.x + objLength / 2)
                        {
                            xfTemp = obj.GetComponent<MeshRenderer>().bounds.center.x + objLength / 2 - objColliderPrecision / 2;
                        }
                        if (Mathf.Approximately(y, obj.GetComponent<MeshRenderer>().bounds.center.y - objHeight / 2))
                        {
                            yfTemp = obj.GetComponent<MeshRenderer>().bounds.center.y - objHeight / 2 + objColliderPrecision / 2;
                        }
                        if (y + objColliderPrecision > obj.GetComponent<MeshRenderer>().bounds.center.y + objHeight / 2)
                        {
                            yfTemp = obj.GetComponent<MeshRenderer>().bounds.center.y + objHeight / 2 - objColliderPrecision / 2;
                        }
                        if (Mathf.Approximately(z, obj.GetComponent<MeshRenderer>().bounds.center.z - objWidth / 2))
                        {
                            zfTemp = obj.GetComponent<MeshRenderer>().bounds.center.z - objWidth / 2 + objColliderPrecision / 2;
                        }
                        if (z + objColliderPrecision > obj.GetComponent<MeshRenderer>().bounds.center.z + objWidth / 2)
                        {
                            zfTemp = obj.GetComponent<MeshRenderer>().bounds.center.z + objWidth / 2 - objColliderPrecision / 2;
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
                        Collider[] cols = Physics.OverlapBox(new Vector3(obj.GetComponent<MeshRenderer>().bounds.center.x - objLength / 2 + x * objColliderPrecision, obj.GetComponent<MeshRenderer>().bounds.center.y - objHeight / 2 + y * objColliderPrecision, obj.GetComponent<MeshRenderer>().bounds.center.z - objWidth / 2 + z * objColliderPrecision), new Vector3(objColliderPrecision, objColliderPrecision, objColliderPrecision) / 2);
                        bool hitObj = false;
                        foreach (Collider col in cols)
                        {
                            if (col.gameObject == obj)
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
                            if (EditorUtility.DisplayCancelableProgressBar(string.Format("Calculating Item {0}/{1}", currentObject, totalObjects), string.Format("Calculating Colliders {0}/{1}", cubeOverlapCurrent, cubeCountTotal), cubeOverlapCurrent / (float)cubeCountTotal * 0.2f))
                            {
                                executeBreak = true;
                                _executeBreak = executeBreak;
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
                nextMergeSkip = DateTime.Now.AddSeconds(objMergePrecision);
                bool haveCollider = true;
                for (int c = 0; c < colliderPositionArray.Count; c++)
                {
                    if (finish || DateTime.Now > nextMergeSkip && objMergePrecision != 100)
                    {
                        break;
                    }
                    if (DateTime.Now > nextDelay)
                    {
                        nextDelay = DateTime.Now.AddSeconds(1);
                        if (EditorUtility.DisplayCancelableProgressBar(string.Format("Calculating Item {0}/{1}", currentObject, totalObjects), string.Format("Merging Colliders {0}_{1}/{2}", colliderCombo.Count, c, colliderPositionArray.Count), 1 - colliderPositionArray.Count / (float)colliderMax * 0.8f))
                        {
                            executeBreak = true;
                            _executeBreak = executeBreak;
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
                                nextMergeSkip = DateTime.Now.AddSeconds(objMergePrecision);
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
                                        nextMergeSkip = DateTime.Now.AddSeconds(objMergePrecision);
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
                                                nextMergeSkip = DateTime.Now.AddSeconds(objMergePrecision);
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
                if (colliderCombo.Count >= objMinSize)
                {
                    List<GameObject> meshes = new List<GameObject>();
                    for (int i = 0; i < colliderCombo.Count; i++)
                    {
                        GameObject g = new GameObject();
                        g.name = "Collider_" + i;
                        g.transform.SetParent(obj.transform);
                        g.transform.position = positionPoint[colliderCombo[i].x][colliderCombo[i].y][colliderCombo[i].z];
                        g.transform.localScale = new Vector3(objColliderPrecision, objColliderPrecision, objColliderPrecision);
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
                        m.m00 /= obj.transform.lossyScale.x;
                        m.m03 /= obj.transform.lossyScale.x;
                        m.m11 /= obj.transform.lossyScale.y;
                        m.m13 /= obj.transform.lossyScale.y;
                        m.m22 /= obj.transform.lossyScale.z;
                        m.m23 /= obj.transform.lossyScale.z;
                        combineMeshes[i].transform = m;
                    }
                    Mesh newMesh = new Mesh();
                    newMesh.name = "Mesh_" + meshCount + "_" + colliderCombo.Count;
                    newMesh.CombineMeshes(combineMeshes);
                    MeshCollider boxCol = obj.AddComponent<MeshCollider>();
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
            obj.transform.position = objOriPos;
            obj.transform.rotation = objOriRot;
            DestroyImmediate(objCol);
            DestroyImmediate(cubeCollider);
            if (hasRb)
            {
                obj.AddComponent<Rigidbody>();
                obj.GetComponent<Rigidbody>().mass = objRb.mass;
                obj.GetComponent<Rigidbody>().drag = objRb.drag;
                obj.GetComponent<Rigidbody>().angularDrag = objRb.angularDrag;
                obj.GetComponent<Rigidbody>().useGravity = objRb.useGravity;
                obj.GetComponent<Rigidbody>().isKinematic = objRb.isKinematic;
                obj.GetComponent<Rigidbody>().interpolation = objRb.interpolation;
                obj.GetComponent<Rigidbody>().collisionDetectionMode = objRb.collisionDetectionMode;
                obj.GetComponent<Rigidbody>().constraints = objRb.constraints;
            }
            DestroyImmediate(rbObj);
            obj.GetComponent<BoxedConvexCollider>().colliderPrecision = objColliderPrecision;
            obj.GetComponent<BoxedConvexCollider>().mergePrecision = objMergePrecision;
            obj.GetComponent<BoxedConvexCollider>().colliderMinSize = objMinSize;
            obj.GetComponent<BoxedConvexCollider>().colliderMeshCount = meshCount;
            obj.GetComponent<BoxedConvexCollider>().colliderMeshPrecision = objColliderPrecision;
            obj.GetComponent<BoxedConvexCollider>().colliderMeshMinSize = objMinSize;
            obj.GetComponent<BoxedConvexCollider>().colliderIsTrigger = objTrigger;
            obj.GetComponent<BoxedConvexCollider>()._colliderIsTrigger = objTrigger;
            obj.GetComponent<BoxedConvexCollider>().colliderEnableCollider = true;
            obj.GetComponent<BoxedConvexCollider>()._colliderEnableCollider = true;
            obj.GetComponent<BoxedConvexCollider>().colliderShowCollider = false;
            if (executeBreak)
            {
                RemoveCollider(obj);
                obj.GetComponent<BoxedConvexCollider>().colliderCalculated = false;
            }
            else
            {
                obj.GetComponent<BoxedConvexCollider>().colliderCalculated = true;
            }
        }
        foreach (Transform child in obj.transform)
        {
            if (!executeBreak)
            {
                AddColliders(child.gameObject);
            }
        }
    }

    void RemoveColliders(GameObject obj)
    {
        foreach (BoxedConvexCollider col in obj.GetComponents<BoxedConvexCollider>())
        {
            DestroyImmediate(col);
        }
        RemoveCollider(obj);
        foreach (Transform child in obj.transform)
        {
            RemoveColliders(child.gameObject);
        }
    }
    
    void RemoveCollider(GameObject obj)
    {
        foreach (Collider col in obj.GetComponents<Collider>())
        {
            DestroyImmediate(col);
        }
    }

    public class ColliderPosition
    {
        public int x;
        public int y;
        public int z;
    }
}
