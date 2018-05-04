using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sorumi.Util;

namespace Sorumi.UI {

	[RequireComponent(typeof(ScrollRect))]
	public class CellView : MonoBehaviour {

		// Use this for initialization

		public enum Direction {
			Horizontal,
			Vertical
		}

		public Direction direction;
		public GameObject cellObject;
		public float cellWidth;
		public float cellHeight;
		public float minItemSpace = 0.0f;
		public float minLineSpace = 0.0f;

		public float max

		private ScrollRect scrollRect;

		private Vector2 scrollSize;
		private Vector2 contentSize;

		private float itemSpace = 0.0f;
		private float lineSpace = 0.0f;

		private int countOfCell;
		private int countPreLine;
		private int countOfLine;

		private GameObject poolGO;
		private ObjectPool<GameObject> cellPool;
		private Dictionary<int, GameObject> dictionary;

		private int startIndex = -1;
		private int endIndex = -1;
		

		public delegate int IntDelegate();
		public delegate void GameObjectIntAction(GameObject gameObject, int i);
		public IntDelegate CountOfCell;
		public GameObjectIntAction CellAtIndex;

		void Start () {
			scrollRect = GetComponent<ScrollRect>();
			Rect sr = scrollRect.GetComponent<RectTransform>().rect;
			scrollSize = new Vector2(sr.width, sr.height);
			Rect cr = scrollRect.content.GetComponent<RectTransform>().rect;
			contentSize = new Vector2(cr.width, cr.height);
			// Debug.Log(scrollSize + " " + contentSize);
			PrepareLayout();
		}

		private void PrepareLayout() {
			if (direction == Direction.Horizontal) {
				countPreLine = (int) Mathf.Floor((contentSize.y - cellHeight) / (cellHeight + minItemSpace));
			} else if (direction == Direction.Vertical) {
				countPreLine = (int) Mathf.Floor((contentSize.x - cellWidth) / (cellWidth + minItemSpace));
			}

			countOfCell = CountOfCell != null ? CountOfCell() : 0;
			countPreLine = countPreLine + 1;
			countOfLine = countOfCell / countPreLine + (countOfCell % countPreLine == 0 ? 0 : 1);

			if (countPreLine == 1) {
				itemSpace = 0;
			} else if (direction == Direction.Horizontal) {
				itemSpace = (contentSize.y - countPreLine * cellHeight) / (countPreLine - 1);
			} else if (direction == Direction.Vertical) {
				itemSpace = (contentSize.x - countPreLine * cellWidth) / (countPreLine - 1);
			}

			lineSpace = itemSpace > minLineSpace ? itemSpace : minLineSpace;

			// Renew contentSize
			if (direction == Direction.Horizontal) {
				contentSize.x = countOfLine * (cellWidth + lineSpace) - lineSpace;
				contentSize.x = contentSize.x < 0 ? 0 : contentSize.x;
				scrollRect.content.sizeDelta = contentSize;
				scrollRect.content.anchoredPosition = Vector3.zero;
			} else if (direction == Direction.Vertical) {
				contentSize.y = countOfLine * (cellHeight + lineSpace) - lineSpace;
				contentSize.y = contentSize.y < 0 ? 0 : contentSize.y;
				scrollRect.content.sizeDelta = contentSize;
				scrollRect.content.anchoredPosition = Vector3.zero;
			}

			cellPool = new ObjectPool<GameObject>(CreateCell, ResetCell);
			poolGO = new GameObject("PoolGameObject");
			poolGO.transform.position = Vector3.zero;
			poolGO.SetActive(false);
			poolGO.transform.SetParent(scrollRect.transform);

			dictionary = new Dictionary<int, GameObject>();

			Rect rect = new Rect(0, 0, scrollSize.x, scrollSize.y);
			UpdateInRect(rect);
		}
		
		private GameObject CreateCell() {
			GameObject cell = GameObject.Instantiate(cellObject, Vector3.zero, Quaternion.identity);
			// RectTransform rt = cell.GetComponent<RectTransform>();
			// rt.anchoredPosition = new Vector2(-cellWidth, cellHeight);
			cell.transform.SetParent(poolGO.transform);
			return cell;
		}

		private void ResetCell(GameObject cell) {
			// RectTransform rt = cell.GetComponent<RectTransform>();
			// rt.anchoredPosition = new Vector2(-cellWidth, cellHeight);
			cell.transform.SetParent(poolGO.transform);
		}

		public void OnValueChanged(Vector2 value) {
			float deltaY = scrollSize.y - contentSize.y;
			float startY = (value.y - 1) * deltaY;
			
			float deltaX = scrollSize.x - contentSize.x;
			float startX = - value.x * deltaX;
			
			Debug.Log(value.x + " " + startX);

			Rect rect = new Rect(startX, startY, scrollSize.x, scrollSize.y);

			UpdateInRect(rect);
		}

		private void UpdateInRect(Rect rect) {
			if (countOfCell == 0)
				return;
			
			IndexInRect(rect, out startIndex, out endIndex);

			List<int> deleteList = new List<int>();
        	foreach (KeyValuePair<int, GameObject> pair in dictionary) {
				if (pair.Key < startIndex || pair.Key > endIndex) {
					deleteList.Add(pair.Key);
				}
        	}
        
        	foreach (int index in deleteList) {
				cellPool.PutObject(dictionary[index]);
				dictionary.Remove(index);
        	}

			if (direction == Direction.Horizontal) {
				for (int i = startIndex; i <= endIndex; i++) {
					if (!dictionary.ContainsKey(i)) {
						GameObject cell = cellPool.GetObject();
						SetCellPositionHorizontal(cell, i);
						dictionary.Add(i, cell);
						if (CellAtIndex != null)
							CellAtIndex(cell, i);
					}
				}
			} else if (direction == Direction.Vertical) {
				for (int i = startIndex; i <= endIndex; i++) {
					if (!dictionary.ContainsKey(i)) {
						GameObject cell = cellPool.GetObject();
						SetCellPositionVertical(cell, i);
						dictionary.Add(i, cell);
						if (CellAtIndex != null)
							CellAtIndex(cell, i);
					}
				}
			}
			
		}

		private void IndexInRect(Rect rect, out int startIndex, out int endIndex) {
			startIndex = 0;
			endIndex = 0;
			if (direction == Direction.Horizontal) {
				float startX = rect.x;
				float endX = rect.x + rect.width;
				

				int startLine =  (int) Mathf.Floor(startX / (cellWidth + lineSpace));
				int endLine =  (int) Mathf.Floor(endX / (cellWidth + lineSpace)) + 1;
				
				startIndex = startLine * countPreLine;
				endIndex = (endLine + 1) * countPreLine - 1;

				startIndex = Mathf.Clamp(startIndex, 0, countOfCell - 1);
				endIndex = Mathf.Clamp(endIndex, 0, countOfCell - 1);
			} else if (direction == Direction.Vertical) {
				float startY = rect.y;
				float endY = rect.y + rect.height;

				int startLine =  (int) Mathf.Floor(startY / (cellHeight + lineSpace));
				int endLine =  (int) Mathf.Floor(endY / (cellHeight + lineSpace)) + 1;
				
				startIndex = startLine * countPreLine;
				endIndex = (endLine + 1) * countPreLine - 1;

				startIndex = Mathf.Clamp(startIndex, 0, countOfCell - 1);
				endIndex = Mathf.Clamp(endIndex, 0, countOfCell - 1);
			}

		}

		private void SetCellPositionHorizontal(GameObject cell, int index) {
			cell.transform.SetParent(scrollRect.content);
			RectTransform rt = cell.GetComponent<RectTransform>();
			float y = - index % countPreLine * (cellHeight + itemSpace);
			float x = index / countPreLine * (cellWidth + lineSpace);
			rt.anchoredPosition = new Vector2(x, y);
		}

		private void SetCellPositionVertical(GameObject cell, int index) {
			cell.transform.SetParent(scrollRect.content);
			RectTransform rt = cell.GetComponent<RectTransform>();
			float x = index % countPreLine * (cellWidth + itemSpace);
			float y = - index / countPreLine * (cellHeight + lineSpace);
			rt.anchoredPosition = new Vector2(x, y);
		}
	}

	

}