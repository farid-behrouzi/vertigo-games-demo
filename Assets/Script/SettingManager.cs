using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour {

    public InputField gridWidth;
    public InputField gridHeight;

    public void saveInputs() {
        int width = int.Parse(gridWidth.text);
        int height = int.Parse(gridHeight.text);
        PlayerPrefs.SetInt("WIDTH", width);
        PlayerPrefs.SetInt("HEIGHT", height);

        SceneManager.LoadScene("Gameplay");
    }
}
