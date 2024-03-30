using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout: LayoutGroup
{
    public int row;
    public int col;
    public Vector2 cellSize;
    public Vector2 spacing;


    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        float parentHeight = rectTransform.rect.height - padding.top - padding.bottom - spacing.y * (row - 1);
        float parentWidth = rectTransform.rect.width - padding.left - padding.right - spacing.x * (col - 1);

        float cellHeight = parentHeight / row;
        float cellWidth = parentWidth / col;

        cellSize = new Vector2 (cellWidth, cellHeight);

        int rowCount = 0;
        int colCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            var item = rectChildren[i];

            var xPos = cellSize.x * colCount + padding.left + spacing.x * colCount;
            var yPos = cellSize.y * rowCount + padding.top + spacing.y * rowCount;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);

            colCount++;
            if (colCount == col)
            {
                colCount = 0;
                rowCount++;
            }
        }
    }
    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }

    public void SetSize(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
}
