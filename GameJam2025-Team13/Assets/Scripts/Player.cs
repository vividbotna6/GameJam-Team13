using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

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
    public bool testWinCondition;
    //Object spawning 
    public GameObject objectToSpawn;
    public GameObject playerObject;
    public int numberOfObjects = 1;
    public float radius = 5f;

    public float StartSpeed;
    public float MaxSpeed;
    public float acceleration = 1f;
    public float fallSpeed = 10f;
    public float collapseThreshHold = 10;

    public float collapseTimer;
    public float timerMax = 3;

    public Transform Cam;
    public Transform CamHeight;

    Vector3 CamHeightTarget = new Vector3(0,2,0);

    private Vector3 lastVelocity = Vector3.zero;
    private float currentSpeed = 0f; // The current maximum speed of the character, changed by acceleration.

    [SerializeField] int currentModelIndex = 0;

    public GameObject CanvasUI;
    [SerializeField] TMP_Text UIText;

    int arrayCount;

    // Start is called before the first frame update
    void Start()
    {
        Controller = GetComponent<Rigidbody>();

        currentSpeed = StartSpeed;
        arrayCount = ratModels.Length;
        Debug.Log("Current model index - " + currentModelIndex);   
        
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
            SpawnAroundPlayer(currentModelIndex);
            testSpawnSwitch = false;
        }
        if (testWinCondition)
        {
            WinGame();
        }
        // MOVEMENT SCRIPT - DONT TOUCH ANYMORE
        float Horizontal = Input.GetAxis("Horizontal") * StartSpeed * Time.deltaTime;
        float Vertical = Input.GetAxis("Vertical") * StartSpeed * Time.deltaTime;
        



        Vector3 Movement = Cam.transform.right * Horizontal + Cam.transform.forward * Vertical;
        Movement.y = 0f;

        Controller.AddForce(Movement.normalized * currentSpeed);
        Controller.AddForce(transform.up * fallSpeed);
        

        Vector3 currentVelocity = Controller.velocity;

        if (Horizontal + Vertical >= 1f)
        {
            float angle = Vector3.Angle(currentVelocity.normalized, lastVelocity.normalized);
            if (angle <= 30f)
            {
                // Accelerate, clamped to maximum
                currentSpeed = Mathf.Min(currentSpeed + (acceleration * Time.fixedDeltaTime), MaxSpeed);
            }
            else
            {
                // Decelerate, clamped to start speed.
                // NOTE: We can add a separate "deceleration" value here.
                currentSpeed = Mathf.Max(currentSpeed - (acceleration * Time.fixedDeltaTime), StartSpeed);
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

        
        
        Vector3 velocity = Controller.velocity;

        float speed = velocity.magnitude;

        Debug.Log("Speed - " + speed);


        //TIMER FOR LOOSING RATS -
        if (speed >= collapseThreshHold)
        {
            collapseTimer += Time.fixedDeltaTime;
            Debug.Log(collapseTimer);
            if (collapseTimer >= timerMax)
            {
                arrayCount = ratModels.Length;
                SpawnAroundPlayer(currentModelIndex);
                ResetModel();
                Debug.Log("RIP BOZO");
            }

        }
        else if (collapseTimer > 0)
        {
            collapseTimer -= Time.fixedDeltaTime;

        }








    }
    //Collider portion of script, for both ally and enemy objects
    void OnTriggerEnter(Collider collision)
    {
        
        Debug.Log("Collision Detected");
        if (collision.CompareTag("AllyRat"))
        {

            SwitchModel();
            arrayCount = currentModelIndex;
            Destroy(collision.gameObject);
            Debug.Log("current Index - " + arrayCount);
            Debug.Log("Collected Rat.");
        }
        else if (collision.CompareTag("Hazard"))
        {
            arrayCount = currentModelIndex;

            Debug.Log("current index - " + arrayCount);
            ResetModel();
            Debug.Log("Lost Rats");
            SpawnAroundPlayer(arrayCount);

        }
        // else if (collision.CompareTag("WinCondition"))
        // {
        //     Debug.Log("Player has Won!");
        //     WinGame();

        // }


    }
    void SwitchModel()
    {
        if (currentModelIndex <= 9)
        {
            ratModels[currentModelIndex].SetActive(false);
            currentModelIndex++;
            ratModels[currentModelIndex].SetActive(true);
            CamHeight.Translate(Vector3.up * 2);
        }
        else;
        
    }
    void ResetModel()
    {
        ratModels[currentModelIndex].SetActive(false);
        currentModelIndex = 0;
        ratModels[currentModelIndex].SetActive(true);
        CamHeight.localPosition = new Vector3(0, 2, 0);
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

            GameObject instance = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            Rigidbody rb = instance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                    rb.velocity = new Vector3(5, 10, 5);
                
            }
            else
            {
                    Debug.LogWarning("No Rigidbody found on the instantiated object.");   
            }
            

            
        }
    }
    public void WinGame()
    {
        
        SceneManager.LoadScene("EndScreen");
    }
    
        

 
}
//to do 
//figure out joints
//check speed
// create event to drop rats
// spawn rats
// create rat pickup