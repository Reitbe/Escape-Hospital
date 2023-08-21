using UnityEngine;

public class MutantFootstepSound : MonoBehaviour
{
    private SoundManager _soundManager;

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = SoundManager.Instance;
    }
    
    // Mutant�� ���� �ٴڿ� ���� �� �߼Ҹ��� ����Ѵ�
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            _soundManager.PlayFootstep("Mutant");
        }
    }
}
