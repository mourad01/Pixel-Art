public class WorkbookModel
{
    public static WorkbookModel Instance { get;  set; }

    public ColorizationModeModel ColorizationModeModel { get; set; }

    public CurrentColorModel CurrentColorModel { get;  set; }

    public TutorialModel TutorialModel { get;  set; }

    public SpecBoostersModel SpecBoostersModel { get;  set; }

    public WorkbookModel()
    {
        ColorizationModeModel = new ColorizationModeModel();
        CurrentColorModel = new CurrentColorModel();
        TutorialModel = new TutorialModel();
        SpecBoostersModel = new SpecBoostersModel();
    }

    public static void Init()
    {
        Instance = new WorkbookModel();
    }
}


