using System.Collections.Generic;
using UnityEngine;

public class syspoints : MonoBehaviour
{
    private int totalScore = 0;
    // ����� ��� ������� �����
    public void CalculateScore(List<ObjectScript> placedObjects)
    {
        foreach (ObjectScript objScript in placedObjects)
        {
            int scoreToAdd = CalculateScoreForObject(objScript, placedObjects);
            totalScore += scoreToAdd;
        }
        Debug.Log("Total Score: " + totalScore);
        // ����� �� ������ �������� ��� UI ��� ����������� ������ �����
    }

    // ����� ��� ������� ����� ��� ���������� �������
    private int CalculateScoreForObject(ObjectScript objScript, List<ObjectScript> placedObjects)
    {
        int score = 0;
        foreach (ObjectScript otherObj in placedObjects)
        {
            if (otherObj != objScript)
            {
                float distance = Vector3.Distance(objScript.transform.position, otherObj.transform.position);
                // ��������� ������ ������� �����
                if (distance <= 5f)
                {
                    score += 2; // ��������� 2 ����, ���� ���������� ������ ��� ����� 5
                }
                else if (distance <= 7f)
                {
                    score -= 2; // �������� 2 ����, ���� ���������� ������ ��� ����� 7
                }
            }
        }
        return score;
    }
}
