using System;
using System.Windows.Forms;

namespace JimiBrowningRevitPlugin;

public partial class RenumberFabDuctForm : Form
{
    private static readonly object lockObject = new();
    private static RenumberFabDuctForm instance;
    private static readonly RenumberFabDuctSettings settings =
        RenumberFabDuctSettings.Instance;

    private readonly GetHighestItemNumberCallback getHighestItemNumberCallback;
    private readonly StartRenumberingCallback startRenumberingCallback;
    private bool disableCountOnChanged = false;

    private RenumberFabDuctForm(
        GetHighestItemNumberCallback getHighestItemNumberCallback,
        StartRenumberingCallback startRenumberingCallback)
    {
        this.getHighestItemNumberCallback = getHighestItemNumberCallback;
        this.startRenumberingCallback = startRenumberingCallback;

        instance = this;

        InitializeComponent();
    }

    public static void ShowForm(
        GetHighestItemNumberCallback getHighestItemNumberCallback,
        StartRenumberingCallback startRenumberingCallback)
    {
        if (instance != null)
            return;

        lock (lockObject)
        {
            if (instance != null)
                return;

            instance = new RenumberFabDuctForm(
                getHighestItemNumberCallback,
                startRenumberingCallback);
            instance.Show();
        }
    }

    public static bool IsOpened()
    {
        return instance != null;
    }

    public static void CloseIfOpened()
    {
        if (instance == null)
            return;

        lock (lockObject)
        {
            instance?.Close();
        }
    }

    public static void EnableSettings()
    {
        if (instance == null)
            return;

        lock (lockObject)
        {
            if (instance == null)
                return;

            instance.finishRenumberingLabel.Visible = false;
            instance.lockCurrentCheckBox.Enabled = true;
            instance.lockAllCheckBox.Enabled = true;
            instance.startRenumberingButton.Enabled = true;
            instance.closeButton.Enabled = true;
        }
    }

    public static void DisableSettings()
    {
        if (instance == null)
            return;

        lock (lockObject)
        {
            if (instance == null)
                return;

            instance.finishRenumberingLabel.Visible = true;
            instance.lockCurrentCheckBox.Enabled = false;
            instance.lockAllCheckBox.Enabled = false;
            instance.startRenumberingButton.Enabled = false;
            instance.closeButton.Enabled = false;
        }
    }

    public delegate int GetHighestItemNumberCallback(string itemNumberPrefix);

    public delegate void StartRenumberingCallback();

    public void OnItemNumberIncrementedCallback(int itemNumberCount)
    {
        disableCountOnChanged = true;
        countNumericUpDown.Value = itemNumberCount;
        disableCountOnChanged = false;
    }

    private void RenumberFabDuctFormOnLoad(object sender, EventArgs e)
    {
        prefixTextBox.Text = settings.itemNumberPrefix;
        countNumericUpDown.Value = settings.itemNumberCount;
        preventDuplicateCheckBox.Checked = settings.PreventDuplicates;
        lockCurrentCheckBox.Checked = settings.LockCurrent;
        lockAllCheckBox.Checked = settings.LockAllWithItemNumber;

        settings.SetOnItemNumberIncrementedCallback(
            OnItemNumberIncrementedCallback);
    }

    private void RenumberFabDuctFormOnClosed(
        object sender,
        FormClosedEventArgs e)
    {
        settings.SetOnItemNumberIncrementedCallback(null);

        instance = null;
    }

    private void PrefixTextBoxOnTextChanged(object sender, EventArgs e)
    {
        settings.itemNumberPrefix = prefixTextBox.Text;
    }

    private void CountNumericUpDownOnValueChanged(object sender, EventArgs e)
    {
        if (disableCountOnChanged)
            return;

        try
        {
            settings.itemNumberCount =
                Convert.ToInt32(countNumericUpDown.Value);
        }
        catch (OverflowException)
        {
            countNumericUpDown.Value =
                RenumberFabDuctSettings.DEFAULT_ITEM_NUMBER_COUNT;
        }
    }

    private void SetToNextHighestButtonOnClick(object sender, EventArgs e)
    {
        int highestNumber =
            getHighestItemNumberCallback(settings.itemNumberPrefix);

        if (highestNumber >= countNumericUpDown.Minimum - 1
            && highestNumber < countNumericUpDown.Maximum)
        {
            countNumericUpDown.Value = highestNumber + 1;
        }
        else
        {
            countNumericUpDown.Value =
                RenumberFabDuctSettings.DEFAULT_ITEM_NUMBER_COUNT;
        }
    }

    private void PreventDuplicateCheckBoxOnCheckedChanged(
        object sender,
        EventArgs e)
    {
        settings.PreventDuplicates = preventDuplicateCheckBox.Checked;
    }

    private void LockCurrentCheckBoxOnCheckedChanged(object sender, EventArgs e)
    {
        settings.LockCurrent = lockCurrentCheckBox.Checked;

        if (!lockCurrentCheckBox.Checked && lockAllCheckBox.Checked)
            lockAllCheckBox.Checked = false;
    }

    private void LockAllCheckBoxOnCheckedChanged(object sender, EventArgs e)
    {
        settings.LockAllWithItemNumber = lockAllCheckBox.Checked;

        if (lockAllCheckBox.Checked && !lockCurrentCheckBox.Checked)
            lockCurrentCheckBox.Checked = true;
    }

    private void StartRenumberingButtonOnClick(object sender, EventArgs e)
    {
        startRenumberingCallback();
    }
    private void CloseButtonOnClick(object sender, EventArgs e)
    {
        Close();
    }
}