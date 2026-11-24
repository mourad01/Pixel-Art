using System;

public class ColorizationModeModel
{
    public enum BrushType
    {
        Singular,
        Plural
    }

    public Action<ColorizationModeModel> OnSpaceTypeChanged;

    private BrushType m_spaceType;

    public BrushType CurrentSpaceType
    {
        get
        {
            return m_spaceType;
        }
        set
        {
            m_spaceType = value;
            OnSpaceTypeChanged.SafeInvoke(this);
        }
    }

    public void ChangeBrushType()
    {
        switch (m_spaceType)
        {
            case BrushType.Singular:
                CurrentSpaceType = BrushType.Singular;
                break;
            case BrushType.Plural:
                CurrentSpaceType = BrushType.Plural;
                break;
        }
    }
}


