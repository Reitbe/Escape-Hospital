using UnityEngine;

public class ZombieFootstepSound : MonoBehaviour
{
    private SoundManager _soundManager;

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = SoundManager.Instance;
    }

    // Zombie�� ���� �ٴڿ� ���� �� �߼Ҹ��� ����Ѵ�
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            _soundManager.PlayFootstep("Zombie");
        }
    }
}
