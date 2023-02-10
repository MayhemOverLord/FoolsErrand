using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 4;
    public Transform goal;

    void Start() {
        goal.SetParent(null);
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, goal.position, speed*Time.deltaTime);
        if(Vector3.Distance(transform.position,goal.position) == 0f){
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal"))==1f){
                goal.position = goal.position - new Vector3(Input.GetAxisRaw("Horizontal")*speed*0.5f, 0f, 0f);
            }
            else if(Mathf.Abs(Input.GetAxisRaw("Vertical"))==1f){
                goal.position = goal.position - new Vector3(0f, Input.GetAxisRaw("Vertical")*speed*0.5f, 0f);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Interaction
        }
    }
}