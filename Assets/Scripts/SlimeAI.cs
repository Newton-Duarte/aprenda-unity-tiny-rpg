using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : MonoBehaviour
{
    Player _player;
    Rigidbody2D rb;

    [SerializeField] float moveSpeed;
    [SerializeField] Vector2 moveDirection;
    [SerializeField] float minMovementTime;
    [SerializeField] float maxMovementTime;
    [SerializeField] float minWaitTime;
    [SerializeField] float maxWaitTime;

    [SerializeField] LayerMask whatIsPlayer;
    [SerializeField] float perceptionArea;
    bool isLockPlayer;


    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType(typeof(Player)) as Player;
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(move());
    }

    void Update()
    {
        rb.velocity = moveDirection * moveSpeed;

        if (isLockPlayer)
        {
            moveDirection = Vector3.Normalize(_player.transform.position - transform.position);
        }
    }

    void FixedUpdate()
    {
        isLockPlayer = Physics2D.OverlapCircle(transform.position, perceptionArea, whatIsPlayer);
    }

    IEnumerator move()
    {
        moveDirection = Vector2.zero;

        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        if (!isLockPlayer)
        {
            int xDirection = Random.Range(-1, 2); // -1, 0, 1
            int yDirection = Random.Range(-1, 2); // -1, 0, 1

            moveDirection = new Vector2(xDirection, yDirection);

            yield return new WaitForSeconds(Random.Range(minMovementTime, maxMovementTime));
        }

        StartCoroutine(move());
    }
}
