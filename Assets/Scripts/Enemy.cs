using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Room _room;
    [SerializeField] GameObject deathPrefab;
    [SerializeField] bool isRoomCondition;

    void Start()
    {
        if (isRoomCondition && _room != null)
        {
            _room.enemies.Add(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        print($"Collision {collision.gameObject.tag}");
        switch (collision.gameObject.tag)
        {
            case "Slash":
                GameObject tempPrefab = Instantiate(deathPrefab, transform.position, transform.localRotation);
                Destroy(tempPrefab, 0.5f);

                if (isRoomCondition && _room != null)
                {
                    _room.removeEnemy(gameObject);
                }

                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
