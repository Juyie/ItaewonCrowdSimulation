using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class SPHManager : MonoBehaviour
{
    // Import
    [Header("Import")]
    [SerializeField] private GameObject sphParticlePrefab = null;
    [SerializeField] private GameObject sphColliderPrefab = null;

    // Properties
    [Header("Properties")]
    [SerializeField] private int amount = 5000;


    private void Start()
    {
        // Setup
        AddColliders();
        AddParticles(amount);
    }



    private void AddParticles(int _amount)
    {
        for (int i = 0; i < _amount; i++)
        {
            GameObject particle = Instantiate(sphParticlePrefab, new Vector3(i % 16 + UnityEngine.Random.Range(-0.1f, 0.1f), 2 + (i / 16 / 16) * 1.1f, (i / 16.0f) % 16.0f), Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
        }
    }



    private void AddColliders()
    {
        // Find all colliders
        GameObject[] colliders = GameObject.FindGameObjectsWithTag("SPHCollider");

        // Set data
        for (int i = 0; i < colliders.Length; i++)
        {
            new SPHCollider
            {
                position = colliders[i].transform.position,
                right = colliders[i].transform.right,
                up = colliders[i].transform.up,
                scale = new float2(colliders[i].transform.localScale.x / 2f, colliders[i].transform.localScale.y / 2f)
            };
        }
    }
}
