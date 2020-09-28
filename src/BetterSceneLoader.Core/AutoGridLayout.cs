namespace UnityEngine.UI.Extensions
{
    public class AutoGridLayout : GridLayoutGroup
    {
        public bool m_IsColumn;
        public int m_Column = 1, m_Row = 1;
        public float aspectRatio = 16f / 9f;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            float iColumn = -1;
            float iRow = -1;
            if(m_IsColumn)
            {
                iColumn = m_Column;
                if(iColumn <= 0)
                {
                    iColumn = 1;
                }
                iRow = Mathf.CeilToInt(transform.childCount / iColumn);
                float fWidth = (rectTransform.rect.width - ((iColumn - 1) * (spacing.x))) - ((padding.right + padding.left));
                float width = fWidth / iColumn;
                cellSize = new Vector2(width, width / aspectRatio);
            }
            else
            {
                iRow = m_Row;
                if(iRow <= 0)
                {
                    iRow = 1;
                }
                iColumn = Mathf.CeilToInt(transform.childCount / iRow);
                float fHeight = (rectTransform.rect.height - ((iRow - 1) * (spacing.y))) - ((padding.top + padding.bottom));
                float height = fHeight / iRow;
                cellSize = new Vector2(height * aspectRatio, height);
            }
        }
    }
}
