using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class RectToCameraFitter : MonoBehaviour
    {
        [SerializeField] 
        private RectTransform _rectTransform;

        [SerializeField] 
        private Camera _camera;

        [SerializeField] 
        private UnityEvent _onRectUpdated;
    
        private void OnEnable() 
        {
            var a = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            var b = _camera.ScreenToWorldPoint(new Vector2(0, 0));

            _rectTransform.position = Vector3.zero;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (a.x - b.x));
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (a.y - b.y));
            
            _onRectUpdated?.Invoke();
        }
    }
}