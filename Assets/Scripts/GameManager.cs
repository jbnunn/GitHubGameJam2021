using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TrafficSimulation;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    [HideInInspector]
    public GameState gameState;

    [HideInInspector]
    public BugSelectionState bugSelectionState;

    [Header("Light Settings")]
    public Light mainDirectionalLight; // SceneManager makes this scene dark, this hask is temporary until i figure it out

    [Header("Level Settings")]
    public TrafficSystem trafficSystem;
    
    [Header("Prefabs")]
    public GameObject packagePrefab;
    public GameObject vanPrefab;

    [Header("UI Settings")]
    public GameObject disruptionPowerText;
    public GameObject deliveriesRemainingText;
    public Text bugCostNavigationText;
    public Text bugCostRedLightsText;
    public Text bugCostQuarantineText;
    public Text waveValue;
    public Canvas notificationCanvasUI;
    
    [Header("Game Settings")]
    public int disruptionPower = 100;
    public int totalPackagesToDeliver = 100;
    public int packageValue = 5;
    [HideInInspector] public Vector3 targetPositionForScore;
    [HideInInspector] public Vector3 targetPositionForPackage;
    private Vector3 scoreArea;
    private Vector3 packageArea;
    private int wave;

    [HideInInspector]
    public bool playerIsPurchasing;

    [Header("Bug Settings")]
    [HideInInspector]
    public Intersection[] redLightHackIntersections;
    public float quarantineDuration = 5.0f;
    public float navigationBugDuration = 5.0f;
    public float redLightHackDuration = 5.0f;

    [Header("Vans")]
    public int vanPackagesToDeliver = 5;
    private int packagesDelivered;
    public float totalTimeToDeliver = 40f;  // How long the van stays on the map to deliver all packages
    public float packageDeliveryTime = 2.0f; // How long the van stays to deliver a package
    public float spawnInterval = 8f;
    private int maxVehiclesForWave;
    private int numVehiclesSpawned = 0;
    private int lastSpawnSegmentId = -1;
    private float lastSpawnTime;
    public DeliveryTarget[] deliveryTargets;

    [Header("Vehicle Entry/Exit Segments")]
    public List<Segment> exitSegments;  
    public List<Segment> entrySegments;

    [Header("Audio Settings")]
    public AudioSource backgroundMusicLoop;
    public AudioSource backgroundTrafficLoop;
    public AudioSource honkSound;
    public AudioSource[] doorbellSounds;
    public AudioSource explosionSound;
    public AudioSource buttonClickSound;
    public AudioSource packageCapturedSound;

    private IEnumerator coroutine;

    [HideInInspector]
    public Store store;

    void Start() {
        mainDirectionalLight.intensity = 2.0f;
        ResetGame();
    }

    void Update() {
      
        if (gameState == GameState.RUNNING) {
            // Spawn a vehicle every spawnInterval seconds
            if (numVehiclesSpawned < maxVehiclesForWave) {
                if (GetNumberOfVehicles() < maxVehiclesForWave && Time.time - lastSpawnTime > spawnInterval) {
                    SpawnVehicle();
                    lastSpawnTime = Time.time;
                } 
            } if (numVehiclesSpawned >= maxVehiclesForWave) {
                // Wave is over
                if (GetNumberOfVehicles() == 0) {
                    if (totalPackagesToDeliver > 0) {
                        wave++;
                        // Wave is over, but there are still packages to deliver
                        CancelInvoke("PlayRandomHonks");
                        gameState = GameState.NEXT_WAVE;
                        StartWave(wave);
                    } else {
                        // Game is over
                        gameState = GameState.ENDED;
                        GameOver();
                    }
                }
            }
        }

        if (gameState == GameState.ENDED) {
            // Game is over
            StopAllVehicles();
            
            if (Input.GetKeyDown(KeyCode.Space)) {
                ResetGame();
            }
        }

        // Uncomment for testing
        /*
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
            SpawnVehicle();
        }
        */

        if (Input.GetKeyDown(KeyCode.P))  {
            // Pause/unpause the game
            if (gameState == GameState.RUNNING) {
                gameState = GameState.PAUSED;
                Pause(true);
            } else if (gameState == GameState.PAUSED) {
                gameState = GameState.RUNNING;
                Pause(false);
            }
        }
        
    }

    private void ResetGame() {
        // Remove all vans
        Van[] vans = FindObjectsOfType<Van>();
        foreach (Van van in vans) {
            Destroy(van.gameObject);
        }

        notificationCanvasUI.GetComponent<NotificationCanvasUI>().DisableCanvas();
        gameState = GameState.INIT;

        bugSelectionState = BugSelectionState.NONE;
        playerIsPurchasing = false;
        store = GetComponent<Store>();
        wave = 1;
        
        SetMaxVehiclesForWave(wave);
        ResetDeliveredPackageCount();
        StartWave(wave);    // Sets the gamestate to RUNNING
        PlayBackgroundAudio();
        UpdateDisruptionPower(disruptionPower);
        UpdateBugUI();
    }


    private void PlayBackgroundAudio() {
        backgroundMusicLoop.Play();
        backgroundTrafficLoop.Play();
    }

    private void StopBackgroundAudio() {
        backgroundMusicLoop.Stop();
        backgroundTrafficLoop.Stop();
    }

    private void PlayRandomHonks() {

        // These are annoying. Ignoring this for now
        return;
        /*
        if (!honkSound.isPlaying) {
            int numTimesToHonk = Random.Range(1, 3);
            for (int i = 0; i < numTimesToHonk; i++) {
                honkSound.Play();
            }
        }

        Invoke("PlayRandomHonks", Random.Range(5.0f, 20.0f));
        */
    }

    private void SetMaxVehiclesForWave(int wave) {
        maxVehiclesForWave = (int)((wave * 2) + 3);
    }

    private void StartWave(int wave) {
        UpdateWaveValue(wave);
        SetMaxVehiclesForWave(wave);
        StartCoroutine(notificationCanvasUI.GetComponent<NotificationCanvasUI>().AnimateWaveLabel());
        gameState = GameState.RUNNING;
        PlayBackgroundAudio();
        //Invoke("PlayRandomHonks", Random.Range(11.0f, 18.0f));

    }

    public void SetTargetPositionForScore() {
        // Undelivered packages will animate towards this target when a van explodes
        RectTransform rt = disruptionPowerText.GetComponent<RectTransform>();
        scoreArea = Camera.main.ScreenToWorldPoint(rt.TransformPoint(rt.rect.center));
        targetPositionForScore = new Vector3(scoreArea.x, scoreArea.y, scoreArea.z);
    }

    public void SetTargetPositionForPackages() {
        // Delivered packages will originate from this target
        RectTransform rt = deliveriesRemainingText.GetComponent<RectTransform>();
        packageArea = Camera.main.ScreenToWorldPoint(rt.TransformPoint(rt.rect.center));
        targetPositionForPackage = new Vector3(packageArea.x - 11, packageArea.y - 2, packageArea.z);
    }

    private void Pause(bool pause) {
        Time.timeScale = pause ? 0 : 1;
    }

    private void UpdateWaveValue(int waveNum) {
        waveValue.text = waveNum.ToString();
        notificationCanvasUI.GetComponent<NotificationCanvasUI>().textContent.text = "WAVE " + waveNum.ToString();
    }

    private void UpdateBugUI() {
        bugCostNavigationText.text = "(" + store.navigationBugCost.ToString() + ")";
        bugCostRedLightsText.text = "(" + store.redLightHackCost.ToString() + ")";
        bugCostQuarantineText.text = "(" + store.quarantineCost.ToString() + ")";
    }

    public void UpdateDisruptionPower(int power) {
        disruptionPower = power;
        disruptionPowerText.GetComponent<Text>().text = power.ToString();
    }

    private void ResetDeliveredPackageCount() {
        packagesDelivered = 0;
        UpdateDeliveredPackageCount(packagesDelivered);
    }

    public void UpdateDeliveredPackageCount(int val) {
        packagesDelivered += val;
        deliveriesRemainingText.GetComponent<Text>().text = (totalPackagesToDeliver - packagesDelivered).ToString();

        if (packagesDelivered >= totalPackagesToDeliver) {
            GameOver();
        }
    }

    private void GameOver() {
        gameState = GameState.ENDED;
        CancelInvoke("PlayRandomHonks");
        StopAllVehicles();
        coroutine = notificationCanvasUI.GetComponent<NotificationCanvasUI>().AnimateEndScreen(wave, disruptionPower);
        StartCoroutine(coroutine);
        StopBackgroundAudio();
    }

    private void StopAllVehicles() {
        Van[] vans = FindObjectsOfType<Van>();
        foreach (Van van in vans) {
            van.Stop();
        }
    }

    private int GetNumberOfVehicles() {
        return FindObjectsOfType<Van>().Length;
    }
    
    private void SpawnVehicle() {
        if (gameState == GameState.RUNNING) {
            int spawnSegmentId = Random.Range(0, entrySegments.Count);

            if (spawnSegmentId == lastSpawnSegmentId) {
                while (spawnSegmentId == lastSpawnSegmentId) {
                    spawnSegmentId = Random.Range(0, entrySegments.Count);
                }
            }

            Waypoint entryWaypoint = entrySegments[spawnSegmentId].waypoints[0];

            GameObject vanGO = Instantiate(vanPrefab, entryWaypoint.transform.position, transform.rotation);
            Van van = vanGO.GetComponent<Van>();
            van.Spawn(entrySegments[spawnSegmentId], entryWaypoint, deliveryTargets);
            numVehiclesSpawned++;
        }
    }

}
