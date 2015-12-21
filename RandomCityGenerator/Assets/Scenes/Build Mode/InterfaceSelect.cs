using UnityEngine;
using System.Collections;

public class InterfaceSelect : MonoBehaviour {


	// Use this for initialization
	void Start () {
        //start in First person
        gameObject.GetComponent<LineRender>().enabled = true;
        gameObject.GetComponent<BuildMode>().enabled = false;
        gameObject.GetComponent<ScaleBuild>().enabled = false;
        gameObject.GetComponent<CameraMove>().enabled = false;
        gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

        //First Person View
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gameObject.GetComponent<BuildMode>().enabled = false;
            gameObject.GetComponent<ScaleBuild>().enabled = false;
            gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
            gameObject.GetComponent<CameraMove>().enabled = false;
            gameObject.GetComponent<LineRender>().enabled = false;

        }

        //Build Mode View
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gameObject.GetComponent<LineRender>().enabled = false;
            gameObject.GetComponent<CameraMove>().enabled = false;
            gameObject.GetComponent<BuildMode>().enabled = true;
            gameObject.GetComponent<ScaleBuild>().enabled = false;
            gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
        }

        //Scale View
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gameObject.GetComponent<LineRender>().enabled = false;
            gameObject.GetComponent<ScaleBuild>().enabled = true;
            gameObject.GetComponent<CameraMove>().enabled = false;
            gameObject.GetComponent<BuildMode>().enabled = false;
            gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
        }
        //movement
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            gameObject.GetComponent<LineRender>().enabled = false;
            gameObject.GetComponent<ScaleBuild>().enabled = false;
            gameObject.GetComponent<CameraMove>().enabled = true;
            gameObject.GetComponent<BuildMode>().enabled = false;
            gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
        }
        //movement
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            gameObject.GetComponent<ScaleBuild>().enabled = false;
            gameObject.GetComponent<CameraMove>().enabled = false;
            gameObject.GetComponent<BuildMode>().enabled = false;
            gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
            gameObject.GetComponent<LineRender>().enabled = true;
        }


    }
}
