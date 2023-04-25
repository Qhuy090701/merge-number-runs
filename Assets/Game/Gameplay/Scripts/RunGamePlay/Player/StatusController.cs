using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StatusController : PlayerMove
{
    [SerializeField] private bool status;
    [SerializeField] private Behaviour scripts;
    private PlayerMove playerMove;
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
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            currentState = PlayerMove.PlayerState.Moving;
            status = true;
            scripts.enabled = status;
        }
    }
}
