using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    FadeController _fadeController;
    GameController _gameController;
    Inventory _inventory;
    Interaction interactionScript;
    DialogManager _dialogManager;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer sr;

    float horizontal, vertical;
    float raycastX, raycastY;

    RaycastHit2D hit;
    [SerializeField] float moveSpeed;
    [SerializeField] Transform rayPoint;
    [SerializeField] LayerMask whatIsInteraction;
    [SerializeField] int keys;
    [SerializeField] LayerMask dropItemLayers;

    bool isWalk;
    bool isAttack;
    public bool isLookLeft;

    [SerializeField] GameObject slashFrontPrefab, slashBackPrefab, slashRightSidePrefab, slashLeftSidePrefab;
    public int directionId;
    // Animator Layers
    // Layer 0 = front
    // Layer 1 = back
    // Layer 2 = side
    int LAYER_FRONT = 0;
    int LAYER_BACK = 1;
    int LAYER_SIDE = 2;

    // Start is called before the first frame update
    void Start()
    {
        _fadeController = FindObjectOfType(typeof(FadeController)) as FadeController;
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
        _inventory = FindObjectOfType(typeof(Inventory)) as Inventory;
        _dialogManager = FindObjectOfType(typeof(DialogManager)) as DialogManager;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameController.currentGameState == GameState.gameplay)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            if (horizontal != 0 || vertical != 0)
            {
                isWalk = true;
                raycastX = horizontal;
                raycastY = vertical;
            }
            else if (horizontal == 0 && vertical == 0) { isWalk = false; }

            if (vertical > 0) // movimento para cima (personagem de costas)
            {
                directionId = LAYER_BACK;
                animator.SetLayerWeight(LAYER_BACK, 1);
                animator.SetLayerWeight(LAYER_SIDE, 0);
            }
            else if (vertical < 0) // movimento para baixo (personagem de frente)
            {
                directionId = LAYER_FRONT;
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
                directionId = LAYER_SIDE;
                animator.SetLayerWeight(LAYER_SIDE, 1);

                if (horizontal > 0 && isLookLeft) { flip(); }
                else if (horizontal <0 && !isLookLeft) { flip(); }
            }

            rb.velocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);
        
            if (Input.GetButtonDown("Fire1") && !isAttack)
            {
                animator.SetTrigger("Attack");
            }
        }

        hitInteract();
        updateAnimator();
    }

    void FixedUpdate()
    {
        Debug.DrawRay(rayPoint.position, new Vector2(raycastX, raycastY) * 0.12f, Color.red);
        hit = Physics2D.Raycast(rayPoint.position, new Vector2(raycastX, raycastY), 0.12f, whatIsInteraction);
    }

    void hitInteract()
    {
        if (hit)
        {
            print("Colidiu com Raycast");
            if (interactionScript == null)
            {
                interactionScript = hit.transform.gameObject.GetComponent<Interaction>();
                print("Atribuiu o interaction script");

                switch (interactionScript.interactionId)
                {
                    case 0:
                        if (!_fadeController.isDoor)
                        {
                            print("Colidiu com uma porta");
                            Door doorScript = hit.transform.gameObject.GetComponent<Door>();

                            if (!doorScript.isLocked)
                            {
                                _fadeController.isDoor = true;
                                _fadeController.startFade(doorScript);
                            }
                            else if (doorScript.isLocked && doorScript.openWithKey)
                            {
                                if (_inventory.isItemInInventory(doorScript.requiredItem))
                                {
                                    _inventory.removeItem(doorScript.requiredItem, 1);
                                    doorScript.openDoor();
                                    _fadeController.isDoor = true;
                                    _fadeController.startFade(doorScript);
                                }
                                else
                                {
                                    print("I don't have the key");
                                }
                            }
                        }
                        break;
                    case 1:
                        break;
                }
            }
        }
        else
        {
            interactionScript = null;
            print("Removeu o interaction script");
        }

        // Se o NPC for do tipo loja abre a loja ao apertar a tecla E
        if (
            Input.GetKeyDown(KeyCode.E) &&
            interactionScript != null &&
            interactionScript.interactionId == 1 &&
            !_inventory.shopPanel.activeSelf
            )
        {
            print("Open shop");
            _inventory.openShopPanel();
        }

        // Se o NPC for do tipo dialogo abre o dialogo ao apertar a tecla E
        else if (
            Input.GetKeyDown(KeyCode.E) &&
            interactionScript != null &&
            interactionScript.interactionId == 2 &&
            !_dialogManager.dialogPanel.activeSelf &&
            !_inventory.shopPanel.activeSelf
            )
        {
            print("Start conversation");
            _dialogManager.gNpc = interactionScript.gameObject;
            interactionScript.gameObject.SendMessage("startConversation", SendMessageOptions.DontRequireReceiver);
            _gameController.setGameState(GameState.dialog);
        }
        // Se o objeto for do tipo stash abre o stash ao apertar a tecla E
        else if (
            Input.GetKeyDown(KeyCode.E) &&
            interactionScript != null &&
            interactionScript.interactionId == 3 &&
            !_inventory.shopPanel.activeSelf &&
            !_dialogManager.dialogPanel.activeSelf &&
            !_inventory.stashPanel.activeSelf
            )
        {
            print("Open stash");
            _inventory.openStash();
        }
        else if (
            Input.GetKeyDown(KeyCode.E) &&
            interactionScript != null &&
            interactionScript.interactionId == 2 &&
            _dialogManager.dialogPanel.activeSelf
            )
        {
            _dialogManager.nextSpeech();
        }
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

    internal void slashHit()
    {
        GameObject slashPrefab = null;

        switch(directionId)
        {
            case 0:
                slashPrefab = slashFrontPrefab;
                break;
            case 1:
                slashPrefab = slashBackPrefab;
                break;
            case 2:
                if (isLookLeft) { slashPrefab = slashLeftSidePrefab; }
                else { slashPrefab = slashRightSidePrefab; }
                break;
        }

        GameObject tempSlash = Instantiate(slashPrefab, transform.position, transform.localRotation);
        Destroy(tempSlash, 0.5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Collectable":
                if (!_inventory.isInventoryFull())
                {
                    Collectable temp = collision.gameObject.GetComponent<Collectable>();
                    _inventory.getItem(temp.item);
                    Destroy(collision.gameObject);
                }
                break;
        }
    }

    public void usePotion()
    {
        print("I've used a potion!");
    }

    public RaycastHit2D canDropItem(float raySize)
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(rayPoint.position, new Vector2(raycastX, raycastY), raySize, dropItemLayers);

        if (raycastHit)
        {
            print(raycastHit.transform.name);
        }

        return raycastHit;
    }
}
