using UnityEngine;

public class RaycastInfo : MonoBehaviour
{
    public bool IsPreviousRaycastHit; // ���� �����ӿ��� ������ �浹�ߴ��� ����
    public bool IsPresentRaycastHit; // ���� �����ӿ��� ������ �浹�ߴ��� ����

    public bool IsRaycastHitChanged; // ���� �浹-���浹 ���� ��ȯ ����
    public bool IsRaycastHitObjectChanged; // ������ ���ο� ������Ʈ�� �浹�ߴ��� ����

    public GameObject PreviousObject; // ���� �����ӿ��� ������ �浹�� ������Ʈ
    public GameObject PresentObject; // ���� �����ӿ��� ������ �浹�� ������Ʈ
    public GameObject PresentObjectParent; // ���� �����ӿ��� ������ �浹�� ������Ʈ�� �θ� ������Ʈ
    
    public RaycastHit HitInfo; // ���� �浹 ����

    [SerializeField]
    private float _rayDistance; // ������ ����

    // Start is called before the first frame update
    void Start()
    {
        IsPresentRaycastHit = false;
        IsPreviousRaycastHit = false;
        IsRaycastHitChanged = false;
        IsRaycastHitObjectChanged = false;

        _rayDistance = 3.0f;

        PreviousObject = null;
        PresentObject = null;
        PresentObjectParent = null;
    }

    // Update is called once per frame
    void Update()
    {
        ShootRay();
        CheckRaycastHitChanged();
        if (IsPresentRaycastHit) // �߻�� ������ �浹�� ��쿡�� ����
        {
            RaycastHitObject();
            CheckRaycastHitObjectChanged();
        }
    }

    // Raycast�� ���� 
    private void ShootRay()
    {
        IsPreviousRaycastHit = IsPresentRaycastHit;
        IsPresentRaycastHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out HitInfo, _rayDistance);
    }

    // ���� �浹-���浹 ���� ��ȯ Ȯ��
    private void CheckRaycastHitChanged()
    {
        if (IsPreviousRaycastHit != IsPresentRaycastHit)
        {
            IsRaycastHitChanged = true;   
        }
        else
        {
            IsRaycastHitChanged = false;
        }
    }

    // ������ �浹�� ������Ʈ, �� ������Ʈ�� �θ� ������Ʈ Ȯ��
    private void RaycastHitObject()
    {
        PreviousObject = PresentObject;
        PresentObject = HitInfo.transform.gameObject;

        // �θ� ������Ʈ�� �������� �ʴ� ���
        if (PresentObject.transform.parent == null)
        {
            PresentObjectParent = null;
        }
        // �θ� ������Ʈ�� ���������� �±׶� �������� ���� ���(�� �̵� ����X)
        else if (PresentObject.transform.parent.gameObject.CompareTag("Untagged"))
        {
            PresentObjectParent = PresentObject.transform.parent.gameObject;
        }
        // �θ� ������Ʈ�� �����ϸ� �±ױ��� ������ ���(�� �̵� ����O)
        else
        {
            PresentObject = PresentObject.transform.parent.gameObject;
            //Debug.Log("Parent->Present, PresentObject tag = " + PresentObject.tag);
            if (PresentObject.transform.parent != null) PresentObjectParent = PresentObject.transform.parent.gameObject;
        }
        //Debug.Log("Present : " + PresentObject + ", Parent : " + PresentObjectParent);
    }

    // ������ ���ο� ������Ʈ�� �浹�ߴ��� Ȯ��
    private void CheckRaycastHitObjectChanged()
    {
        if (PreviousObject != PresentObject)
        {
            IsRaycastHitObjectChanged = true;
        }
        else
        {
            IsRaycastHitObjectChanged = false;
        }
    }
}
