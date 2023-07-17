using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SPHSpawner : MonoBehaviour
{
    private Vector3[] spawnPositions = new[]
   {
        new Vector3(-4.5f, 0.0f, -19.5f), new Vector3(-4.3f, 0.0f, -19.5f), new Vector3(-4.1f, 0.0f, -19.5f), new Vector3(-3.9f, 0.0f, -19.5f), new Vector3(-3.7f, 0.0f, -19.5f),
        new Vector3(-3.5f, 0.0f, -19.5f), new Vector3(-3.3f, 0.0f, -19.5f), new Vector3(-3.1f, 0.0f, -19.5f), new Vector3(-2.9f, 0.0f, -19.5f), new Vector3(-2.7f, 0.0f, -19.5f),
        new Vector3(-2.5f, 0.0f, -19.5f), new Vector3(-2.3f, 0.0f, -19.5f), new Vector3(-2.1f, 0.0f, -19.5f), new Vector3(-1.9f, 0.0f, -19.5f), new Vector3(-1.7f, 0.0f, -19.5f),
        new Vector3(-1.5f, 0.0f, -19.5f), new Vector3(-1.3f, 0.0f, -19.5f), new Vector3(-1.1f, 0.0f, -19.5f), new Vector3(-0.9f, 0.0f, -19.5f), new Vector3(-0.7f, 0.0f, -19.5f),
        new Vector3(-0.5f, 0.0f, -19.5f), new Vector3(-0.3f, 0.0f, -19.5f), new Vector3(-0.1f, 0.0f, -19.5f),
        new Vector3(4.5f, 0.0f, -19.5f), new Vector3(4.3f, 0.0f, -19.5f), new Vector3(4.1f, 0.0f, -19.5f), new Vector3(3.9f, 0.0f, -19.5f), new Vector3(3.7f, 0.0f, -19.5f),
        new Vector3(3.5f, 0.0f, -19.5f), new Vector3(3.3f, 0.0f, -19.5f), new Vector3(3.1f, 0.0f, -19.5f), new Vector3(2.9f, 0.0f, -19.5f), new Vector3(2.7f, 0.0f, -19.5f),
        new Vector3(2.5f, 0.0f, -19.5f), new Vector3(2.3f, 0.0f, -19.5f), new Vector3(2.1f, 0.0f, -19.5f), new Vector3(1.9f, 0.0f, -19.5f), new Vector3(1.7f, 0.0f, -19.5f),
        new Vector3(1.5f, 0.0f, -19.5f), new Vector3(1.3f, 0.0f, -19.5f), new Vector3(1.1f, 0.0f, -19.5f), new Vector3(0.9f, 0.0f, -19.5f), new Vector3(0.7f, 0.0f, -19.5f),
        new Vector3(0.5f, 0.0f, -19.5f), new Vector3(0.3f, 0.0f, -19.5f), new Vector3(0.1f, 0.0f, -19.5f),
        new Vector3(-4.5f, 0.0f, 19.5f), new Vector3(-4.3f, 0.0f, 19.5f), new Vector3(-4.1f, 0.0f, 19.5f), new Vector3(-3.9f, 0.0f, 19.5f), new Vector3(-3.7f, 0.0f, 19.5f),
        new Vector3(-3.5f, 0.0f, 19.5f), new Vector3(-3.3f, 0.0f, 19.5f), new Vector3(-3.1f, 0.0f, 19.5f), new Vector3(-2.9f, 0.0f, 19.5f), new Vector3(-2.7f, 0.0f, 19.5f),
        new Vector3(-2.5f, 0.0f, 19.5f), new Vector3(-2.3f, 0.0f, 19.5f), new Vector3(-2.1f, 0.0f, 19.5f), new Vector3(-1.9f, 0.0f, 19.5f), new Vector3(-1.7f, 0.0f, 19.5f),
        new Vector3(-1.5f, 0.0f, 19.5f), new Vector3(-1.3f, 0.0f, 19.5f), new Vector3(-1.1f, 0.0f, 19.5f), new Vector3(-0.9f, 0.0f, 19.5f), new Vector3(-0.7f, 0.0f, 19.5f),
        new Vector3(-0.5f, 0.0f, 19.5f), new Vector3(-0.3f, 0.0f, 19.5f), new Vector3(-0.1f, 0.0f, 19.5f),
        new Vector3(4.5f, 0.0f, 19.5f), new Vector3(4.3f, 0.0f, 19.5f), new Vector3(4.1f, 0.0f, 19.5f), new Vector3(3.9f, 0.0f, 19.5f), new Vector3(3.7f, 0.0f, 19.5f),
        new Vector3(3.5f, 0.0f, 19.5f), new Vector3(3.3f, 0.0f, 19.5f), new Vector3(3.1f, 0.0f, 19.5f), new Vector3(2.9f, 0.0f, 19.5f), new Vector3(2.7f, 0.0f, 19.5f),
        new Vector3(2.5f, 0.0f, 19.5f), new Vector3(2.3f, 0.0f, 19.5f), new Vector3(2.1f, 0.0f, 19.5f), new Vector3(1.9f, 0.0f, 19.5f), new Vector3(1.7f, 0.0f, 19.5f),
        new Vector3(1.5f, 0.0f, 19.5f), new Vector3(1.3f, 0.0f, 19.5f), new Vector3(1.1f, 0.0f, 19.5f), new Vector3(0.9f, 0.0f, 19.5f), new Vector3(0.7f, 0.0f, 19.5f),
        new Vector3(0.5f, 0.0f, 19.5f), new Vector3(0.3f, 0.0f, 19.5f), new Vector3(0.1f, 0.0f, 19.5f),
    };

    private Vector3 goalPos1 = new Vector3(0.0f, 0.0f, 19.5f);
    private Vector3 goalPos2 = new Vector3(0.0f, 0.0f, -19.5f);

    [Header("Import")]
    [SerializeField] private GameObject character0Prefab = null;

    [Header("Properties")]
    public int amount = 500;

    // Start is called before the first frame update
    void Start()
    {
        InitSPH();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void InitSPH()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject go = Instantiate(character0Prefab);
            go.transform.parent = GameObject.Find("Agents").transform;
            if (i % 92 <= 45)
            {
                go.transform.position = new Vector3(-4.5f + (i % 46) * 0.2f, 0.0f, -19.5f + (i / 46) % 46 * 0.3f);
                go.GetComponent<SPHAgents>().Init(go.transform.position, goalPos1);
                go.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
            }
            else
            {
                go.transform.position = new Vector3(-4.5f + (i % 46) * 0.2f, 0.0f, 19.5f - (i / 46) % 46 * 0.3f);
                go.GetComponent<SPHAgents>().Init(go.transform.position, goalPos2);
                go.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
            }
            go.name = "char" + i.ToString();
        }
    }
}
