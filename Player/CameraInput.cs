using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
public class CameraInput : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private bool isFreeToLook = true;

    [SerializeField] private CinemachineVirtualCamera firstPersonCamera; 

    private float xRotation = 0f;
    private float xSensitivity;
    private float ySensitivity;

    private float smoothSpeed = 50f;
    private Vector2 smoothInput;

    // Variables for smooth camera look
    private float _smoothTime = 5f;

    private float _vertAngularVelocity;
    private float _horiAngularVelocity;

   


    private const string MOUSE_SENSITIVITY_KEY = "MouseSensitivity";


    public void Start(){

        float savedSensitivity = PlayerPrefs.GetFloat(MOUSE_SENSITIVITY_KEY, 5f);
        setMouseSensitivity(.5f);

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        Cursor.lockState = CursorLockMode.Locked;

        onFoot.Enable();

        setIsFreeToLook(true); // Allow camera movement for the local player
    }


    private void LateUpdate(){
        ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    public void ProcessLook(Vector2 input)
    {
        if (isFreeToLook)
        {
            // Smooth the input using linear interpolation
            smoothInput = Vector2.Lerp(smoothInput, input, smoothSpeed * Time.deltaTime);

            float mouseX = smoothInput.x * xSensitivity; // Multiply by sensitivity
            float mouseY = smoothInput.y * ySensitivity; // Multiply by sensitivity

            // Calculate vertical rotation
            xRotation -= mouseY;

            // Clamp vertical rotation to prevent flipping
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            // Apply vertical rotation
            firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, firstPersonCamera.transform.localEulerAngles.y, 0f);

            // Calculate horizontal rotation
            float targetAngle = transform.localEulerAngles.y + mouseX;

            // Apply horizontal rotation
            transform.localRotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }


    public void setIsFreeToLook(bool value)
    {        
        if (value)
        {
            // Hide and lock the cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // Make the cursor visible and unlock it
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        isFreeToLook = value;
    }

    

     private void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        onFoot.Disable();
    }

    public void setMouseSensitivity(float newValue){

        xSensitivity = newValue;
        ySensitivity = newValue;

        // Save the mouse sensitivity value to PlayerPrefs
        PlayerPrefs.SetFloat(MOUSE_SENSITIVITY_KEY, newValue);
        PlayerPrefs.Save(); // Save the PlayerPrefs data immediately
    }

    public float getMouseSensitivy(){
        return xSensitivity;
    }
}
