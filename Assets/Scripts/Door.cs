using UnityEngine;

public class Door : MonoBehaviour
{
    SpriteRenderer sr;

    public Transform exit;
    public Transform cameraPos;
    public int roomId;
    public bool isLocked;
    public bool openWithKey;
    public Item requiredItem;

    public Sprite doorOpenImg;
    public Sprite doorClosedImg;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (isLocked)
        {
            sr.sprite = doorClosedImg;
        }
        else
        {
            sr.sprite = doorOpenImg;
        }
    }

    internal void openDoor()
    {
        isLocked = false;
        sr.sprite = doorOpenImg;
    }
}
