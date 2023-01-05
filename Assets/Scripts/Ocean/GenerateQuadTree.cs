using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateQuadTree : MonoBehaviour
{
    [Header("QuadTree Settings")]
    [SerializeField] private Transform sceneCamera;
    [Tooltip("In km")]
    [SerializeField] private float mapLength;
    [SerializeField] private int maxSubdivisions;
    [SerializeField] private float minScale;
    [SerializeField] private Transform treeParent;

    [Header("LOD Settings")] 
    [Tooltip("In meters, distance from player where medium resolution prefabs are spawned")]
    [SerializeField] private float highResolutionCutoff;
    [Tooltip("In meters, distance from player where low resolution prefabs are spawned")]
    [SerializeField] private float mediumResolutionCutoff;
    [SerializeField] private GameObject highResolutionPlane;
    [SerializeField] private GameObject mediumResolutionPlane;
    [SerializeField] private GameObject lowResolutionPlane;
    [SerializeField] private Material shader;
    [SerializeField] private float renderDistance;

    private int _numSubdivisions;
    private ObjectPool _lowResolutionPool;
    private ObjectPool _mediumResolutionPool;
    private ObjectPool _highResolutionPool;

    private Transform _player;

    private void Awake()
    {
        _lowResolutionPool = new ObjectPool(lowResolutionPlane);
        _mediumResolutionPool = new ObjectPool(mediumResolutionPlane);
        _highResolutionPool = new ObjectPool(highResolutionPlane);

        _player = sceneCamera;
    }

    public void AssignPlayer(Transform player)
    {
        _player = player;
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = sceneCamera;
        }
        
        int childCount = treeParent.childCount;            
        Vector3 playerPosition = _player.transform.position;

        for (int i = 0; i < childCount; i++)
        {
            GameObject currentNode = treeParent.GetChild(i).gameObject;
            currentNode.GetComponent<Temporary>().pool.Return(currentNode);
        }
        
        treeParent.DetachChildren();

        _numSubdivisions = 0;
        
        GameObject master = CreateInitialNode(new Vector3(0, 0, 0), playerPosition);
        master.transform.localScale = new Vector3(mapLength * 1000, 1, mapLength * 1000);
        
        Split(ref _numSubdivisions, ref master);
    }

    private GameObject CreateInitialNode(Vector3 position, Vector3 playerPosition)
    {
        ObjectPool pool;
        float distanceToPlayer = (position - new Vector3(playerPosition.x, 0, playerPosition.z)).magnitude;
        
        if (distanceToPlayer < highResolutionCutoff)
        {
            pool = _highResolutionPool;
        }
        else if (distanceToPlayer < mediumResolutionCutoff)
        {
            pool = _mediumResolutionPool;
        }
        else
        {
            pool = _lowResolutionPool;
        }

        return pool.Query();
    }

    private void Split(ref int numSubdivisions, ref GameObject currentNode)
    {
        Vector3 parentScale = currentNode.transform.localScale;

        if (numSubdivisions > maxSubdivisions)
            return;
        
        if (parentScale.x < minScale && parentScale.z < minScale)
            return;

        Vector3 playerPosition = _player.transform.position;
        
        float distanceToPlayer = (currentNode.transform.position - new Vector3(playerPosition.x, 0, playerPosition.z)).magnitude;
        Vector3 parentPosition = currentNode.transform.position;
        Bounds bounds = currentNode.GetComponent<Renderer>().bounds;

        bool containsPlayer = bounds.Intersects(new Bounds(new Vector3(playerPosition.x, parentPosition.y, playerPosition.z), new Vector3(renderDistance, 0, renderDistance)));
        
        if (!containsPlayer)
            return;

        ObjectPool parentPool;

        if (distanceToPlayer < highResolutionCutoff)
        {
            parentPool = _highResolutionPool;
        }
        else if (distanceToPlayer < mediumResolutionCutoff)
        {
            parentPool = _mediumResolutionPool;
        }
        else
        {
            parentPool = _lowResolutionPool;
        }
        
        numSubdivisions++;

        for (int i = 0; i < 4; i++)
        {
            ObjectPool pool;
            
            Vector3 location = i switch
            {
                0 => new Vector3(1.0f, 0.0f, 1.0f),
                1 => new Vector3(-1.0f, 0.0f, 1.0f),
                2 => new Vector3(-1.0f, 0.0f, -1.0f),
                _ => new Vector3(1.0f, 0.0f, -1.0f)
            };
            
            Vector3 movePos = currentNode.transform.position + location * ((parentScale.x/2.0f)/2.0f * 2.0f);
            float toPlayer = (movePos - new Vector3(playerPosition.x, 0, playerPosition.z)).magnitude;
            
            if (toPlayer < highResolutionCutoff)
            {
                pool = _highResolutionPool;
            }
            else if (toPlayer < mediumResolutionCutoff)
            {
                pool = _mediumResolutionPool;
            }
            else
            {
                pool = _lowResolutionPool;
            }
            
            GameObject quadrant = pool.Query();
            quadrant.transform.parent = treeParent;
            quadrant.GetComponent<Temporary>().pool = pool;
            quadrant.GetComponent<MeshRenderer>().material = shader;
            quadrant.transform.localScale = new Vector3(parentScale.x/2.0f, 1.0f, parentScale.z/2.0f);
            quadrant.transform.position = movePos;

            Split(ref numSubdivisions, ref quadrant);
        }

        currentNode.GetComponent<Temporary>().pool = parentPool;
        currentNode.transform.SetParent(null);
        parentPool.Return(currentNode);
    }
    
    public struct ObjectPool
    {
        private List<GameObject> _prefabInstances;
        private GameObject _poolPrefab;

        public ObjectPool(GameObject prefab)
        {
            _poolPrefab = prefab;
            _prefabInstances = new List<GameObject>();
        }

        public GameObject Query()
        {
            if (_prefabInstances.Count == 0)
            {
                GameObject newObject = Instantiate(_poolPrefab);
                return newObject;
            }
            else
            {
                GameObject listObj = _prefabInstances[0];
                listObj.SetActive(true);
                _prefabInstances.RemoveAt(0);
                return listObj;
            }
        }

        public void Return(GameObject prefabInstance)
        {
            prefabInstance.SetActive(false);
            prefabInstance.transform.position = Vector3.zero;
            _prefabInstances.Add(prefabInstance);
        }
    }
}
