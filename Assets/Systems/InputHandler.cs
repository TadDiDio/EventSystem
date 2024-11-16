using UnityEngine;

public class InputHandler : MonoBehaviour
{
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        /*
         * Input events should be passed directly to the 
         * event system without custom logic applied. 
         * Otherwise, you will be spreading your filtering
         * logic across many scripts and this leads to bugs
         * and difficult debugging. Let recievers decide how
         * to use input signals by themselves.
         */
        GameEvents.Instance.Input.Move.Invoke(new MoveData 
        { 
            horizontalAxis = horizontal, 
            verticalAxis = vertical 
        });

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            GameEvents.Instance.Input.Crouch.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEvents.Instance.Input.Jump.Invoke();
        }    
    }
}

/*
 * Define relevant types in the files which generate them.
 */
public struct MoveData
{
    public float horizontalAxis;
    public float verticalAxis;
}
