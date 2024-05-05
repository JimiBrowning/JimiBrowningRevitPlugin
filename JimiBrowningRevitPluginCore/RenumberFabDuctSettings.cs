namespace JimiBrowningRevitPlugin;

// TODO: Save and load settings
// TODO: Separate locking and greying out

public class RenumberFabDuctSettings
{
    internal const string DEFAULT_ITEM_NUMBER_PREFIX = "";
    internal const int DEFAULT_ITEM_NUMBER_COUNT = 1;
    internal const bool DEFAULT_PREVENT_DUPLICATES = true;
    internal const bool DEFAULT_LOCK_CURRENT = true;
    internal const bool DEFAULT_LOCK_ALL_WITH_ITEM_NUMBER = true;

    public static RenumberFabDuctSettings Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    instance ??= new RenumberFabDuctSettings();
                }
            }

            return instance;
        }
    }

    private static readonly object lockObject = new();
    private static RenumberFabDuctSettings instance = null;

    public bool PreventDuplicates { get; internal set; } =
        DEFAULT_PREVENT_DUPLICATES;
    public bool LockCurrent { get; internal set; } =
        DEFAULT_LOCK_CURRENT;
    public bool LockAllWithItemNumber { get; internal set; } =
        DEFAULT_LOCK_ALL_WITH_ITEM_NUMBER;
    public string ItemNumber
    {
        get { return itemNumberPrefix + itemNumberCount.ToString(); }
    }

    internal string itemNumberPrefix = DEFAULT_ITEM_NUMBER_PREFIX;
    internal int itemNumberCount = DEFAULT_ITEM_NUMBER_COUNT;

    private OnItemNumberIncrementedCallback
        onItemNumberIncrementedCallback = null;

    public void IncrementItemNumber()
    {
        if (itemNumberCount < int.MaxValue)
            itemNumberCount++;
        else
            itemNumberCount = DEFAULT_ITEM_NUMBER_COUNT;

        onItemNumberIncrementedCallback?.Invoke(itemNumberCount);
    }

    internal delegate void OnItemNumberIncrementedCallback(int itemNumberCount);

    internal void SetOnItemNumberIncrementedCallback(
        OnItemNumberIncrementedCallback onItemNumberIncrementedCallback)
    {
        this.onItemNumberIncrementedCallback = onItemNumberIncrementedCallback;
    }
}