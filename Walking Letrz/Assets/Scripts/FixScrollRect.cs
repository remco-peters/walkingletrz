using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class FixScrollRect: MyMonoBehaviour, IBeginDragHandler,  IDragHandler, IEndDragHandler, IScrollHandler
    {
        public ScrollRect MainScroll;
 
 
        public void OnBeginDrag(PointerEventData eventData)
        {
            MainScroll.OnBeginDrag(eventData);
        }
 
 
        public void OnDrag(PointerEventData eventData)
        {
            MainScroll.OnDrag(eventData);
        }
 
        public void OnEndDrag(PointerEventData eventData)
        {
            MainScroll.OnEndDrag(eventData);
        }
 
 
        public void OnScroll(PointerEventData data)
        {
            MainScroll.OnScroll(data);
        }
    }
}
