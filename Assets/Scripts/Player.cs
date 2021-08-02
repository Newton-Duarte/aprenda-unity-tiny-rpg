using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer sr;

    [SerializeField] float moveSpeed;

    bool isWalk;
    bool isAttack;
    bool isLookLeft;

    // Animator Layers
    // Layer 0 = front
    // Layer 1 = back
    // Layer 2 = side
    int LAYER_BACK = 1;
    int LAYER_SIDE = 2;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0) { isWalk = true; }
        else if (horizontal == 0 && vertical == 0) { isWalk = false; }

        if (vertical > 0) // movimento para cima (personagem de costas)
        {
            animator.SetLayerWeight(LAYER_BACK, 1);
            animator.SetLayerWeight(LAYER_SIDE, 0);
        }
        else if (vertical < 0) // movimento para baixo (personagem de frente)
        {
            animator.SetLayerWeight(LAYER_BACK, 0);
            animator.SetLayerWeight(LAYER_SIDE, 0);
        }

        if (vertical != 0)
        {
            sr.flipX = false;
            isLookLeft = false;
        }

        if (vertical == 0 && horizontal != 0) // movimento para os lados
        {
            animator.SetLayerWeight(LAYER_SIDE, 1);

            if (horizontal > 0 && isLookLeft) { flip(); }
            else if (horizontal <0 && !isLookLeft) { flip(); }
        }

        rb.velocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);
        
        if (Input.GetButtonDown("Fire1") && !isAttack)
        {
            animator.SetTrigger("Attack");
        }

        updateAnimator();
    }

    void updateAnimator()
    {
        animator.SetBool("isWalk", isWalk);
    }

    void flip()
    {
        isLookLeft = !isLookLeft;
        sr.flipX = !sr.flipX;
    }

    void OnAttackEnd()
    {
        isAttack = false;
    }
}
