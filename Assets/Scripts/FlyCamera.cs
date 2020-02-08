using UnityEngine;
using System.Collections;
 
public class FlyCamera : MonoBehaviour {


    public float moveSpeed;
    public float scrollSpeed;
    public GameObject PreMesh;

    public float scalerX, scalerY; 

    private void Start()
    {
        Destroy(PreMesh);
    }

    private void Update()
    {
        var forward = transform.forward;
        forward.y = 0;

        transform.position += (forward.normalized * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * moveSpeed
            + Vector3.up * Input.GetAxis("ChangeHeight") * moveSpeed
            + transform.forward * Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        transform.localRotation *= Quaternion.Euler(Vector3.up * Input.GetAxis("Mouse X") * scalerX) *
            Quaternion.Euler(Vector3.left * Input.GetAxis("Mouse Y") * scalerY);

        transform.rotation = Quaternion.LookRotation(transform.forward);
    }







    ///*
    //Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
    //Converted to C# 27-02-13 - no credit wanted.
    //Simple flycam I made, since I couldn't find any others made public.  
    //Made simple to use (drag and drop, done) for regular keyboard layout  
    //wasd : basic movement
    //shift : Makes camera accelerate
    //space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/


    //float mainSpeed = 100.0f; //regular speed
    //float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    //float maxShift = 1000.0f; //Maximum speed when holdin gshift
    //float camSens = 0.1f; //How sensitive it with mouse
    //private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    //private float totalRun= 1.0f;

    //void Update () {
    //    Cursor.lockState = CursorLockMode.Locked;
    //    lastMouse = Input.mousePosition - lastMouse ;
    //    lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0 );
    //    lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x , transform.eulerAngles.y + lastMouse.y, 0);
    //    transform.eulerAngles = lastMouse;
    //    lastMouse =  Input.mousePosition;
    //    //Mouse  camera angle done.  

    //    //Keyboard commands
    //    //float f = 0.0f;
    //    Vector3 p = GetBaseInput();
    //    if (Input.GetKey (KeyCode.LeftShift)){
    //        totalRun += Time.deltaTime;
    //        p  = p * totalRun * shiftAdd;
    //        p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
    //        p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
    //        p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
    //    }
    //    else{
    //        totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
    //        p = p * mainSpeed;
    //    }

    //    p = p * Time.deltaTime;
    //   Vector3 newPosition = transform.position;
    //    if (Input.GetKey(KeyCode.Space)){ //If player wants to move on X and Z axis only
    //        transform.Translate(p);
    //        newPosition.x = transform.position.x;
    //        newPosition.z = transform.position.z;
    //        transform.position = newPosition;
    //    }
    //    else{
    //        transform.Translate(p);
    //    }

    //}

    //private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
    //    Vector3 p_Velocity = new Vector3();
    //    if (Input.GetKey (KeyCode.W)){
    //        p_Velocity += new Vector3(0, 0 , 1);
    //    }
    //    if (Input.GetKey (KeyCode.S)){
    //        p_Velocity += new Vector3(0, 0, -1);
    //    }
    //    if (Input.GetKey (KeyCode.A)){
    //        p_Velocity += new Vector3(-1, 0, 0);
    //    }
    //    if (Input.GetKey (KeyCode.D)){
    //        p_Velocity += new Vector3(1, 0, 0);
    //    }
    //    return p_Velocity;
    //}
}

/*
 * using UnityEngine;
using System.Collections;
 
public class FlyCamera : MonoBehaviour {


    public float moveSpeed;
    public float scrollSpeed;
    private Vector3 lastMouse = new Vector3(255, 255, 255);
    float camSens = 0.1f;

    private void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        var forward = transform.forward;
        forward.y = 0;

        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angleY = -Remap(dir.y,0,Screen.height,-90,90);
        float angleX = Remap(dir.x, 0, Screen.width, -180,180);
        transform.rotation = Quaternion.Euler(new Vector3(angleY, angleX,0));
    }

    static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
 * */
