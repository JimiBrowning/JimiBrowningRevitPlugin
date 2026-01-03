using System;
using System.Windows.Forms;

namespace JimiBrowningRevitPlugin;

public partial class RenumberFabDuctForm : Form
{
    private static readonly object LockObject = new();
    private static readonly RenumberFabDuctSettings Settings =
        RenumberFabDuctSettings.Instance;

    private static RenumberFabDuctForm? instance;

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

        lock (LockObject)
        {
            if (instance != null)
                return;

            instance =
                new RenumberFabDuctForm(
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

        lock (LockObject)
        {
            instance?.Close();
        }
    }

    public static void EnableSettings()
    {
        if (instance == null)
            return;

        lock (LockObject)
        {
            if (instance == null)
                return;

            instance.finishRenumberingLabel.Visible = false;
            instance.lockCurrentCheckBox.Enabled = true;
            instance.lockAllCheckBox.Enabled = true;
            instance.startRenumberingButton.Enabled = true;
        }
    }

    public static void DisableSettings()
    {
        if (instance == null)
            return;

        lock (LockObject)
        {
            if (instance == null)
                return;

            instance.finishRenumberingLabel.Visible = true;
            instance.lockCurrentCheckBox.Enabled = false;
            instance.lockAllCheckBox.Enabled = false;
            instance.startRenumberingButton.Enabled = false;
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
        prefixTextBox.Text = Settings.itemNumberPrefix;
        countNumericUpDown.Value = Settings.itemNumberCount;
        preventDuplicateCheckBox.Checked = Settings.PreventDuplicates;
        lockCurrentCheckBox.Checked = Settings.LockCurrent;
        lockAllCheckBox.Checked = Settings.LockAllWithItemNumber;

        Settings.SetOnItemNumberIncrementedCallback(
            OnItemNumberIncrementedCallback);
    }

    private void RenumberFabDuctFormOnClosed(
        object sender,
        FormClosedEventArgs e)
    {
        Settings.SetOnItemNumberIncrementedCallback(null);

        instance = null;
    }

    private void PrefixTextBoxOnTextChanged(object sender, EventArgs e)
    {
        Settings.itemNumberPrefix = prefixTextBox.Text;
    }

    private void CountNumericUpDownOnValueChanged(object sender, EventArgs e)
    {
        if (disableCountOnChanged)
            return;

        try
        {
            Settings.itemNumberCount =
                Convert.ToInt32(countNumericUpDown.Value);
        }
        catch (OverflowException)
        {
            countNumericUpDown.Value =
                RenumberFabDuctSettings.DefaultItemNumberCount;
        }
    }

    private void SetToNextHighestButtonOnClick(object sender, EventArgs e)
    {
        int highestNumber =
            getHighestItemNumberCallback(Settings.itemNumberPrefix);

        if (highestNumber >= countNumericUpDown.Minimum - 1
            && highestNumber < countNumericUpDown.Maximum)
        {
            countNumericUpDown.Value = highestNumber + 1;
        }
        else
        {
            countNumericUpDown.Value =
                RenumberFabDuctSettings.DefaultItemNumberCount;
        }
    }

    private void PreventDuplicateCheckBoxOnCheckedChanged(
        object sender,
        EventArgs e)
    {
        Settings.PreventDuplicates = preventDuplicateCheckBox.Checked;
    }

    private void LockCurrentCheckBoxOnCheckedChanged(object sender, EventArgs e)
    {
        Settings.LockCurrent = lockCurrentCheckBox.Checked;

        if (!lockCurrentCheckBox.Checked && lockAllCheckBox.Checked)
            lockAllCheckBox.Checked = false;
    }

    private void LockAllCheckBoxOnCheckedChanged(object sender, EventArgs e)
    {
        Settings.LockAllWithItemNumber = lockAllCheckBox.Checked;

        if (lockAllCheckBox.Checked && !lockCurrentCheckBox.Checked)
            lockCurrentCheckBox.Checked = true;
    }

    private void StartRenumberingButtonOnClick(object sender, EventArgs e)
    {
        startRenumberingCallback();
    }
}