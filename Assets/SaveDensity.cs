using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GridDensity
{
    public int[] name;
    public int[] count;
    public int[] density;
}

public class SaveDensity : MonoBehaviour
{
    [SerializeField]
    private bool save;

    [SerializeField]
    private GridAgentList[] grid;

    private bool ready = true;
    private GridDensity gridDensity;
    private int count = 0;
    private bool once = true;

    // Start is called before the first frame update
    void Start()
    {
        gridDensity = new GridDensity();
        int lenght = grid.Length * 101;
        gridDensity.density = new int[lenght];
        gridDensity.name = new int[lenght];
        gridDensity.count = new int[lenght];
    }

    // Update is called once per frame
    void Update()
    {
        if (save && ready)
        {
            StartCoroutine(JsonAppend());
        }

        if (save && count == 40 && once)
        {
            JsonSave();
            once = false;
        }
    }

    IEnumerator JsonAppend()
    {
        ready = false;
        int a = grid.Length * count;

        for (int i = 0; i < grid.Length; i++)
        {
            gridDensity.name[a + i] = i;
            gridDensity.count[a + i] = count;
            gridDensity.density[a + i] = grid[i].GetListLength();
        }

        yield return new WaitForSeconds(5.0f);
        count++;
        ready = true;
    }

    private void JsonSave()
    {
        string path = "C:\\Users\\juyie\\Desktop\\SimulationData\\";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string saveJson = JsonUtility.ToJson(gridDensity);

        File.WriteAllText(path + "gridDensity" + gameObject.name + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".json", saveJson);
    }
}
