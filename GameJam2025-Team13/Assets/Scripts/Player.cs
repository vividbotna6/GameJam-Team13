using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    Rigidbody Controller;
    public GameObject[] ratModels;
    public GameObject[] ratUI;
    public bool testModelSwitch;
    public bool testResetSwitch;


    public float StartSpeed;
    public float MaxSpeed;
    public float acceleration = 1f;
    public float collapseSpeed = 0f;

    public Transform Cam;

    private Vector3 lastVelocity = Vector3.zero;
    private float currentSpeed = 0f; // The current maximum speed of the character, changed by acceleration.

    [SerializeField] int currentModelIndex = 0;

    public GameObject CanvasUI;
    [SerializeField] TMP_Text UIText;

    // Start is called before the first frame update
    void Start()
    {
        Controller = GetComponent<Rigidbody>();

        currentSpeed = StartSpeed;
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (testModelSwitch)
        {
            SwitchModel();
            testModelSwitch = false;
        }

        if (testResetSwitch)
        {
            ResetModel();
            testResetSwitch = false;
        }

        float Horizontal = Input.GetAxis("Horizontal") * StartSpeed * Time.deltaTime;
        float Vertical = Input.GetAxis("Vertical") * StartSpeed * Time.deltaTime;



        Vector3 Movement = Cam.transform.right * Horizontal + Cam.transform.forward * Vertical;
        Movement.y = 0f;

        Controller.AddForce(Movement * currentSpeed);

        Vector3 currentVelocity = Controller.velocity;

        if (Horizontal + Vertical >= 1f)
        {
            float angle = Vector3.Angle(currentVelocity.normalized, lastVelocity.normalized);
            if (angle <= 30f)
            {
                // Accelerate, clamped to maximum
                currentSpeed = Mathf.Min(currentSpeed + (acceleration * Time.deltaTime), MaxSpeed);
            }
            else
            {
                // Decelerate, clamped to start speed.
                // NOTE: We can add a separate "deceleration" value here.
                currentSpeed = Mathf.Max(currentSpeed - (acceleration * Time.deltaTime), StartSpeed);
            }
        }
        else
        {
            currentSpeed = StartSpeed;
        }


        lastVelocity = currentVelocity;

        if (Movement.magnitude != 0f)
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Cam.GetComponent<CameraMove>().sensitivity * Time.deltaTime);

            Quaternion CamRotation = Cam.rotation;
            CamRotation.x = 0f;
            CamRotation.z = 0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, CamRotation, 0.1f);
        }

        if (Controller.velocity.magnitude > collapseSpeed)
        {

        }








    }
    void OnTriggerEnter(Collider other)
    {
        SwitchModel();
        Debug.Log("COLLECTED RAT");


    }
    void SwitchModel()
    {
        ratModels[currentModelIndex].SetActive(false);
        currentModelIndex++;
        ratModels[currentModelIndex].SetActive(true);
    }
    void ResetModel()
    {
        ratModels[currentModelIndex].SetActive(false);
        currentModelIndex = 0;
        ratModels[currentModelIndex].SetActive(true);
    }
        

 
}
//to do 
//make placeholder models
//figure out joints
//check speed