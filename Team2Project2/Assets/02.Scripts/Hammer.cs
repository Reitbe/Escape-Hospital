using System.Collections;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public int UseCount; // �ظ��� �ִ� ��� Ƚ��
    private readonly float _hammerMovingSpeed = 50f; // �ظ� ��� �� �����̴� ������ �ӵ�

    private Vector3 _upPosition = new(0.005f, 0, 0); // ���� ���ø� �� �ظӰ� �����̴� ����
    private Vector3 _downPosition = new(-0.005f, 0, 0); // ���� ���� �� �ظӰ� �����̴� ����

    private void Start()
    {
        UseCount = 10;
    }

    public void HammerUP()
    {
        StartCoroutine(HammerMove(_upPosition));
    }

    public void HammerDown()
    {
        StartCoroutine(HammerMove(_downPosition));
    }

    // ���� �ű� �� �ظ��� ��ġ�� ���� �ö󰡰�, ���� �� ���� ��������
    IEnumerator HammerMove(Vector3 dir)
    {
        int count = 5;
        while(count >= 0){
            count--;
            transform.Translate(_hammerMovingSpeed * Time.deltaTime * dir);
            yield return null;
        }
    }
}
