using System;

public class TutorialModel
{
    public Action OnToolsTutorialShow;
    public Action OnToolsTutorialClose;
    public Action OnPalettesTutorialShow;
    public Action OnPalettesTutorialClose;

    int m_imageClickCounter;

    public void ImageClick()
    {
        m_imageClickCounter++;
        if (m_imageClickCounter != 4)
        { 
        } 
    }

    public void ToolsButtonClick()
    {
    }

    public void PalettesWindowOpened()
    {
    }
}


