using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // declares the PlayerControls Input Actions as a variable for reference in the script
    private PlayerControls playerControls;

    // reference to the gameObject's Rigidbody
    private Rigidbody rb;
    // reference to the Vector3 value used to determine movement
    private Vector3 movement;

    // declaring a speed variable that can be edited in the inspector
    [SerializeField] private int speed;
    // reference to the animator
    [SerializeField] private Animator anim;
    // reference to the sprite renderer
    [SerializeField] private SpriteRenderer playerSprite;

    // reference to paramater set in Animator window
    private const string IS_WALK_PARAM = "IsWalk";

    private void Awake()
    {
        // assigns the playerControls variable the PlayerControls Input Actions on Awake
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        // makes sure playerControls are enabled when the script is enabled
        playerControls.Enable();
    }

    private void Start()
    {
        // gets the player's Rigidbody and assigns it to the rb variable
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // listens for whether or not the player has pressed the movement keys and assigns the pressed value to the floats
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;

        // outputs to the console the values of the Vector2/floats
        Debug.Log(x + "," + z);

        // normalizes the values of the Vector2 outputs so that they are flat values (e.g. 1, -1 etc) to smooth out movement. Assigns the floats x and z to the values of the Vector3. 
        movement = new Vector3(x, 0, z).normalized;

        // sets bool to true if movement values are not zero. This should mean that whenever the sprite walks, the animation transitions from idel to walk
        anim.SetBool(IS_WALK_PARAM, movement!=Vector3.zero);

        // if player moves left, flips sprite
        if(x!= 0 && x<0)
        {
            playerSprite.flipX = true;
        }

        // if player moves right, resets sprite to default orientation
        if(x!= 0 && x>0)
        {
            playerSprite.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        // moves the players rigidbody based on the movement and speed values. 
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }
}
