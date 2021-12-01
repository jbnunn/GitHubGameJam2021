using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NotificationCanvasUI : MonoBehaviour
{
    public GameObject contentBG;
    public Text textContent;

    private GameManager gameManager;

    void Start() {
        gameManager = GameManager.Instance;
    }

    public void DisableCanvas() {
        contentBG.SetActive(false);
        textContent.gameObject.SetActive(false);
    }

    public IEnumerator AnimateWaveLabel() {
        // Enable the backtground of the content label
        contentBG.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        
        // Position the text off the screen
        textContent.transform.position = new Vector2(Screen.width + 100, textContent.transform.position.y);
        
        // Slide the text content in to the center of the screen
        textContent.gameObject.SetActive(true);
        textContent.transform.DOMove(new Vector2(Screen.width * 0.5f, textContent.transform.position.y), 1);

        // Show it for 3 seconds
        yield return new WaitForSeconds(3f);

        // Slide the text content out of the center of the screen
        textContent.transform.DOMove(new Vector2(-Screen.width * 0.5f, textContent.transform.position.y), 1);
        yield return new WaitForSeconds(.5f);

        contentBG.transform.DOMove(new Vector2(Screen.width * 0.5f, contentBG.transform.position.y), 1);
        yield return new WaitForSeconds(.5f);
        
        contentBG.SetActive(false);
        DisableCanvas();

        yield return null;
    }

    public IEnumerator AnimateEndScreen(int wave, int disruptionPower) {
        
        // Enable the backtground of the content label
        contentBG.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        // Position the text off the screen
        textContent.transform.position = new Vector2(Screen.width + 100, textContent.transform.position.y);

        // Set the GAME OVER text
        textContent.text = "GAME OVER";
        
        // Slide the text content in to the center of the screen
        textContent.gameObject.SetActive(true);
        textContent.transform.DOMove(new Vector2(Screen.width * 0.5f, textContent.transform.position.y), 1);

        // Show it for 2 seconds
        yield return new WaitForSeconds(2f);
        
        // Position the text off the screen
        textContent.transform.position = new Vector2(Screen.width + 100, textContent.transform.position.y);

        // Set the FINAL SCORE text
        textContent.text = "FINAL SCORE";

        // Slide the text content in to the center of the screen
        textContent.transform.DOMove(new Vector2(Screen.width * 0.5f, textContent.transform.position.y), 1);

        // Show it for 2 seconds
        yield return new WaitForSeconds(2f);

        // Position the text off the screen
        textContent.transform.position = new Vector2(Screen.width + 100, textContent.transform.position.y);

        // Set the final score
        textContent.text = wave * disruptionPower + "";

        // Slide the text content in to the center of the screen
        textContent.transform.DOMove(new Vector2(Screen.width * 0.5f, textContent.transform.position.y), 1);

        // Show it for 2 seconds
        yield return new WaitForSeconds(3f);

         // Position the text off the screen
        textContent.transform.position = new Vector2(Screen.width + 100, textContent.transform.position.y);

        // Set the PRESS SPACE text
        textContent.text = "PRESS SPACE TO TRY AGAIN";

        // Slide the text content in to the center of the screen
        textContent.transform.DOMove(new Vector2(Screen.width * 0.5f, textContent.transform.position.y), 1);

        yield return null;
    }

}
