using Cysharp.Threading.Tasks;
using Maze.MazeService;
using UnityEngine;

namespace Maze.Components
{
    public class CameraSizeFitterComponent: MonoBehaviour, IComponent
    {
        [SerializeField] 
        private Camera _camera;

        [SerializeField] 
        private RectTransform _target;

        [SerializeField] 
        private float _padding;

        public UniTask Initialize(Context context)
        {
            context.Camera = _camera;
            
            var rect = _target.rect;
            
            // portrait
            var orthographicSizePortrait = rect.height / 2;
            
            // landscape
            var ratio = Screen.height / (float)Screen.width;
            var orthographicSizeLandscape = ratio * rect.width / 2;
            
            // The orthographicSize is half the size of the vertical viewing volume.
            _camera.orthographicSize = _padding + Mathf.Max(orthographicSizePortrait, orthographicSizeLandscape);
            
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}