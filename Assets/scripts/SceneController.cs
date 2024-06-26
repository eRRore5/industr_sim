using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
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
}