public class WorkbookModel
{
    public static WorkbookModel Instance { get;  set; }

    public ColorizationModeModel ColorizationModeModel { get; set; }

    public CurrentColorModel CurrentColorModel { get;  set; }

    public TutorialModel TutorialModel { get;  set; }

    public SpecBoostersModel SpecBoostersModel { get;  set; }

    public WorkbookModel()
    {
        this.ColorizationModeModel = new ColorizationModeModel();
        this.CurrentColorModel = new CurrentColorModel();
        this.TutorialModel = new TutorialModel();
        this.SpecBoostersModel = new SpecBoostersModel();
    }

    public static void Init()
    {
        WorkbookModel.Instance = new WorkbookModel();
    }
}


