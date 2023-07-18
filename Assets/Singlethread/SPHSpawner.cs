using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SPHSpawner : MonoBehaviour
{
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
