using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    Rigidbody Controller;
    //
    public GameObject[] ratModels;
    public GameObject[] ratUI;
    //variables for testing rat stacking
    public bool testModelSwitch;
    public bool testResetSwitch;
    public bool testExpelSwitch;
    public bool testSpawnSwitch;
    //Object spawning 
    public GameObject objectToSpawn;
    public GameObject playerObject;
    public int numberOfObjects = 1;
    public float radius = 5f;

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

        if (testExpelSwitch)
        {
            ExpelModel();
            testExpelSwitch = false;
        }
        
        if (testSpawnSwitch)
        {
            SpawnAroundPlayer(numberOfObjects);
            testSpawnSwitch = false;
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

        // if (Controller.velocity.magnitude > collapseSpeed)
        // {

        // }

        Debug.Log("Current Speed - " + currentVelocity);







    }
    //Collider portion of script, for both ally and enemy objects
    void OnTriggerEnter(Collider collision)
    {
        
        Debug.Log("Collision Detected");
        if (collision.CompareTag("AllyRat"))
        {
            SwitchModel();
            Destroy(collision.gameObject);
            Debug.Log("Collected Rat.");
        }
        else if(collision.CompareTag("Hazard"))
        {
            ResetModel();
            Debug.Log("Lost Rats");
            SpawnAroundPlayer(10);

        }


    }
    void SwitchModel()
    {
        ratModels[currentModelIndex].SetActive(false);
        if (currentModelIndex < 10)
        {
            currentModelIndex++;
        }
        else;
        ratModels[currentModelIndex].SetActive(true);
    }
    void ResetModel()
    {
        ratModels[currentModelIndex].SetActive(false);
        currentModelIndex = 0;
        ratModels[currentModelIndex].SetActive(true);
    }
    void ExpelModel()
    {
        ratModels[currentModelIndex].SetActive(false);
        if (currentModelIndex > 0)
        {
            currentModelIndex--;
        }
        else;
        ratModels[currentModelIndex].SetActive(true);
    }


public void SpawnAroundPlayer(int count)
{
    if (objectToSpawn == null || playerObject == null)
    {
        Debug.LogWarning("Missing Spawner object or player object reference.");
        return;
    }

    for (int i = 0; i < count; i++)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(0.5f * radius, radius);
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        Vector3 spawnPosition = playerObject.transform.position + offset;

        Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
    }
}
    
        

 
}
//to do 
//figure out joints
//check speed
// create event to drop rats
// spawn rats
// create rat pickup