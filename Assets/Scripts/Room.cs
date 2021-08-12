using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] Door _door;
    public List<GameObject> enemies;

    internal void removeEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
        {
            _door.openDoor();
        }
    }
}
