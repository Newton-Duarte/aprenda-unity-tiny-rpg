using UnityEngine;

public class FadeController : MonoBehaviour
{
    Transform mainCamera;
    Player _player;
    Shadow _playerShadow;
    MiniMap _miniMap;

    [SerializeField] Animator animator;
    Door doorScript;
    internal bool isDoor;

    void Start()
    {
        mainCamera = Camera.main.transform;
        _player = FindObjectOfType(typeof(Player)) as Player;
        _playerShadow = _player.gameObject.GetComponent<Shadow>();
        _miniMap = FindObjectOfType(typeof(MiniMap)) as MiniMap;
    }

    internal void startFade(Door door)
    {
        doorScript = door;
        animator.SetTrigger("FadeOut");
    }

    internal void OnFadeComplete()
    {
        _player.transform.position = doorScript.exit.position;
        mainCamera.position = new Vector3(doorScript.cameraPos.position.x, doorScript.cameraPos.position.y, -10);
        _miniMap.updateMiniMap(doorScript.roomId);
        _playerShadow.updateLightsList();
        isDoor = false;
        animator.SetTrigger("FadeIn");
    }
}
