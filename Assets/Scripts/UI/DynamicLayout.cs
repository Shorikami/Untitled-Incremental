using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicLayout : LayoutGroup
{
    public enum FitType
    { 
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    public int m_Rows, m_Columns;

    public Vector2 m_CellSize;
    public Vector2 m_Spacing;

    public FitType m_FitType;

    public bool fitX, fitY;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (m_FitType == FitType.Width || m_FitType == FitType.Height || m_FitType == FitType.Uniform)
        {
            fitX = true;
            fitY = true;

            float sqrt = Mathf.Sqrt(transform.childCount);
            m_Rows = Mathf.CeilToInt(sqrt);
            m_Columns = Mathf.CeilToInt(sqrt);
        }

        switch (m_FitType)
        {
            case FitType.Width:
            case FitType.FixedColumns:
            {
                 m_Rows = Mathf.CeilToInt(transform.childCount / (float)m_Columns);
            }
            break;

            case FitType.Height:
            case FitType.FixedRows:
            {
                m_Columns = Mathf.CeilToInt(transform.childCount / (float)m_Rows);
            }
            break;
        }

        float parentW = rectTransform.rect.width;
        float parentH = rectTransform.rect.height;

        float cellW = (parentW / (float)m_Columns) - ((m_Spacing.x / (float)m_Columns) * (m_Columns - 1)) 
            - (padding.left / (float)m_Columns) - (padding.right / (float)m_Columns);

        float cellH = (parentH / (float)m_Rows) - ((m_Spacing.y / (float)m_Rows) * (m_Rows - 1))
            - (padding.top / (float)m_Rows) - (padding.bottom / (float)m_Rows);

        m_CellSize.x = fitX ? cellW : m_CellSize.x;
        m_CellSize.y = fitY ? cellH : m_CellSize.y;

        int colCount = 0, rowCount = 0;

        for (int i = 0; i < rectChildren.Count; ++i)
        {
            rowCount = i / m_Columns;
            colCount = i % m_Columns;

            var item = rectChildren[i];

            var xPos = (m_CellSize.x * colCount) + (m_Spacing.x * colCount) + padding.left;
            var yPos = (m_CellSize.y * rowCount) + (m_Spacing.y * rowCount) + padding.top;

            SetChildAlongAxis(item, 0, xPos, m_CellSize.x);
            SetChildAlongAxis(item, 1, yPos, m_CellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
        throw new System.NotImplementedException();
    }

    public override void SetLayoutHorizontal()
    {
        throw new System.NotImplementedException();
    }

    public override void SetLayoutVertical()
    {
        throw new System.NotImplementedException();
    }
}
