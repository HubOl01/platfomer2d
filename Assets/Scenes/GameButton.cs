using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void ReturnGame()
    {
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene("Menu");
    }
}
