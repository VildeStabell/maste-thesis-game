using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalibrationController : MonoBehaviour {

    const float maxInput = 1.0f;
    public const float readSpeed = 0.2f;
    public const float maxCadence = 50f;

    [Range(0, maxInput)]
    public float cadence;

    public MasterThesisGameInput input;
    public TMP_Text LoadingPercentageText;
    public Slider loadingBar;
    public TMP_Text instructionText;

    private InputAction cadenceInput;
    private PlayerInput playerInput; // Needed to check control scheme
    private float sumCadence = 0;
    private float startTime = 0;
    private float usedTime = 0;


    private void Awake() {
        input = new MasterThesisGameInput();
        playerInput = gameObject.GetComponent<PlayerInput>();
    }

    private void OnEnable() {
        cadenceInput = input.Player.Cadence;
        cadenceInput.Enable();
    }

    private void OnDisable() {
        cadenceInput = input.Player.Cadence;
    }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(readCadence(readSpeed));
    }

    // Update is called once per frame
    void Update() {
        if (sumCadence > maxCadence && usedTime == 0) {
            //eqCad is a number between 0.3-0.9, defined by the prosentage caluclated by sumCadence/usedTime*2
            //This is because the max of sumCadence/usedTime = 50

            usedTime = Time.time - startTime;
            float eqCad = Mathf.Floor((sumCadence / usedTime) * 20 * 60) / 10000 + 0.30f;
            SessionController.sessionCtrl.setEqCadence(eqCad);
            SceneManager.LoadScene(sceneName: "MainMenuScene");
        }

    }

    private IEnumerator readCadence(float seconds) {
        yield return new WaitForSeconds(seconds);

        if (sumCadence < maxCadence) {
            if (playerInput.currentControlScheme == "Bike") {
                Vector2 cadenceVector = cadenceInput.ReadValue<Vector2>();
                float absX = Mathf.Abs(cadenceVector.x);
                float absY = Mathf.Abs(cadenceVector.y);
                cadence = Mathf.Max(absX, absY) - Mathf.Min(absY, absX);
            }

            sumCadence += cadence;

            if (sumCadence > 0 && startTime == 0) {
                startTime = Time.time;
            }

            if (startTime != 0) {
                if (cadence <= 0.40f) {
                    instructionText.text = "Speed up!";
                } else if (cadence > 0.40f && cadence < 0.97f) {
                    instructionText.text = "Well done! Keep going";
                } else {
                    instructionText.text = "Increase resistance!";
                }
            }

            float progress = Mathf.Clamp01(sumCadence / maxCadence / .9f);
            loadingBar.value = progress;
            LoadingPercentageText.text = System.String.Format("{0:F0}%", progress * 100f); ;


            StartCoroutine(readCadence(seconds));
        }
    }
}
