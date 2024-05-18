namespace JimiBrowningRevitPlugin;

// TODO: Save and load settings
// TODO: Separate locking and greying out

public class RenumberFabDuctSettings
{
    internal const string DefaultItemNumberPrefix = "";
    internal const int DefaultItemNumberCount = 1;
    internal const bool DefaultPreventDuplicates = true;
    internal const bool DefaultLockCurrent = true;
    internal const bool DefaultLockAllWithItemNumber = true;

    public static RenumberFabDuctSettings Instance
    {
        get
        {
            if (instance == null)
            {
                lock (LockObject)
                {
                    instance ??= new RenumberFabDuctSettings();
                }
            }

            return instance;
        }
    }

    private static readonly object LockObject = new();

    private static RenumberFabDuctSettings? instance = null;

    public bool PreventDuplicates { get; internal set; } =
        DefaultPreventDuplicates;
    public bool LockCurrent { get; internal set; } =
        DefaultLockCurrent;
    public bool LockAllWithItemNumber { get; internal set; } =
        DefaultLockAllWithItemNumber;
    public string ItemNumber
    {
        get { return itemNumberPrefix + itemNumberCount.ToString(); }
    }

    internal string itemNumberPrefix = DefaultItemNumberPrefix;
    internal int itemNumberCount = DefaultItemNumberCount;

    private OnItemNumberIncrementedCallback?
        onItemNumberIncrementedCallback = null;

    public void IncrementItemNumber()
    {
        if (itemNumberCount < int.MaxValue)
            itemNumberCount++;
        else
            itemNumberCount = DefaultItemNumberCount;

        onItemNumberIncrementedCallback?.Invoke(itemNumberCount);
    }

    internal delegate void OnItemNumberIncrementedCallback(int itemNumberCount);

    internal void SetOnItemNumberIncrementedCallback(
        OnItemNumberIncrementedCallback? onItemNumberIncrementedCallback)
    {
        this.onItemNumberIncrementedCallback = onItemNumberIncrementedCallback;
    }
}