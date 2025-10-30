using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : MonoBehaviour {
    // References to the UI elements
    public GameObject panel; // The panel to be shown or hidden
    public Text messageText; // The text element inside the panel

    public static bool messageDisplayed = true; // Tracks if a message is currently displayed

    // Static queue to hold all messages
    public static Queue<string> messageQueue = new Queue<string>();

    // Public getter to check if a message is displayed
    public static bool MessageLock {
        get { return messageDisplayed; }
    }

    void Start() {
        // Ensure the panel is hidden at the start
        StartCoroutine(HidePanel());
    }

    void Update() {
        // When a message is displayed, allow the player to click to go to the next message
        if (messageDisplayed && Input.GetMouseButtonDown(0)) {
            ShowNextMessage(); // Show the next message in the queue or hide the panel
        }
    }

    // Add a message to the static queue
    public static void QueueMessage(string message) {
        messageQueue.Enqueue(message);
    }

    // Show the next message in the queue
    public void ShowNextMessage() {
        // If there are still messages in the queue, display the next one
        if (messageQueue.Count > 0) {
            string nextMessage = messageQueue.Dequeue();
            ShowMessage(nextMessage);
        } 
        else {
            // If no more messages, hide the panel
            StartCoroutine(HidePanel());
        }
    }

    // Shows the panel and sets the text message
    public void ShowMessage(string message) {
        messageText.text = message; // Set the text
        panel.SetActive(true); // Show the panel
        messageDisplayed = true; // Track that a message is being displayed
    }

    // Hides the panel and clears the message
    public IEnumerator HidePanel() {
        messageText.text = ""; // Clear the text
        panel.SetActive(false); // Hide the panel
        yield return new WaitForSeconds(.2f);
        messageDisplayed = false; // Update the message status
    }
}
