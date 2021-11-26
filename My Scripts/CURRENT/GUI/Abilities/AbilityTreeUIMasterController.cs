using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AbilityTreeUIMasterController : MonoBehaviour
{
	int currentPage;
	int lastPage;

	[SerializeField] Transform[] pages;
    [SerializeField] Transform previousButton;
    [SerializeField] Transform nextButton;
    void Start()
    {
		lastPage = transform.childCount - 1;
    }

	private void Update()
	{
		CycleThroughPages();
		
		if (pages[0].gameObject.activeSelf)
		{
			previousButton.gameObject.SetActive(false);
		}
		else
		{
			previousButton.gameObject.SetActive(true);
		}
		if(pages[lastPage].gameObject.activeSelf)
		{
			nextButton.gameObject.SetActive(false);
		}
		else
		{
			nextButton.gameObject.SetActive(true);
		}
	}

	#region Pages
	public void SetFirstPage()
	{
		currentPage = 0;
	}
	public void PreviousPage()
	{
		if (pages[0].gameObject.activeSelf) return;
		currentPage--;
	}
	public void NextPage()
	{
		currentPage++;
	}

	private void CycleThroughPages()
	{
		int pageIndex = 0;
		foreach (Transform page in pages)
		{
			if (pageIndex == currentPage)
			{
				page.gameObject.SetActive(true);
			}
			else
			{
				page.gameObject.SetActive(false);
			}
			pageIndex++;
		}
	}
	#endregion

}
