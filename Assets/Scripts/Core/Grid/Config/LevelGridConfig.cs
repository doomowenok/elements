using System.Collections.Generic;
using Core.Element;
using UnityEngine;

namespace Core.Grid.Config
{
    [CreateAssetMenu(fileName = nameof(LevelGridConfig), menuName = "Configs/Core/Level Grid")]
    public sealed class LevelGridConfig : ScriptableObject
    {
        [SerializeField] private List<LevelGridRowData> _levelGridRows;

        private ElementType[][] _levelGrid;

        public ElementType[][] LevelGrid
        {
            get
            {
                _levelGrid = new ElementType[_levelGridRows.Count][];

                for (int i = 0; i < _levelGrid.Length; i++)
                {
                    _levelGrid[i] = new ElementType[_levelGridRows[i].Elements.Count];
                    
                    for (int k = 0; k < _levelGrid[i].Length; k++)
                    {
                        _levelGrid[i][k] = _levelGridRows[i].Elements[k];
                    }
                }

                return _levelGrid;
            }
        }
    }
}