using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class LocationSetter : MonoBehaviour
{
    [SerializeField] private Transform meditationLocation;
    [SerializeField] private Transform afterMeditationTransform;

    [SerializeField] private AudioSource welcomeSound;

    [SerializeField] private AudioSource[] instructionSound;

    [SerializeField] private GameObject feedbackPanel;

    [SerializeField] private GameObject scaleObject; // Reference to the object whose size will change

    [SerializeField] private TMPro.TextMeshProUGUI instructionText; // Reference to the single text box

    private float inhaleDuration = 4f;
    private float holdDuration = 7f;
    private float exhaleDuration = 8f;
    private int count = 0;

    private void Start()
    {
        feedbackPanel.SetActive(false);
        instructionText.text = "";
        scaleObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Set initial scale
        scaleObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LocationSetter")
        {
            print("Triggered");
            this.transform.position = meditationLocation.position;
            this.transform.rotation = meditationLocation.rotation;

            this.gameObject.GetComponent<DynamicMoveProvider>().moveSpeed = 0;
            other.gameObject.SetActive(false);
            scaleObject.SetActive(true);
            welcomeSound.Play();
            Controller();
        }

        if(other.tag == "EndGame")
        {
            feedbackPanel.SetActive(true);
        }
    }

    private void Controller()
    {
        StartCoroutine(InstructionController());
    }

    IEnumerator InstructionController()
    {

        if (count == 0)
        {
            yield return new WaitForSeconds(3);
        }
        // Inhale
        instructionSound[0].Play();
        yield return StartCoroutine(ChangeScaleAndUpdateText(scaleObject, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(4f, 4f, 4f), inhaleDuration, "Breathe in"));

        // Hold
        instructionSound[1].Play();
        yield return StartCoroutine(ChangeScaleAndUpdateText(scaleObject, new Vector3(4f, 4f, 4f), new Vector3(4f, 4f, 4f), holdDuration, "Hold"));

        // Exhale
        instructionSound[2].Play();
        yield return StartCoroutine(ChangeScaleAndUpdateText(scaleObject, new Vector3(4f, 4f, 4f), new Vector3(0.5f, 0.5f, 0.5f), exhaleDuration, "Breathe out"));

        count++;

        if (count < 3)
        {
            Controller();
        }
        else
        {
            scaleObject.SetActive(false);
            instructionText.text = "";

            this.transform.position = afterMeditationTransform.position;
            this.transform.rotation = afterMeditationTransform.rotation;
            this.gameObject.GetComponent<DynamicMoveProvider>().moveSpeed = 2;
            //feedbackPanel.SetActive(true);
            //instructionText.text = "Meditation Complete!";
        }
    }

    IEnumerator ChangeScaleAndUpdateText(GameObject obj, Vector3 fromScale, Vector3 toScale, float duration, string instruction)
    {

        yield return new WaitForSeconds(1);
        float elapsedTime = 0f;
        float timer = duration;

        while (elapsedTime < duration)
        {
            // Update object scale
            obj.transform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / duration);

            // Update text with the remaining time
            instructionText.text = $"{instruction} {Mathf.Ceil(timer)} sec";

            elapsedTime += Time.deltaTime;
            timer -= Time.deltaTime;

            yield return null;
        }

        // Ensure final values are set
        obj.transform.localScale = toScale;
        instructionText.text = $"{instruction} 0 sec";
    }
}
