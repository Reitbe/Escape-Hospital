using UnityEngine;

public class DoctorFootstepSound : MonoBehaviour
{
    private SoundManager _soundManager;

    void Start()
    {
        _soundManager = SoundManager.Instance;
    }

    // Doctor�� ���� �ٴڿ� ���� �� �߼Ҹ��� ����Ѵ�
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            _soundManager.PlayFootstep("Doctor");
        }
    }
}
