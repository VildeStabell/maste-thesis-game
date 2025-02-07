using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public TMP_Text title;
    public TMP_Text description;
    public GameObject mainMenuButtons;
    public GameObject gameModeDetails;
    public Button primaryButton;
    public Button playButton;

    private GameModeEnum chosenGameMode;

    public void balanceModeButtonPressed() {
        chosenGameMode = GameModeEnum.BalanceMode;
        openGameDetails(new BalanceMode(null));
    }

    public void shapesModeButtonPressed() {
        chosenGameMode = GameModeEnum.ShapesMode;
        openGameDetails(new ShapesMode(null));
    }

    public void marbleModeButtonPressed() {
        chosenGameMode = GameModeEnum.MarbleMode;
        openGameDetails(new MarbleMode(null));
    }

    public void backButtonPressed() {
        gameModeDetails.SetActive(false);
        mainMenuButtons.SetActive(true);
        primaryButton.Select();
    }

    public void Start() {
        gameModeDetails.SetActive(false);
        mainMenuButtons.SetActive(true);
        AudioManager.instance.InitializeMusic(FMODEvents.instance.menuMusic);
    }

    public void PlayButtonPressed() {
        switch (chosenGameMode) {
            case GameModeEnum.BalanceMode:
                SceneManager.LoadScene(sceneName: "BalanceScene");
                break;
            case GameModeEnum.ShapesMode:
                SceneManager.LoadScene(sceneName: "ShapesScene");
                break;
            case GameModeEnum.MarbleMode:
                SceneManager.LoadScene(sceneName: "MarbleScene");
                break;
        }
    }

    public void QuitGame() {
        Application.Quit();
        Debug.Log("Game is exiting"); // Will not actually quit in editor
    }

    // ---- Utility functions ----

    void openGameDetails(GameMode gameMode) {
        mainMenuButtons.SetActive(false);
        gameModeDetails.SetActive(true);
        title.text = gameMode.getName();
        description.text = gameMode.getDescription();
        playButton.Select();
    }
}
