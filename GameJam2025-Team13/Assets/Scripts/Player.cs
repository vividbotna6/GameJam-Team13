using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController Controller;
    public GameObject[] ratModels;
    public bool testModelSwitch;


    public float Speed;

    public Transform Cam;

    [SerializeField] int currentModelIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        Controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (testModelSwitch)
        {
            SwitchModel();
            testModelSwitch = false;
        }
        
        float Horizontal = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
        float Vertical = Input.GetAxis("Vertical") * Speed * Time.deltaTime;



        Vector3 Movement = Cam.transform.right * Horizontal + Cam.transform.forward * Vertical;
        Movement.y = 0f;


        Controller.Move(Movement);

        if (Movement.magnitude != 0f)
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Cam.GetComponent<CameraMove>().sensitivity * Time.deltaTime);

            Quaternion CamRotation = Cam.rotation;
            CamRotation.x = 0f;
            CamRotation.z = 0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, CamRotation, 0.1f);
        }





        //to do
        //track current model index
        //on collect turn off current model, incriment index, turn on new model, 
    }
     void SwitchModel()
        {
            ratModels[currentModelIndex].SetActive(false);
            currentModelIndex++;
            ratModels[currentModelIndex].SetActive(true);
        }

    // void TestIndex()
    //     {
            
    //     }
}
