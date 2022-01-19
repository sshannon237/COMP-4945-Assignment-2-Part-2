using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Worked on by Bryan Xing and Ethan Sadowski
public class SpawnFood : MonoBehaviour
{
    public GameObject food;
    // Start is called before the first frame update

    void Start()
    {
        InvokeRepeating("Spawn", 3, 4);
    }

    // Spawns a single piece of food in a random location
    void Spawn()
    {
        int x = (int)Random.Range(-25, 25);

        int y = (int)Random.Range(-25, 25);

        Instantiate(food, new Vector2(x, (float) (y - 0.5)), Quaternion.identity);
    }
}
