using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class play : MonoBehaviour
{
    // Метод для загрузки следующей сцены
    public void LoadNextScene()
    {
        // Получаем индекс текущей сцены
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        // Загружаем следующую сцену по порядку
        SceneManager.LoadScene(currentIndex + 1);
    }

    // Метод для загрузки конкретной сцены по ее названию
    public void LoadSceneByName(string sceneName)
    {
        // Загружаем сцену по ее названию
        SceneManager.LoadScene(sceneName);
    }
    public void LoadPrevScene()
    {
        // Получаем индекс текущей сцены
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        // Загружаем следующую сцену по порядку
        SceneManager.LoadScene(currentIndex - 1);
    }
}