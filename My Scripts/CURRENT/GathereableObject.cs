using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HighlightPlus;
using StolenLands.Player;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ResourceType { Mineral, Wood, Edible }
public class GathereableObject : MonoBehaviour
{
	public Item requiredItem_1;
	public Item requiredItem_2;

	public string tooltipName;
	public Item itemToGather;
    [Space]
    public int minAmount = 1;
    public int maxAmount = 4;
    public int totalAmount;
    public ResourceType resourceType;
    [Space]
    public float minGatherDistance;

    const int maxGathers = 8; // has to be even number otherwise shit will happen manolooo :):):)
    int currentGather;
    int randomAmount;
    string color;
    public bool isSelected = false;

    public bool playerCloseBy = false;
    
    PlayerUI ui;
    CursorManager cursorManager;
    GameObject player;

	private void Start()
	{
		ui = PlayerUI.instance;
        cursorManager = CursorManager.instance;
        player = GameObject.FindWithTag("Player");
        randomAmount = Random.Range(minAmount, maxAmount);
        totalAmount = randomAmount;
        totalAmount = Mathf.Clamp(totalAmount, 1, maxAmount);
        currentGather = maxGathers;
		switch (resourceType)
		{
			case ResourceType.Mineral:
                color = "#fc9803"; // orange-ish
				break;
			case ResourceType.Wood:
                color = "#3E1900"; // dark brown
                break;
			case ResourceType.Edible:
                color = "#15DB04"; // green
                break;
		}
    }

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, minGatherDistance);
	}
	private void RandomizeAmount()
	{
        randomAmount = Random.Range(minAmount, maxAmount);
        totalAmount = randomAmount;
	}
    public void ApplyGather()
	{
		RandomizeAmount();
        currentGather--;
        transform.DOShakeScale(0.35f, 0.30f, 5, 50f, true);
        if (currentGather == maxGathers / 2 && resourceType != ResourceType.Edible)
		{
            transform.DOScale(0.5f, 0.67f);
		}
        if (currentGather <= 0)
		{
            StartCoroutine(DestroyRoutine());
            Debug.Log("should destroy gather object");
		}
	}
    IEnumerator DestroyRoutine()
	{
        Vector3 a = new Vector3(0, 0, 0);
        PlayerInteract.instance.StopGatherInteraction();
        GetComponentInChildren<SphereCollider>().enabled = false;
        transform.DOShakeScale(0.4f, 0.78f, 6, 30f, true);
        yield return new WaitForSeconds(.2f);
        Destroy(gameObject, 0.7f);
        transform.DOScale(a, 0.6f);
        yield break;
	}
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Vector3.Distance(transform.position, player.transform.position) > minGatherDistance + 10f) return;
        
        isSelected = !isSelected;
        ui.resourceTooltip.gameObject.SetActive(true);
        ui.resourceIcon.sprite = itemToGather.icon;
        ui.resourceNameText.text = itemToGather.itemName;
        ui.resourceTypeText.text = string.Format("<color={0}>{1}</color>", color, resourceType.ToString());
	}
	private void OnMouseEnter()
	{
		if (Vector3.Distance(transform.position, player.transform.position) > minGatherDistance + 10f) return;
		cursorManager.SetGatherCursor();
		var hl = GetComponent<HighlightEffect>();
		hl.highlighted = true;
	}

	public void OnMouseExit()
    {
	    var hl = GetComponent<HighlightEffect>();
	    cursorManager.SetDefaultCursor();
	    hl.highlighted = false;
    }
}