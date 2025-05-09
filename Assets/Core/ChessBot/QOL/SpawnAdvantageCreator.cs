using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnAdvantageCreator : MonoBehaviour
{
    public GameObject spawnPrefab;
    public GameObject[] spawned;

    public float baseX = 0;
    public float baseY = 0;

    public float multiplierX = 1;
    public float multiplierY = 1;

    private void Awake()
    {
        spawned = new GameObject[64];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                print("spawning");
                GameObject spawn = Instantiate(spawnPrefab, new Vector3(x, y, 0), Quaternion.identity);
                spawn.transform.SetParent(transform);
                spawn.name = $"Spawn ({x}, {y})";

                spawned[y+x*8] = spawn;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string list_str = "{ ";

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    list_str += spawned[x + y * 8].GetComponent<TMP_InputField>().text == "" ? "0, " : spawned[x + y * 8].GetComponent<TMP_InputField>().text +  ", ";

                }
                list_str += "\n";
            }

            list_str = list_str.Remove(list_str.Length - 2);
            list_str += " }";

            print(list_str);
        }

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                spawned[y + x * 8].GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    baseX + x * multiplierX,
                    baseY + y * multiplierY
                );

            }
        }
    }
}
