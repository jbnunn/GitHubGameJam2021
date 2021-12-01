using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TitleManager : MonoBehaviour
{
    public Text spaceBarText;

    void Start() {
        StartCoroutine(FadeInFadeOut(1));
    }

    IEnumerator FadeInFadeOut(float fadeSeconds)
	{
		spaceBarText.CrossFadeAlpha(0.5f,fadeSeconds,true);
		yield return new WaitForSeconds(fadeSeconds);

		spaceBarText.CrossFadeAlpha(1,fadeSeconds,true);
		yield return new WaitForSeconds(fadeSeconds);

		StartCoroutine(FadeInFadeOut(fadeSeconds));
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Small Town America");
        }
    }
   
}
