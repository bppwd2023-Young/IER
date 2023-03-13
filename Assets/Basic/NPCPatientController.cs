using UnityEngine;
using System.Collections;

public class NPCPatientController : MonoBehaviour {

    public float movementSpeed = 5.0f;
    public float rotationSpeed = 50.0f;
    public float minimumDistance = 1.0f;
    public float maximumDistance = 10.0f;
    public float minWaitTime = 1.0f;
    public float maxWaitTime = 5.0f;
    public string[] dialogueLines;

    private Transform target;
    private bool hasDialogue = false;
    private int dialogueIndex = 0;

    void Start() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(MovementCoroutine());
    }

    void Update() {
        if (target) {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= maximumDistance && distance >= minimumDistance) {
                // Stop movement when player is close
                StopAllCoroutines();
                hasDialogue = true;

                if (dialogueIndex < dialogueLines.Length) {
                    // Display the current dialogue line
                    Debug.Log(dialogueLines[dialogueIndex]);

                    // Move to the next line of dialogue
                    dialogueIndex++;
                }
                else {
                    // Reset the dialogue state
                    dialogueIndex = 0;
                    hasDialogue = false;

                    // Start moving again after dialogue is finished
                    StartCoroutine(MovementCoroutine());
                }
            }
        }
    }

    IEnumerator MovementCoroutine() {
        while (true) {
            // Choose a random point to move to within the minimum and maximum distance
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * Random.Range(minimumDistance, maximumDistance);
            randomPoint.y = transform.position.y;

            // Rotate towards the random point
            Vector3 direction = randomPoint - transform.position;
            direction.y = 0;

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

            // Move towards the random point
            bool collided = false;
            while (Vector3.Distance(transform.position, randomPoint) > 0.1f) {
                transform.Translate(0, 0, movementSpeed * Time.deltaTime);
                if (collided) {
                    direction *= -1;
                    rotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
                    collided = false;
                }
                yield return null;
            }

            // Wait for a random amount of time before choosing a new point to move to
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Obstacle") {
            // If the NPC collides with an obstacle, turn around in the opposite direction
            Vector3 direction = transform.position - collision.transform.position;
            direction.y = 0;
            direction *= -1;

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

            // Set collided to true to handle turning around in the while loop in MovementCoroutine
            bool collided = true;
        }
    }
}
