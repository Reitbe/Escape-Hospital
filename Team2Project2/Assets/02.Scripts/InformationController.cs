using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine.UI;
using UnityEngine;

public class InformationController : MonoBehaviour
{
    private bool isInfoControl;

    [SerializeField]
    private Text actionText;
    [SerializeField]
    private RaycastInfo raycastInfo;
    [SerializeField]
    private ActionController actionController;

    void Start()
    {
        isInfoControl = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (raycastInfo.isRaycastHitChanged)
        {
            // ���� �浹X -> �浹O ����Ǿ��� ��
            if (raycastInfo.isPresentRaycastHit)
            {
                isInfoControl = true;
                Debug.Log("���� �浹X -> ���� �浹O");
                Debug.Log("���� �浹 ��ü�� �����. Previous = " + raycastInfo.previousObject + ", Present = " + raycastInfo.presentObject);
                InfoControl();
            }
            // ���� �浹O -> �浹X ����Ǿ��� ��
            else
            {
                isInfoControl = false;
                InfoDisappear();
                if (raycastInfo.previousObject.GetComponent<Outline>() != null)
                {
                    OutlineDisappear(raycastInfo.previousObject);
                }
                raycastInfo.previousObject = null;

                Debug.Log("���� �浹O -> ���� �浹X");
                Debug.Log("���� UI, �ܰ���, previousObject �ʱ�ȭ");
            }
        }

        // ������ ���� �浹�ϰų�, �浹���� ���¿��� ��ü�� ����Ǿ��� �� ȣ��
        if (isInfoControl && raycastInfo.isRaycastHitObjectChanged)
        {
            Debug.Log("���� �浹 ��ü�� �����. Previous = " + raycastInfo.previousObject + ", Present = " + raycastInfo.presentObject);
            InfoControl();
        }
    }

    // ���� UI �� �ܰ��� ����
    private void InfoControl()
    {
        // ���� UI ��Ȱ��ȭ �� ���� ������Ʈ �ܰ��� ����
        InfoDisappear();
        if (raycastInfo.previousObject != null && raycastInfo.previousObject.GetComponent<Outline>() != null)
        {
            OutlineDisappear(raycastInfo.previousObject);
        }

        // �±׿� ���� ���� UI �� �ܰ��� ����
        switch (raycastInfo.presentObject.tag)
        {
            case "Item":
                InfoAppear(raycastInfo.presentObject.GetComponent<ItemPickUp>().item.itemName + "  ȹ��" + "<color=yellow>" + " (E)" + "</color>");
                OutlineAppear(raycastInfo.presentObject, Color.yellow, 10.0f);
                break;

            case "MovingWall":
                if (actionController.isMoveWallActivated)
                {
                    InfoAppear("�ű��" + "<color=yellow>" + " (R)" + "</color>");
                    OutlineAppear(raycastInfo.presentObject, Color.green, 10.0f);
                }
                break;

            case "FixedWall":
                if (actionController.isMoveWallActivated)
                {
                    InfoAppear("<color=red>" + "�̵� �Ұ�" + "</color>");
                    OutlineAppear(raycastInfo.presentObject, Color.red, 10.0f);
                }
                break;

            case "MovingTemp":
                if (actionController.isMovingWall)
                {
                    InfoAppear("��������");
                    OutlineAppear(raycastInfo.presentObject, Color.red, 10.0f);
                }
                break;

            case "Player":
                break;

            default:
                break;
        }
    }

    // ���� UI Ȱ��ȭ
    private void InfoAppear(string txt)
    {
        actionText.text = txt;
        actionText.gameObject.SetActive(true);
    }

    // ���� UI ��Ȱ��ȭ
    private void InfoDisappear()
    {
        actionText.text = "";
        actionText.gameObject.SetActive(false);
    }

    // �ܰ��� Ȱ��ȭ - Outline ������Ʈ ����
    private void OutlineAppear(GameObject outlineObject, Color color, float width)
    {
        outlineObject.AddComponent<Outline>();
        outlineObject.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
        outlineObject.GetComponent<Outline>().OutlineColor = color;
        outlineObject.GetComponent<Outline>().OutlineWidth = width;
    }

    // �ܰ��� ��Ȱ��ȭ - Outline ������Ʈ �ı�
    private void OutlineDisappear(GameObject outlineObject)
    {
        Destroy(outlineObject.GetComponent<Outline>());
    }
}
