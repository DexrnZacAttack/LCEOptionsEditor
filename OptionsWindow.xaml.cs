using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace LCEOptionsEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        Options options;

        public StackPanel GetCategoryPanel(Option.OptionCategory optionCategory)
        {
            switch (optionCategory)
            {
                case Option.OptionCategory.GameOptions:
                    return GameOptions;
                case Option.OptionCategory.Audio:
                    return Audio;
                case Option.OptionCategory.Graphics:
                    return Graphics;
                case Option.OptionCategory.UserInterface:
                    return UserInterface;
                case Option.OptionCategory.Controls:
                    return Controls;
                default:
                    return Other;
            }
        }
        
        public void LoadOptions()
        {
            GameOptions.Children.Clear();
            Audio.Children.Clear();
            Graphics.Children.Clear();
            UserInterface.Children.Clear();
            Controls.Children.Clear();
            Other.Children.Clear();

            foreach (var option in options.Settings)
            {
                int value = options.Read(option);

                switch (option.Type)
                {
                    case Option.OptionType.Boolean: 
                        CheckBox bCheck = new CheckBox()
                        {
                            Content = option.Name,
                            IsChecked = Convert.ToBoolean(value)
                        };
                        
                        bCheck.Click += (sender, args) =>
                        {
                            options.Write(option, Convert.ToByte(bCheck.IsChecked.Value));
                        };
                        GetCategoryPanel(option.Category).Children.Add(bCheck);
                        break;
                    case Option.OptionType.ReverseBoolean: 
                        CheckBox rbCheck = new CheckBox()
                        {
                            Content = option.Name,
                            IsChecked = !Convert.ToBoolean(value)
                        };
                        
                        rbCheck.Click += (sender, args) =>
                        {
                            options.Write(option, Convert.ToByte(!rbCheck.IsChecked.Value));
                        };
                        GetCategoryPanel(option.Category).Children.Add(rbCheck);
                        break;
                    case Option.OptionType.Slider:
                        TextBlock name = new TextBlock
                        {
                            Text = option.Name
                        };

                        Slider slider = new Slider
                        {
                            Value = value,
                            Maximum = Math.Pow(2,option.DataInfo.Size) - 1,
                            IsSnapToTickEnabled = true,
                            TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight,
                            AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.TopLeft
                        };
                        slider.ValueChanged += (sender, args) =>
                        {
                            value = Convert.ToByte(args.NewValue);
                            options.Write(option, Convert.ToByte(args.NewValue));
                        };
                        GetCategoryPanel(option.Category).Children.Add(name);
                        GetCategoryPanel(option.Category).Children.Add(slider);
                        break;
                }
                
            }
        }

        public OptionsWindow(string path, int offset = 0)
        {
            InitializeComponent();
            this.Title = App.Name;
            
            options = new Options(path, offset);
            LoadOptions();
                        
            SaveButton.Click += (sender, args) =>
            {
                options.Save();
            };

            RefreshButton.Click += (sender, args) =>
            {
                options = new Options(path);
                LoadOptions();
            };
        }
    }
}