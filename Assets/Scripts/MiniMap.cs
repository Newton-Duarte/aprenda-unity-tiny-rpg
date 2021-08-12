using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] Transform map;
    [SerializeField] GameObject[] rooms;
    [SerializeField] GameObject[] gameplayRooms;
    int currentRoom;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject room in gameplayRooms)
        {
            room.SetActive(false);
        }

        gameplayRooms[currentRoom].SetActive(true);

        foreach (GameObject room in rooms)
        {
            room.SetActive(false);
        }

        rooms[currentRoom].SetActive(true);
    }

    internal void updateMiniMap(int roomId)
    {
        gameplayRooms[currentRoom].SetActive(false);
        gameplayRooms[roomId].SetActive(true);

        rooms[roomId].SetActive(true);

        currentRoom = roomId;

        Vector2 newMapPosition = new Vector2(rooms[roomId].transform.localPosition.x, rooms[roomId].transform.localPosition.y) * -1;
        map.localPosition = newMapPosition;
    }
}
