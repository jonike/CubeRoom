using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.UI;

public class ClickBuildCell : Cell, IPointerUpHandler
{
    private ClickBuildCellView cellView;

    private Text nameText;
    private Image image;

    void Start()
    {
        cellView = GetComponentInParent<ClickBuildCellView>();
    }

    public override void Init()
    {
        nameText = transform.Find("name").GetComponent<Text>();
        image = transform.Find("image").GetComponent<Image>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cellView.OnClick(index);
    }

    public void SetBuild(BuildPO build)
    {
        if (build.type == BuildType.Wall)
        {
            WallPO wall = (WallPO)build;

            nameText.text = wall.name;
            string path = string.Format("Images/Walls/wall_{0}_512", wall.name);
            Sprite sprite = Resources.Load<Sprite>(path) as Sprite;
            if (sprite)
                image.sprite = sprite;
            else
                image.color = Color.clear;
        }

    }
}
