using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBox : MonoBehaviour {
    public List<GameObject> messages = new List<GameObject>();
    [SerializeField] GameObject chatPrefab;

    int maxMessages;

    public void OnMessageReceived(string message){
        Debug.Log("Message received: " + message);

        GameObject entry = Instantiate(chatPrefab, this.transform);
        entry.GetComponent<TMP_Text>().text = message;
        messages.Add(entry);

        /* I'm not sure how to implement a scroll bar to let players view older
        messages, so for now I've set it to just delete the oldest messages as
        new ones come in (to prevent the list from overflowing). */
        if(messages.Count > maxMessages){
            Destroy(messages[0]);
            messages.RemoveAt(0);
        }
    }
}
