using System.Windows.Controls;

namespace LCEOptionsEditor;

public partial class ControlBox : UserControl
{
    BindOptions options;
    BindOption option;
    public ControlBox(BindOptions options, BindOption option)
    {
        this.options = options;
        this.option = option;
        InitializeComponent();
        foreach (BindOption.ControllerButton button in Enum.GetValues(typeof(BindOption.ControllerButton)))
        {
            ctrlBox.Items.Add(button.ToString());
        }
        
        ctrlBox.SelectedItem = options.Read(option).ToString();
    }

    private void CtrlBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Enum.TryParse<BindOption.ControllerButton>((string)ctrlBox.SelectedItem, out var sel))
        {
            options.Write(option, sel);
        }
    }
}