using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class play : MonoBehaviour
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
    public void LoadPrevScene()
    {
        // �������� ������ ������� �����
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        // ��������� ��������� ����� �� �������
        SceneManager.LoadScene(currentIndex - 1);
    }
}