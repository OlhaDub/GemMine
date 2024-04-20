using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenScene : MonoBehaviour
{
    // Назва сцени, яку хочемо відкрити
    public string sceneName;

    public void Open()
    {
        // Перевіряємо, чи існує сцена з такою назвою
        if (SceneManager.GetSceneByName(sceneName) != null)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
