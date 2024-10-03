using System.Collections.Generic;
using Core.Element;
using Core.Element.Config;
using Core.Element.Factory;
using Core.Session;
using DG.Tweening;
using Services;
using Unity.Mathematics;
using UnityEngine;

namespace Core
{
    public sealed class GridRecalculationController : IGridRecalculationController
    {
        private readonly SessionController _sessionController;
        private readonly IGridGameElementFactory _elementFactory;
        private readonly IRenderOrderHelper _renderOrderHelper;

        private readonly List<AfterMergeData> _elementsToMove = new List<AfterMergeData>();
        private readonly List<GridGameElement> _elementsInColumns = new List<GridGameElement>();
        private readonly ElementConfig _elementConfig;

        public GridRecalculationController(
            SessionController sessionController,
            IGridGameElementFactory elementFactory,
            IRenderOrderHelper renderOrderHelper,
            IConfigProvider configs)
        {
            _sessionController = sessionController;
            _elementFactory = elementFactory;
            _renderOrderHelper = renderOrderHelper;
            _elementConfig = configs.GetConfig<ElementConfig>();
        }
        
        public void RecalculateGrid()
        {
            _elementsToMove.Clear();
            
            for (int column = 0; column < _sessionController.Elements[0].Length; column++)
            {
                if(!HasAnyElementInColumns(column)) continue;
                if(!AnyNotHold(column)) continue;
                if(!NotStableInColumn(column)) continue;
                
                FillElementsInColumn(column);
                FillAfterMergeData(column);
            }
            
            MoveElementsToNewPlaces();
            
            _sessionController.ClearElements();
            _sessionController.FillElements(_elementFactory.GetAllActiveElements());
            
            Debug.Log(_sessionController);
        }
        
        private bool HasAnyElementInColumns(int column)
        {
            bool hasAnyElementInColumns = false;

            for (int k = 0; k < _sessionController.Elements.Length && !hasAnyElementInColumns; k++)
            {
                GridGameElement element = _sessionController.Elements[k][column];
                    
                if (element != null)
                {
                    hasAnyElementInColumns = true;
                }
            }

            return hasAnyElementInColumns;
        }
        
        private bool AnyNotHold(int column)
        {
            bool anyNotHold = false;
            for (int k = 0; k < _sessionController.Elements.Length && !anyNotHold; k++)
            {
                GridGameElement element = _sessionController.Elements[k][column];
                    
                if (element == null) anyNotHold = true;
            }

            return anyNotHold;
        }
        
        private bool NotStableInColumn(int column)
        {
            bool notStableInColumn = false;

            if (_sessionController.Elements[0][column] == null)
            {
                notStableInColumn = true;
            }
            else
            {
                for (int k = 1; k < _sessionController.Elements.Length && !notStableInColumn; k++)
                {
                    GridGameElement element = _sessionController.Elements[k][column];

                    if (element != null && _sessionController.Elements[k - 1][column] == null)
                    {
                        notStableInColumn = true;
                    }
                }
            }

            return notStableInColumn;
        }
        
        private void FillElementsInColumn(int column)
        {
            _elementsInColumns.Clear();

            for (int k = 0; k < _sessionController.Elements.Length; k++)
            {
                GridGameElement element = _sessionController.Elements[k][column];
                    
                if (element != null)
                {
                    _elementsInColumns.Add(element);
                }
            }
        }
        
        private void FillAfterMergeData(int column)
        {
            int2 index = new int2(0, column);

            for (int c = 0; c < _elementsInColumns.Count; c++)
            {
                _elementsToMove.Add(new AfterMergeData()
                {
                    Element = _elementsInColumns[c],
                    Index = index,
                    PreviousIndex = _elementsInColumns[c].GridIndex,
                });

                index += new int2(1, 0);
            }
        }
        
        private void MoveElementsToNewPlaces()
        {
            foreach (AfterMergeData data in _elementsToMove)
            {
                int2 elementIndex = data.Index;
                int2 previousIndex = data.PreviousIndex;
                GridGameElement element = data.Element;
                float moveTime = Mathf.Abs(previousIndex.x - elementIndex.x) * _elementConfig.MoveAcrossGridSpeed;
                _sessionController.Elements[elementIndex.x][elementIndex.y] = element;
                Vector3 position = _sessionController.Positions[elementIndex.x][elementIndex.y];
                element.SetGridIndex(elementIndex);
                element.transform.DOMove(position, moveTime);
                element.SetRenderOrder(_renderOrderHelper.GetRenderOrder(elementIndex.x, elementIndex.y));
            }
        }
    }
}