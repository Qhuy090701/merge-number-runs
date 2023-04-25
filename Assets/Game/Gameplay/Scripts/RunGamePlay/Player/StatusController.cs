using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    [SerializeField] private bool status;
    [SerializeField] private Behaviour scripts;
    // Start is called before the first frame update
    void Start()
    {
        scripts.enabled = status;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Constants.TAG_PLAYER))
        {
            other.transform.localPosition  = new Vector3(transform.localPosition.x + 1.5f , transform.localPosition.y , transform.localPosition.z);
            Debug.Log("Change status = true ");
            status = true;
            scripts.enabled = status;
        }

        //if(other.CompareTag(Constants.TAG_PLAYER_ADD))
        //{
        //       Debug.Log("");

        //}
    }
}
