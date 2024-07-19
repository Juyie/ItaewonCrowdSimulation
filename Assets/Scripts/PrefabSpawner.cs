using UnityEngine;

namespace JKress.AITrainer
{
    public class PrefabSpawner : MonoBehaviour
    {
        [SerializeField] GameObject[] basePrefab;

        [SerializeField] int xCount = 5;
        [SerializeField] int zCount = 5;

        [SerializeField] float offsetX = 20f;
        [SerializeField] float offsetZ = 20f;

        GameObject scenePrefab;

        void Awake()
        {
            if (scenePrefab == null)
            {
                scenePrefab = GameObject.FindWithTag("agentPrefab");
            }
            if (scenePrefab != null) Destroy(scenePrefab); //If prefab is in the scene, remove it

            float behaviorOffset = 0;

            for (int k = 0; k < basePrefab.Length; k++)
            {
                //Spawn prefabs along x and z from basePrefab 
                for (int i = 0; i < xCount; i++)
                {
                    for (int j = 0; j < zCount; j++)
                    {
                        GameObject go = Instantiate(basePrefab[k], new Vector3(i * offsetX + behaviorOffset, 0, j * offsetZ),
                            Quaternion.identity);
                        go.transform.GetChild(1).GetComponent<WalkerAgent>().enabled = true;
                        go.transform.GetChild(4).GetComponent<MovingGround>().enabled = true;
                    }
                }
            }
        }
    }
}
