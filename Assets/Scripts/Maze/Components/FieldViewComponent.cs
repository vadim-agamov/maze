using Configs;
using Cysharp.Threading.Tasks;
using Maze.MazeService;
using UnityEngine;

namespace Maze.Components
{
    public class FieldViewComponent : MonoBehaviour, IComponent
    {
        [SerializeField] 
        private LevelConfig _levelConfig;

        [SerializeField] 
        private CellView _cell;

        [SerializeField]
        private RectTransform _rectTransform;
        
        private float _cellSize = 1.5f;

        public UniTask Initialize(Context context)
        {
            var rows = _levelConfig.Cells.GetLength(0);
            var cols = _levelConfig.Cells.GetLength(1);
            
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cols * _cellSize);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rows * _cellSize);
            var rect = _rectTransform.rect;
            var startCell = new Vector2(_cellSize * 0.5f + rect.xMin, -_cellSize * 0.5f + rect.yMax);

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    var configCell = _levelConfig.Cells[r, c];
                    var cell = Instantiate(_cell, transform, true);
                    cell.transform.localPosition = startCell + new Vector2(_cellSize * c, _cellSize * -r);
                    cell.Setup(configCell, r, c);
                }
            }

            context.LevelConfig = _levelConfig;
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}