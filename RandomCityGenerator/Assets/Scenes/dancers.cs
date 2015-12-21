using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dancers : MonoBehaviour {


    public GameObject prefab;
    List<GameObject> _dancers = new List<GameObject>();
	// Use this for initialization
	void Start () {

        for (int x = 0; x < 20; x++)
        {
            for (int j = 0; j < 20; j++)
            {
                GameObject g = Instantiate(prefab,new Vector3(x*3,2,j*3),Quaternion.identity)as GameObject;
            }
        }

        foreach (GameObject g in _dancers)
        {
            Animation anim;

            
           anim = g.GetComponent<Animation>();
            anim.Play("move");
            // foreach (AnimationState state in anim)

            //  state.speed = 0.5F;
            // }
            //anim.Play();
        }
        

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
