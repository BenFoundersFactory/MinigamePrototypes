using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MarkUpPlayer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

    [SerializeField] private Transform prevParent;
    [SerializeField] private Transform dragParent;

    private Collider2D boxCollider;
    private CanvasGroup canvasGroup;

    private int index;

    void Awake() {
        boxCollider = GetComponent<Collider2D>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        prevParent = transform.parent;
        dragParent = transform.parent.parent.parent;

        index = transform.GetSiblingIndex();
        transform.SetParent(dragParent, false);

        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 1f;
        transform.position = screenPoint;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 1f;
        transform.position = screenPoint;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.pointerEnter.gameObject.tag == "TeamSlot") {
            Destroy(gameObject);
        } else {
            transform.SetParent(prevParent, false);
            transform.SetSiblingIndex(index);
            canvasGroup.blocksRaycasts = true;
        }
    }

}
