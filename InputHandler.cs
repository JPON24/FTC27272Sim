using System;
using UnityEngine;
using UnityEngine.InputSystem;

//this file is referenced in all files requiring player input
//used to make reference to input object in unity, allowing other files to read the input of the controllers
public class InputHandler : MonoBehaviour
{
    // initialize input action variables
    // these variables store the current value of each of the inputs
    [Header("Configuration")]
    InputAction leftStick1;
    InputAction rightStick1;
    InputAction leftBumper1;
    InputAction rightBumper1;
    InputAction dpad1;

    InputAction leftStick2;
    InputAction rightStick2;
    InputAction xButton2;
    InputAction aButton2;

    InputAction leftTrigger2;
    InputAction rightTrigger2;

    //awake is called before the first frame
    private void Awake() 
    {
        // these input action maps give references to action maps on the input mapping
        // actions maps allow the some inputs to be locked behind a map barrier, which is good for multiplayer
        InputActionMap player1 = InputSystem.actions.FindActionMap("Player1"); //gets player 1 action map
        InputActionMap player2 = InputSystem.actions.FindActionMap("Player2"); //gets player 2 action map
        
        // set input actions to their actual readings in the inspector

        // player 1
        leftStick1 = player1.FindAction("LeftStick1");
        rightStick1 = player1.FindAction("RightStick1");
        leftBumper1 = player1.FindAction("LeftBumper1");
        rightBumper1 = player1.FindAction("RightBumper1");

        dpad1 = player1.FindAction("Dpad1");

        // player 2
        leftStick2 = player2.FindAction("LeftStick2");
        rightStick2 = player2.FindAction("RightStick2");
        xButton2 = player2.FindAction("XButton2");
        aButton2 = player2.FindAction("AButton2");

        leftTrigger2 = player2.FindAction("LeftTrigger2");
        rightTrigger2 = player2.FindAction("RightTrigger2");
    }

    /*private void Update()
    {
        print(GetLeftStick1Reading());
        print(GetRightStick1Reading());
        print(GetLeftBumper1Reading());
        print(GetRightBumper1Reading());
        print(GetLeftStick2Reading());
        print(GetRightStick2Reading());
        print(GetXButton2Reading());
        print(GetAButton2Reading());
    }*/
    
    // getter methods, reads the actual value of each input in a specified type
    // note: Vector2 datatype return x and y
    // these methods are what are called in other files in order to get the values of each input

    public Vector2 GetLeftStick1Reading()
    {
        return leftStick1.ReadValue<Vector2>();
    }
    
    public Vector2 GetRightStick1Reading()
    {
        return rightStick1.ReadValue<Vector2>();
    }

    public float GetLeftBumper1Reading()
    {
        return leftBumper1.ReadValue<float>();
    }

    public float GetRightBumper1Reading()
    {
        return rightBumper1.ReadValue<float>();
    }

    public Vector2 GetDpad1Reading()
    {
        return dpad1.ReadValue<Vector2>();
    }

    public Vector2 GetLeftStick2Reading()
    {
        return leftStick2.ReadValue<Vector2>();
    }

    public Vector2 GetRightStick2Reading()
    {
        return rightStick2.ReadValue<Vector2>();
    }

    public float GetXButton2Reading()
    {
        return xButton2.ReadValue<float>();
    }

    public float GetAButton2Reading()
    {
        return aButton2.ReadValue<float>();
    }

    public float GetLeftTrigger2Reading()
    {
        return leftTrigger2.ReadValue<float>();
    }

    public float GetRightTrigger2Reading()
    {
        return rightTrigger2.ReadValue<float>();
    }
}
