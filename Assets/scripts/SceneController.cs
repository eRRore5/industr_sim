using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // ����� ��� �������� ��������� �����
    public void LoadNextScene()
    {
        // �������� ������ ������� �����
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        // ��������� ��������� ����� �� �������
        SceneManager.LoadScene(currentIndex + 1);
    }

    // ����� ��� �������� ���������� ����� �� �� ��������
    public void LoadSceneByName(string sceneName)
    {
        // ��������� ����� �� �� ��������
        SceneManager.LoadScene(sceneName);
    }
}