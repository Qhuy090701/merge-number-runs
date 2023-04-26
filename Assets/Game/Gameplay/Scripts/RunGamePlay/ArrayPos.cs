using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayPos : MonoBehaviour
{
    public List<GameObject> characters;

    private void Start()
    {
        characters.Add(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER_ADD))
        {
            //change tag player add to player
            other.gameObject.tag = Constants.TAG_PLAYER;
            gameObject.tag = Constants.TAG_PLAYER;
            Debug.Log("Đã va chạm");

            // Add the new object to the list in the correct position
            bool added = false;
            for (int i = 0; i < characters.Count; i++)
            {
                if (other.gameObject.transform.position.x < characters[i].transform.position.x)
                {
                    characters.Insert(i, other.gameObject);
                    added = true;
                    break;
                }
            }

            // If the new object is the farthest to the right, add it to the end of the list
            if (!added)
            {
                characters.Add(other.gameObject);
            }
        }
    }

}
