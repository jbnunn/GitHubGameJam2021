using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BugButtons : MonoBehaviour
{
 
    public GameManager gameManager;
    public Text timer;

    private void Awake() {
        //timer.GetComponent<Text>().enabled = false;
        timer.enabled = false;
    }

    private void Update()
    {
        if (gameManager.bugSelectionState != BugSelectionState.NONE || GameObject.FindGameObjectsWithTag("AutonomousVehicle").Length == 0)
        {
            this.GetComponent<Button>().interactable = false;
        } else {

            this.GetComponent<Button>().interactable = true;
        
            if (this.name == "Red Light Icon" && gameManager.disruptionPower < gameManager.store.redLightHackCost)
            {
                this.GetComponent<Button>().interactable = false;
            }

            if (this.name == "Quarantine Icon" && gameManager.disruptionPower < gameManager.store.quarantineCost)
            {
                this.GetComponent<Button>().interactable = false;
            }

            if (this.name == "Compass Icon" && gameManager.disruptionPower < gameManager.store.navigationBugCost)
            {
                this.GetComponent<Button>().interactable = false;
            }
        }
        
    }

    public void ImageClicked()
    {
        gameManager.buttonClickSound.Play();
        if (EventSystem.current.currentSelectedGameObject.name == "Red Light Icon")
        {
            if (gameManager.store.PurchaseRedLightHack())
            {
                gameManager.bugSelectionState = BugSelectionState.REDLIGHT;
                timer.enabled = true;
                StartCoroutine(Countdown(gameManager.redLightHackDuration));
                StartCoroutine(HackRedLights());
            }
        } else if (EventSystem.current.currentSelectedGameObject.name == "Quarantine Icon")
        {
            if (gameManager.store.PurchaseQuarantine())
            {
                gameManager.bugSelectionState = BugSelectionState.QUARANTINE;
                timer.enabled = true;
                StartCoroutine(Countdown(gameManager.quarantineDuration));
                StartCoroutine(QuarantineVehicles());
            }
        } else if (EventSystem.current.currentSelectedGameObject.name == "Compass Icon")
        {
            if (gameManager.store.PurchaseNavigationBug())
            {
                gameManager.bugSelectionState = BugSelectionState.NAVIGATION;
                timer.enabled = true;
                StartCoroutine(Countdown(gameManager.navigationBugDuration));
                StartCoroutine(EnableNavigationBug());
            }
        }  
    }
    
    IEnumerator Countdown(float duration)
    {
        float timeLeft = duration;
        while (timeLeft > 0)
        {
            timer.text = timeLeft.ToString("0");
            yield return new WaitForSeconds(1);
            timeLeft--;
        }
        gameManager.bugSelectionState = BugSelectionState.NONE;
        timer.enabled = false;
    }

    IEnumerator EnableNavigationBug() {
        // Vehicle continues to drive but doesn't deliver packages
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("AutonomousVehicle");

        for (int i=0; i<vehicles.Length; i++) {
            Van van = vehicles[i].GetComponent<Van>();
            van.isHacked = true;
        }
        
        yield return new WaitForSeconds(gameManager.navigationBugDuration);
        
        vehicles = GameObject.FindGameObjectsWithTag("AutonomousVehicle");

        for (int i=0; i<vehicles.Length; i++) {
            Van van = vehicles[i].GetComponent<Van>();
            van.isHacked = false;
            van.Resume();
        }

        gameManager.bugSelectionState = BugSelectionState.NONE;
    }

    IEnumerator QuarantineVehicles() {
        // Makes all vehicles in the scene stop moving
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("AutonomousVehicle");

        for (int i = 0; i < vehicles.Length; i++)
        {
            vehicles[i].GetComponent<Van>().Stop();
        }

        yield return new WaitForSeconds(gameManager.quarantineDuration);

        vehicles = GameObject.FindGameObjectsWithTag("AutonomousVehicle");

        // Resume normal van operation
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i] != null) {
                vehicles[i].GetComponent<Van>().Resume();
            }
        }

        gameManager.bugSelectionState = BugSelectionState.NONE;
    }

    IEnumerator HackRedLights() {
        // Makes all traffic lights red for a short period of time
        TrafficSimulation.Intersection[] intersections = GameObject.FindObjectsOfType<TrafficSimulation.Intersection>();
        for (int i = 0; i < intersections.Length; i++)
        {
            intersections[i].intersectionType = TrafficSimulation.IntersectionType.HACKED_LIGHTS;
        }

        yield return new WaitForSeconds(gameManager.redLightHackDuration);

        // Reset to normal state
        for (int i = 0; i < intersections.Length; i++)
        {
            intersections[i].ResumeNormalLightsOperation();
        }
        
    }
   
}
