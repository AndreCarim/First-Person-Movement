using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    
    private PlayerInput playerInput; // Reference to the player input system
    private PlayerInput.OnFootActions onFoot; // Actions for on-foot movement input

    private CharacterController controller; // Reference to the CharacterController component for the player
    private Vector3 playerVelocity; // Current velocity of the player
    private bool isGrounded; // Flag indicating whether the player is grounded
    private float speed = 3.5f; // Base movement speed of the player
    private float currentSpeed; // Current movement speed of the player

    public float gravity = -9.8f; // Gravity applied to the player

    private bool canWalk = true; // Flag indicating whether the player can walk
    private float maxVelocityUpdate = 10f; // Maximum velocity update allowed
    private float acceleration = 10f; // Acceleration applied to smooth out player movement


    




    



    public  void Start(){

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        controller = GetComponent<CharacterController>();

        currentSpeed = speed;


        onFoot.Enable();
    }


    void FixedUpdate(){

        isGrounded = controller.isGrounded;

       
        ProcessMove(onFoot.Move.ReadValue<Vector2>());

        

    }
    

    
    
   private void ProcessMove(Vector2 input) {

        // Reset vertical velocity when grounded
        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = -2f;
        }

        if (canWalk) {
            // Calculate target velocity based on input and desired speed
            Vector3 moveDirection = transform.TransformDirection(new Vector3(input.x, 0, input.y));
            Vector3 targetVelocity = moveDirection * currentSpeed;

            // Calculate the change in velocity required to reach the target velocity
            Vector3 velocityUpdate = (targetVelocity - playerVelocity);

            // Clamp the velocity change to a maximum value
            velocityUpdate.x = Mathf.Clamp(velocityUpdate.x, -maxVelocityUpdate, maxVelocityUpdate);
            velocityUpdate.z = Mathf.Clamp(velocityUpdate.z, -maxVelocityUpdate, maxVelocityUpdate);
            velocityUpdate.y = 0;

            // Apply acceleration to smooth out the movement
            playerVelocity += velocityUpdate * acceleration * Time.deltaTime;
        }

        // Apply gravity
        playerVelocity.y += gravity * Time.deltaTime;

        // Move the player using the character controller
        controller.Move(playerVelocity * Time.deltaTime);

        // Se não pode andar, interromper o movimento horizontal
        if (!canWalk) {
            playerVelocity.x = 0;
            playerVelocity.z = 0;
        }
    }





    
    public void setCanWalk(bool value){

        canWalk = value;
    }

    private void OnDisable() {

        onFoot.Disable();
    }

    public void updateSpeed(float value){
        speed += value;
    }

}
