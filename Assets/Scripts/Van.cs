using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TrafficSimulation;
using DG.Tweening;

public enum DeliveryService { 
    AMAZON,
    UPS,
    DHL,
    FEDEX
}

public class Van : MonoBehaviour
{
   
    [HideInInspector]
    public bool isPaused = false;
    
    [Header("Delivery Company (Randomly selected at runtime)")]
    public DeliveryService deliveryService;
    public Material AmazonMaterial;
    public Material UPSMaterial;
    public Material DHLMaterial;
    public Material FedExMaterial;

    [Header("Van Settings")]
    public float initialYPosition = 28.48f;
    public bool isHacked = false;
    public Image timerMeter;
    public GameObject failedDeliveryParticleSystem;
    
    private bool isBeingDestroyed = false;

    private float totalTimeToDeliver; // How much time a van has to deliver all packages
    private float timeLeft;
    private float packageDeliveryTime; // How long the van stays to deliver a package
    [HideInInspector] public int packagesToDeliver; // Set by GameManager

    [HideInInspector]
    public List<DeliveryTarget> deliveryTargets;
    private int currentSegment;

    private GameManager gameManager;
    
    [HideInInspector]
    public VehicleAI vehicleAI;
    
    void Awake() {
        gameManager = GameManager.Instance;
        totalTimeToDeliver = gameManager.totalTimeToDeliver;
        timeLeft = totalTimeToDeliver;
        packagesToDeliver = gameManager.vanPackagesToDeliver;
        packageDeliveryTime = gameManager.packageDeliveryTime;
        vehicleAI = GetComponent<VehicleAI>();
    }

    void Start() {
        gameManager.SetTargetPositionForScore();
    }

    void Update() {
        if (isPaused) return;

        // If the van has fallen off the tile, delete it
        if (transform.position.y < initialYPosition - 2.0f && !isBeingDestroyed) {
            DestroyVan();
        }
        
        if (gameManager.gameState != GameState.RUNNING) return;
        
        if (currentSegment != vehicleAI.currentTarget.segment) {
            // New segment detecting, getting targets
            currentSegment = vehicleAI.currentTarget.segment;
        }
        
        SetDeliveryTargetsForSegment(currentSegment);

        if (gameManager.gameState == GameState.RUNNING) {

            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0 && !isBeingDestroyed) {
                DestroyVan();
            } else {
                timerMeter.fillAmount = timeLeft / totalTimeToDeliver;
            }
        }
    }

    private void DestroyVan() {
        
        isBeingDestroyed = true;

        Stop();

        if (gameManager.gameState == GameState.RUNNING) {
            GameObject particleSystem = (GameObject)Instantiate(failedDeliveryParticleSystem, transform.position, transform.rotation);
            Destroy(particleSystem, 5f);
            gameManager.explosionSound.Play();

            StartCoroutine(AnimateUndeliveredPackages());
        }
    }

    IEnumerator AnimateUndeliveredPackages() {
        if (gameManager.gameState == GameState.RUNNING) {
            GameObject package;
            package = (GameObject)Instantiate(gameManager.packagePrefab, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation);
            
            for (int i=0; i<packagesToDeliver; i++) {
                StartCoroutine(DoPackageAnimation(i));
                yield return new WaitForSeconds(0.3f);
            }
            Destroy(gameObject);
            isBeingDestroyed = false;
        }
    }

    IEnumerator DoPackageAnimation(int i) {
        if (gameManager.gameState == GameState.RUNNING) {
            GameObject package;
            package = (GameObject)Instantiate(gameManager.packagePrefab, new Vector3(transform.position.x + i, transform.position.y + 0.5f, transform.position.z), transform.rotation);

            Destroy(package, 0.4f);

            if (package != null) {
                package.transform.DOMove(gameManager.targetPositionForScore, 0.3f);
                gameManager.packageCapturedSound.Play();
            }

            gameManager.disruptionPower += gameManager.packageValue;
            gameManager.UpdateDisruptionPower(gameManager.disruptionPower);
        }
            
        yield return null;
    }

    public void Stop() {
        vehicleAI.vehicleStatus = Status.STOP;
    }

    public void Resume() {
        vehicleAI.vehicleStatus = Status.GO;
    }

    public void Spawn(Segment segment, Waypoint waypoint, DeliveryTarget[] _deliveryTargets) {
        if (gameManager.gameState == GameState.RUNNING) {
            SetVanColors();
            
            transform.position = new Vector3(transform.position.x, initialYPosition, transform.position.z);

            vehicleAI = GetComponent<VehicleAI>();
            vehicleAI.van = this;

            vehicleAI.trafficSystem = gameManager.trafficSystem;
            vehicleAI.currentTarget.segment = segment.id;        
            
            // Set the proper rotation based on the entry point
            if (segment.tag == "North Entry Point") { 
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, -90, transform.eulerAngles.z);
            } else if (segment.tag == "West Entry Point") {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
            } else if (segment.tag == "East Entry Point") {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            } else if (segment.tag == "South Entry Point") { 
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90, transform.eulerAngles.z);
            }

            SetDeliveryTargetsForSegment(segment.id);
            currentSegment = segment.id;
        }
    }

    public void SetDeliveryTargetsForSegment(int segmentId) {
        deliveryTargets = gameManager.deliveryTargets.Where(x => x.segment.id == segmentId).OrderBy(x => Random.value).Take(1).ToList();
    }

    private void SetVanColors() {
        // Randomly select a delivery service and set the material color, particle system color, and timer meter color
        DeliveryService _ds = (DeliveryService)Random.Range(0, 3);

        switch (_ds) {
            case DeliveryService.AMAZON:
                GetComponent<Renderer>().material = AmazonMaterial;
                timerMeter.color = new Color32(0, 174, 239, 255);
                failedDeliveryParticleSystem.GetComponent<ParticleSystemRenderer>().sharedMaterial = AmazonMaterial;
                break;
            case DeliveryService.UPS:
                GetComponent<Renderer>().material = UPSMaterial;
                timerMeter.color = new Color32(255, 181, 0, 255);
                failedDeliveryParticleSystem.GetComponent<ParticleSystemRenderer>().sharedMaterial = UPSMaterial;
                break;
            case DeliveryService.DHL:
                GetComponent<Renderer>().material = DHLMaterial;
                timerMeter.color = new Color32(212, 5, 17, 255);
                failedDeliveryParticleSystem.GetComponent<ParticleSystemRenderer>().sharedMaterial = DHLMaterial;
                break;
            case DeliveryService.FEDEX:
                GetComponent<Renderer>().material = FedExMaterial;
                timerMeter.color = new Color32(77, 20, 140, 255);
                failedDeliveryParticleSystem.GetComponent<ParticleSystemRenderer>().sharedMaterial = FedExMaterial;
                break;
        }

    }

    private void SetDeliveryTargets() {
        deliveryTargets = gameManager.deliveryTargets.OrderBy(x => Random.value).Take(packagesToDeliver).ToList();
    }

    public IEnumerator DeliverPackage(GameObject deliveryTarget)
    {
        if (gameManager.gameState == GameState.RUNNING) {
            if (!this.isHacked) {
                vehicleAI.vehicleStatus = Status.STOP;
                GameObject package = gameManager.packagePrefab.GetComponent<Package>().gameObject;
                Instantiate(package, deliveryTarget.transform.position + Vector3.up * 100, deliveryTarget.transform.rotation);
                yield return new WaitForSeconds(packageDeliveryTime);
                // Select a random doorbell sound
                int doorbellSoundIndex = Random.Range(0, gameManager.doorbellSounds.Length);
                gameManager.doorbellSounds[doorbellSoundIndex].Play();
                gameManager.UpdateDeliveredPackageCount(1);
                packagesToDeliver--;
                vehicleAI.vehicleStatus = Status.GO;
            }
        }
    }
  
}
