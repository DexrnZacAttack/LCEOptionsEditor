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
                    return MovementOptions; // nothing else goes in controls anyways
                default:
                    return Other;
            }
        }
        
        public StackPanel GetBindingCategoryPanel(BindOption.BindCategory category)
        {
            switch (category)
            {
                case BindOption.BindCategory.Standard:
                    return StandardControls;
                case BindOption.BindCategory.Creative:
                    return CreativeControls;
                case BindOption.BindCategory.MiniGameLobby:
                    return MiniGameLobbyControls;
                case BindOption.BindCategory.Spectating:
                    return SpectatingControls;
                case BindOption.BindCategory.GlideMiniGame:
                    return GlideControls; 
                default:
                    return StandardControls;
            }
        }

        public void LoadSkinOptions()
        {
            FavoriteSkins.Children.Clear();
            SelectedSkins.Children.Clear();

            for (byte i = 0; i < 10; i++)
            {
                uint favorite = options.SkinOptions.getFavoriteSkin(i);
                TextBlock textBlock = new TextBlock();
                textBlock.Text = $"Slot {i + 1} ID: {(favorite != 0xFFFFFFFF ? favorite.ToString() : "None")}";
                FavoriteSkins.Children.Add(textBlock);
            }
            
            TextBlock selectedSkin = new TextBlock();
            selectedSkin.Text = $"Selected Skin: {options.SkinOptions.getSelectedSkin()}";
            SelectedSkins.Children.Add(selectedSkin);
            
            TextBlock selectedCape = new TextBlock();
            selectedCape.Text = $"Selected Cape: {options.SkinOptions.getSelectedCape()}";
            SelectedSkins.Children.Add(selectedCape);
        }
        
        public void LoadControlBindings()
        {
            StandardControls.Children.Clear();
            CreativeControls.Children.Clear();
            MiniGameLobbyControls.Children.Clear();
            SpectatingControls.Children.Clear();
            GlideControls.Children.Clear();

            foreach (BindOption bOption in options.BindOptions.Bindings)
            {
                Grid controlPanel = new Grid();
                controlPanel.Width = 400;
                controlPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                controlPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) });

                TextBlock bind = new TextBlock();
                bind.FontSize = 16;
                bind.Text = $"{bOption.Name}";
                Grid.SetColumn(bind, 0);
                
                ControlBox box = new ControlBox(options.BindOptions, bOption);
                Grid.SetColumn(box, 1);
                
                controlPanel.Children.Add(bind);
                controlPanel.Children.Add(box);
                GetBindingCategoryPanel(bOption.Category).Children.Add(controlPanel);
            }
        }

        public void LoadOptions()
        {
            GameOptions.Children.Clear();
            Audio.Children.Clear();
            Graphics.Children.Clear();
            UserInterface.Children.Clear();
            MovementOptions.Children.Clear();
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

            LoadSkinOptions();
            LoadControlBindings();
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var option in options.BindOptions.Bindings)
            {
                options.BindOptions.Reset(option);
            }

            options.Save();
            LoadOptions();
        }
    }
}