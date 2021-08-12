using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum GameState
{
    gameplay, dialog, shop, stash
}

public class GameController : MonoBehaviour
{
    public string personName;
    public int personXp;
    public GameState currentGameState;

    // Start is called before the first frame update
    void Start()
    {
        print(Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {
        //handleKeyDownToSave()
        //handleKeyDownToLoad();
    }

    public void setGameState(GameState newState)
    {
        currentGameState = newState;
    }

    void handleKeyDownToLoad()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            load();
        }
    }

    void handleKeyDownToSave()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            save();
        }
    }

    private void save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");

        PlayerData data = new PlayerData();

        data.personName = personName;
        data.personXp = personXp;

        bf.Serialize(file, data);
        file.Close();

        print("file saved!");
    }

    private void load()
    {
        if (File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);

            PlayerData data = (PlayerData)bf.Deserialize(file);

            personName = data.personName;
            personXp = data.personXp;

            print("file loaded");
        }
        else
        {
            print("file not found");
        }
    }
}

[Serializable]
class PlayerData
{
    public string personName;
    public int personXp;
}
