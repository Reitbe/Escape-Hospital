using UnityEngine;

public class ZombieScreamSound : MonoBehaviour
{
    private SoundManager _soundManager;

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = SoundManager.Instance;
        Invoke("ZombieScream", 4.0f);
    }

    // ���� ���ƴٴϸ鼭 ������ �������� �Ҹ��� ����
    public void ZombieScream()
    {
        _soundManager.PlayZombieScream();
        Invoke("ZombieScream", Random.Range(4, 8));
    }
}
