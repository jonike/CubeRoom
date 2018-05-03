using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

		private ScrollRect scrollRect;

		private Vector2 scrollSize;
		private Vector2 contentSize;

		private float itemSpace = 0.0f;
		private float lineSpace = 0.0f;

		private int countPreLine;
		private int countOfLine;

		public delegate int IntDelegate();
		public delegate void GameObjectIntAction(GameObject gameObject, int i);
		public IntDelegate CountOfCell;
		public GameObjectIntAction CellAtIndex;
		// public delegate 
		

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

			int count = CountOfCell != null ? CountOfCell() : 0;
			countPreLine = countPreLine + 1;
			countOfLine = count / countPreLine + (count % countPreLine == 0 ? 0 : 1);

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
				scrollRect.content.sizeDelta = contentSize;
			} else if (direction == Direction.Vertical) {
				contentSize.y = countOfLine * (cellHeight + lineSpace) - lineSpace;
				scrollRect.content.sizeDelta = contentSize;
			}


			// Test
			if (direction == Direction.Horizontal) {
				for (int i = 0; i < count; i++) {
					GameObject cell = GameObject.Instantiate(cellObject, Vector3.zero, Quaternion.identity);
					cell.transform.SetParent(scrollRect.content);
					RectTransform rt = cell.GetComponent<RectTransform>();
					float y = - i % countPreLine * (cellHeight + itemSpace);
					float x = i / countPreLine * (cellWidth + lineSpace);
					rt.anchoredPosition = new Vector2(x, y);
					if (CellAtIndex != null)
						CellAtIndex(cell, i);
				}
			} else if (direction == Direction.Vertical) {
				for (int i = 0; i < count; i++) {
					GameObject cell = GameObject.Instantiate(cellObject, Vector3.zero, Quaternion.identity);
					cell.transform.SetParent(scrollRect.content);
					RectTransform rt = cell.GetComponent<RectTransform>();
					float x = i % countPreLine * (cellWidth + itemSpace);
					float y = - i / countPreLine * (cellHeight + lineSpace);
					rt.anchoredPosition = new Vector2(x, y);
					if (CellAtIndex != null)
						CellAtIndex(cell, i);
				}
				
			}
			
		}

		public void OnValueChanged(Vector2 value) {
			float deltaY = scrollSize.y - contentSize.y;
			float startY = (value.y - 1) * deltaY;
			
			float deltaX = scrollSize.x - contentSize.x;
			float startX = (value.x - 1) * deltaX;
			
			Rect rect = new Rect(startX, startY, scrollSize.x, scrollSize.y);

			UpdateInRect(rect);
		}

		private void UpdateInRect(Rect rect) {
			float startY = rect.y;
			float endY = rect.y + rect.height;

			int startLine =  (int) Mathf.Floor(startY / (cellHeight + lineSpace));
			int endLine =  (int) Mathf.Ceil(endY / (cellHeight + lineSpace)) + 1;
			startLine = Mathf.Clamp(startLine, 1, countOfLine);
			endLine = Mathf.Clamp(endLine, 1, countOfLine);

			Debug.Log(startLine + " " + endLine);

		}
	}

	

}