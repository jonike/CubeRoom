using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sorumi.Util;

namespace Sorumi.UI
{

    [RequireComponent(typeof(ScrollRect))]
    public class CellView : MonoBehaviour
    {
        public enum Direction
        {
            Horizontal,
            Vertical
        }

        public Direction direction;
        public Cell cell;
        public float cellWidth;
        public float cellHeight;
        public float minHorizontalSpace = 0.0f;
        public float minVerticalSpace = 0.0f;

        public float maxHorizontalPadding = 0.0f;
        public float maxVerticalPadding = 0.0f;

        private ScrollRect scrollRect;

        private Vector2 scrollSize;
        private Vector2 contentSize;

        private float itemSpace = 0.0f;
        private float lineSpace = 0.0f;

        private float itemPadding = 0.0f;
        private float linePadding = 0.0f;

        private int countOfCell;
        private int countPreLine;
        private int countOfLine;

        private GameObject poolGO;
        private ObjectPool<Cell> cellPool;
        private Dictionary<int, Cell> dictionary;

        private int startIndex = -1;
        private int endIndex = -1;


        public delegate int IntDelegate();
        public delegate void CellIntAction(Cell cell, int i);
        public IntDelegate CountOfCell;
        public CellIntAction CellAtIndex;

        public void Init()
        {
            PrepareLayout();
        }

        private void PrepareLayout()
        {
            scrollRect = GetComponent<ScrollRect>();
            Rect sr = scrollRect.GetComponent<RectTransform>().rect;
            scrollSize = new Vector2(sr.width, sr.height);
            Rect cr = scrollRect.content.GetComponent<RectTransform>().rect;
            contentSize = new Vector2(cr.width, cr.height);

            // item padding
            float horizontalPadding = maxHorizontalPadding;
            float verticalPadding = maxVerticalPadding;
            if (horizontalPadding * 2 > (scrollSize.x - cellWidth))
            {
                horizontalPadding = (scrollSize.x - cellWidth) / 2;
                horizontalPadding = horizontalPadding < 0 ? 0 : horizontalPadding;
            }
            if (verticalPadding * 2 > (scrollSize.y - cellHeight))
            {
                verticalPadding = (scrollSize.y - cellHeight) / 2;
                verticalPadding = verticalPadding < 0 ? 0 : verticalPadding;
            }

            if (direction == Direction.Horizontal)
            {
                itemPadding = verticalPadding;
                linePadding = horizontalPadding;
            }
            else if (direction == Direction.Vertical)
            {
                itemPadding = horizontalPadding;
                linePadding = verticalPadding;
            }


            if (direction == Direction.Horizontal)
            {
                countPreLine = (int)Mathf.Floor((scrollSize.y - cellHeight - itemPadding * 2) / (cellHeight + minVerticalSpace));
            }
            else if (direction == Direction.Vertical)
            {
                countPreLine = (int)Mathf.Floor((scrollSize.x - cellWidth - itemPadding * 2) / (cellWidth + minHorizontalSpace));
            }

            
            countPreLine = countPreLine + 1;
            

            cellPool = new ObjectPool<Cell>(CreateCell, ResetCell);
            poolGO = new GameObject("PoolGameObject");
            poolGO.transform.position = Vector3.zero;
            poolGO.SetActive(false);
            poolGO.transform.SetParent(scrollRect.transform);

            dictionary = new Dictionary<int, Cell>();
        }

        private void RefreshLayout()
        {
            countOfCell = CountOfCell != null ? CountOfCell() : 0;
            countOfLine = countOfCell / countPreLine + (countOfCell % countPreLine == 0 ? 0 : 1);

            // item space
            if (countPreLine == 1)
            {
                itemSpace = 0;
            }
            else if (direction == Direction.Horizontal)
            {
                itemSpace = (contentSize.y - itemPadding * 2 - countPreLine * cellHeight) / (countPreLine - 1);
            }
            else if (direction == Direction.Vertical)
            {
                itemSpace = (contentSize.x - itemPadding * 2 - countPreLine * cellWidth) / (countPreLine - 1);
            }

            // line space
            if (direction == Direction.Horizontal)
            {
                lineSpace = itemSpace > minHorizontalSpace ? itemSpace : minHorizontalSpace;
            }
            else if (direction == Direction.Vertical)
            {
                lineSpace = itemSpace > minVerticalSpace ? itemSpace : minVerticalSpace;
            }

            // Renew contentSize
            if (direction == Direction.Horizontal)
            {
                contentSize.y = scrollSize.y;
                contentSize.x = countOfLine * (cellWidth + lineSpace) - lineSpace + linePadding * 2;
                contentSize.x = contentSize.x < 0 ? 0 : contentSize.x;
                scrollRect.content.sizeDelta = contentSize;
                scrollRect.content.anchoredPosition = Vector3.zero;
            }
            else if (direction == Direction.Vertical)
            {
                contentSize.x = scrollSize.x;
                contentSize.y = countOfLine * (cellHeight + lineSpace) - lineSpace + linePadding * 2;
                contentSize.y = contentSize.y < 0 ? 0 : contentSize.y;
                scrollRect.content.sizeDelta = contentSize;
                scrollRect.content.anchoredPosition = Vector3.zero;
            }
        }

        private Cell CreateCell()
        {
            GameObject cellObject = GameObject.Instantiate(cell.gameObject, Vector3.zero, Quaternion.identity);
            cellObject.transform.SetParent(poolGO.transform);
            Cell objectCell = cellObject.GetComponent<Cell>();
            objectCell.Init();
            return objectCell;
        }

        private void ResetCell(Cell cell)
        {
            cell.transform.SetParent(poolGO.transform);
        }

        public void Refresh()
        {
            RefreshLayout();
            Rect rect = new Rect(0, 0, scrollSize.x, scrollSize.y);
            UpdateInRect(rect);
        }

        public void OnValueChanged(Vector2 value)
        {
            float deltaY = scrollSize.y - contentSize.y;
            float startY = (value.y - 1) * deltaY;

            float deltaX = scrollSize.x - contentSize.x;
            float startX = -value.x * deltaX;

            Rect rect = new Rect(startX, startY, scrollSize.x, scrollSize.y);

            UpdateInRect(rect);
        }

        private void UpdateInRect(Rect rect)
        {
            if (countOfCell == 0)
                return;

            IndexInRect(rect, out startIndex, out endIndex);

            List<int> deleteList = new List<int>();
            foreach (KeyValuePair<int, Cell> pair in dictionary)
            {
                if (pair.Key < startIndex || pair.Key > endIndex)
                {
                    deleteList.Add(pair.Key);
                }
            }

            foreach (int index in deleteList)
            {
                cellPool.PutObject(dictionary[index]);
                dictionary.Remove(index);
            }

            if (direction == Direction.Horizontal)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (!dictionary.ContainsKey(i))
                    {
                        Cell cell = cellPool.GetObject();
                        cell.name = i.ToString();
                        cell.index = i;
                        SetCellPositionHorizontal(cell, i);
                        dictionary.Add(i, cell);
                        if (CellAtIndex != null)
                            CellAtIndex(cell, i);
                    }
                }
            }
            else if (direction == Direction.Vertical)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (!dictionary.ContainsKey(i))
                    {
                        Cell cell = cellPool.GetObject();
                        cell.name = i.ToString();
                        cell.index = i;
                        SetCellPositionVertical(cell, i);
                        dictionary.Add(i, cell);
                        if (CellAtIndex != null)
                            CellAtIndex(cell, i);
                    }
                }
            }

        }

        private void IndexInRect(Rect rect, out int startIndex, out int endIndex)
        {
            startIndex = 0;
            endIndex = 0;
            if (direction == Direction.Horizontal)
            {
                float startX = rect.x;
                float endX = rect.x + rect.width;

                int startLine = (int)Mathf.Floor((startX - linePadding) / (cellWidth + lineSpace)) - 1;
                int endLine = (int)Mathf.Floor((endX - linePadding) / (cellWidth + lineSpace)) + 1;

                startIndex = startLine * countPreLine;
                endIndex = (endLine + 1) * countPreLine - 1;

                startIndex = Mathf.Clamp(startIndex, 0, countOfCell - 1);
                endIndex = Mathf.Clamp(endIndex, 0, countOfCell - 1);
            }
            else if (direction == Direction.Vertical)
            {
                float startY = rect.y;
                float endY = rect.y + rect.height;

                int startLine = (int)Mathf.Floor((startY - linePadding) / (cellHeight + lineSpace)) - 1;
                int endLine = (int)Mathf.Floor((endY - linePadding) / (cellHeight + lineSpace)) + 1;

                startIndex = startLine * countPreLine;
                endIndex = (endLine + 1) * countPreLine - 1;

                startIndex = Mathf.Clamp(startIndex, 0, countOfCell - 1);
                endIndex = Mathf.Clamp(endIndex, 0, countOfCell - 1);
            }

        }

        private void SetCellPositionHorizontal(Cell cell, int index)
        {
            cell.transform.SetParent(scrollRect.content);
            RectTransform rt = cell.GetComponent<RectTransform>();
            float y = -itemPadding - index % countPreLine * (cellHeight + itemSpace);
            float x = linePadding + index / countPreLine * (cellWidth + lineSpace);
            rt.anchoredPosition = new Vector2(x, y);
        }

        private void SetCellPositionVertical(Cell cell, int index)
        {
            cell.transform.SetParent(scrollRect.content);
            RectTransform rt = cell.GetComponent<RectTransform>();
            float x = itemPadding + index % countPreLine * (cellWidth + itemSpace);
            float y = -linePadding - index / countPreLine * (cellHeight + lineSpace);
            rt.anchoredPosition = new Vector2(x, y);
        }
    }



}