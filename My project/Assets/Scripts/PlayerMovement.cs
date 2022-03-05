using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed;
    public float dashRange;

    private Vector2 direction;  //direction the player is walking

    private Animator animator;  //object for animating the player

    private enum Facing {UP, DOWN, LEFT, RIGHT};
    private Facing FacingDir = Facing.DOWN; //direction the player is facing

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    //sets the animator object as the Animator component
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
    }

    //Moves the player
    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);   //moves the payer (movement = direction * speed * time)
        AnimateMove(direction);
        
    }

    private void Dash()
    {
        Vector2 targetPos = Vector2.zero;
        if (FacingDir == Facing.UP)
        {
            targetPos.y = 1;
        }
        else if (FacingDir == Facing.LEFT)
        {
            targetPos.x = -1;
        }
        else if (FacingDir == Facing.DOWN)
        {
            targetPos.y = -1;
        }
        else if (FacingDir == Facing.RIGHT)
        {
            targetPos.x = 1;
        }
        transform.Translate(targetPos * dashRange);
    }

    //Gets the input from the keyboard
    private void GetInput()
    {
        direction = Vector2.zero;

        if (Input.GetKey(KeyCode.W))   //pressing W
        {
            direction += Vector2.up;   //sets the direction vector up
            FacingDir = Facing.UP;
        }
        if (Input.GetKey(KeyCode.A))   //pressing A
        {
            direction += Vector2.left;  //sets the direction vector left
            FacingDir = Facing.LEFT;
        }
        if (Input.GetKey(KeyCode.S))    //pressing S
        {
            direction += Vector2.down;  //sets the direction vector down
            FacingDir = Facing.DOWN;
        }
        if (Input.GetKey(KeyCode.D))    //pressing D
        {
            direction += Vector2.right; //sets the direction vector right
            FacingDir = Facing.RIGHT;
        }
        if (Input.GetKeyDown(KeyCode.Space))    //pressing Space
        {
            Dash();
        }
    }

    //Sets the trigger for the animations
    private void AnimateMove(Vector2 direction)
    {
        animator.SetFloat("xDir", direction.x);   //sets x direction as a float to the Animator component
        animator.SetFloat("yDir", direction.y);   //sets y direction as a float to the Animator component
    }
}