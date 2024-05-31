using UnityEngine;
namespace Features.Utility
{
    public static class UtilityExtensions
    {
        public static CanvasGroup SetVisibility(this CanvasGroup canvasGroup, bool visible)
        {
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
            return canvasGroup;
        }
    }
}