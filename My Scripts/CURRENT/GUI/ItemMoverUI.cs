using UnityEngine;
using StolenLands.Abilities;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemMoverUI : MonoBehaviour
{
	#region Singleton
	public static ItemMoverUI instance;
	private void Awake()
	{
        instance = this;
	}
    #endregion
    public Item movingItem;
	public Ability movingAbility;
    public bool hasAbilityMoving;
    public bool hasItemMoving;

    Image movingIcon;
    private PlayerUI ui;
    private Inventory inv;

	private void Start()
	{
        ui = PlayerUI.instance;
        inv = Inventory.instance;
		movingIcon = GetComponent<Image>();
        movingIcon.color = Color.clear;
	}

    void Update()
    {
        transform.position = Input.mousePosition;
        if(movingAbility != null)
		{
            // size is x;
            movingIcon.sprite = movingAbility.abilityImage;
            movingIcon.color = Color.white;
            if (!EventSystem.current.IsPointerOverGameObject())
            {
	            if (Input.GetMouseButtonDown(0))
	            {
		            RemoveFromMover();
	            }
            }

		}
        if(movingItem != null)
		{
			// size is y;
            movingIcon.sprite = movingItem.icon;
            movingIcon.color = Color.white;
            if (!EventSystem.current.IsPointerOverGameObject() && !ui.itemDeletePopup.activeInHierarchy)
            {
	            if (Input.GetMouseButtonDown(0))
	            {
		            ui.itemDeletePopup.SetActive(true);
	            }
            }
		}
    }

    public void DeleteItem() // UI BUTTON
    {
	    if (movingItem != null)
	    {
		    if (movingItem.itemType == ItemType.Quest)
		    {
			    ui.InstantiateWarning("Can't delete quest items");
			    ui.itemDeletePopup.SetActive(false);
			    return;
		    }
		    inv.Remove(movingItem, movingItem.amount);
		    RemoveFromMover();
		    ui.itemDeletePopup.SetActive(false);
		    return;
	    }
	    ui.InstantiateWarning("There is nothing to delete");
	    ui.itemDeletePopup.SetActive(false);
    }

    public void DontDeleteItem() // UI BUTTON
    {
	    ui.itemDeletePopup.SetActive(false);
	    return;
    }
     
    public Ability GetMovableAbility()
	{
        if(hasAbilityMoving)
		{
            return movingAbility;
        }
        return null;
	}

    public Item GetMoveableItem()
	{
        if(hasItemMoving)
		{
            return movingItem;
		}
        return null;
	}

    public void AssignItemToMover(Item itemToMove, int amount)
	{
        hasItemMoving = true;
        movingItem = itemToMove;
        movingItem.amount = amount;
        movingIcon.color = Color.white;
        movingIcon.sprite = itemToMove.icon;
	}

    public void AssignAbilityToMover(Ability abilityToMove)
    {
        hasAbilityMoving = true;
        movingAbility = abilityToMove;
    }

    public void RemoveFromMover()
	{
        if(movingAbility != null && movingItem == null)
		{
            hasAbilityMoving = false;
            movingIcon.color = Color.clear;
            movingIcon.sprite = null;
            movingAbility = null;
        }
        if(movingItem != null && movingAbility == null)
		{
            hasItemMoving = false;
            movingIcon.color = Color.clear;
            movingIcon.sprite = null;
            movingItem = null;
        }
	}
}
