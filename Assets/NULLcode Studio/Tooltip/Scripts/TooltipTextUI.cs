// NULLcode Studio © 2015
// null-code.ru

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TooltipTextUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	
	public string text;
	
	void IPointerEnterHandler.OnPointerEnter(PointerEventData e)
	{
		Tooltip.text = text;
		Tooltip.isUI = true;
	}
	
	void IPointerExitHandler.OnPointerExit(PointerEventData e)
	{
		Tooltip.isUI = false;
	}
}
