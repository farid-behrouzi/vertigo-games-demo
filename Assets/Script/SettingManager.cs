using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour {

    public InputField gridWidth;
    public InputField gridHeight;
    int width = 0;
    int height = 0;

    public void saveInputs() {
        if (gridWidth.text != "" && gridHeight.text != "")
        {
            width = int.Parse(gridWidth.text);
            height = int.Parse(gridHeight.text);
            PlayerPrefs.SetInt("WIDTH", width);
            PlayerPrefs.SetInt("HEIGHT", height);
        }
        else
        {
            width = 8;
            height = 9;
            PlayerPrefs.SetInt("WIDTH", width);
            PlayerPrefs.SetInt("HEIGHT", height);
        }


        SceneManager.LoadScene("Gameplay");
    }
}
