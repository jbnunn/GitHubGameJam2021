using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{

    public int navigationBugCost = 10;
    public int redLightHackCost = 20;
    public int quarantineCost = 40;
    public float hackedLightsDuration = 20.0f;

    private GameManager gameManager;
    private GameState gameState;

    void Start() {
        gameManager = GetComponent<GameManager>();
        gameState = GameState.RUNNING;
    }

    public bool PurchaseRedLightHack() {
        if (gameState == GameState.RUNNING && !gameManager.playerIsPurchasing) {
            gameManager.playerIsPurchasing = true;
            if (gameManager.disruptionPower >= redLightHackCost) {
                gameManager.disruptionPower -= redLightHackCost;
                gameManager.UpdateDisruptionPower(gameManager.disruptionPower); 
                gameManager.playerIsPurchasing = false;
                return true;
            }
        }

        return false;
    }

    public bool PurchaseNavigationBug() {
        if (gameState == GameState.RUNNING && !gameManager.playerIsPurchasing) {
            gameManager.playerIsPurchasing = true;
            if (gameManager.disruptionPower >= navigationBugCost) {
                gameManager.disruptionPower -= navigationBugCost;
                gameManager.UpdateDisruptionPower(gameManager.disruptionPower); 
                gameManager.playerIsPurchasing = false;
                return true;
            }
        }

        return false;
    }

    public bool PurchaseQuarantine() {
        if (gameState == GameState.RUNNING && !gameManager.playerIsPurchasing) {
            gameManager.playerIsPurchasing = true;
            if (gameManager.disruptionPower >= quarantineCost) {
                gameManager.disruptionPower -= quarantineCost;
                gameManager.UpdateDisruptionPower(gameManager.disruptionPower);
                gameManager.playerIsPurchasing = false;
                return true;
            }
        }

        return false;
    }
}
