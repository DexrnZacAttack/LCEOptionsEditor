using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCEOptionsEditor;

public struct DataInfo
{
    public int Offset;
    public byte BitOffset;
    public int Size;
}

public class Option(
    string name,
    int offset,
    byte bitOffset,
    int size,
    Option.OptionType type,
    Option.OptionCategory optionCategory = Option.OptionCategory.Other)
{
    public enum OptionType
    {
        Boolean,
        Slider,
        ReverseBoolean
    }
    
    public enum OptionCategory
    {
        GameOptions,
        Audio,
        Graphics,
        UserInterface,
        Controls,
        Other
    }

    public readonly string Name = name;
    public DataInfo DataInfo = new()
    {
        Offset = offset,
        BitOffset = bitOffset,
        Size = size
    };
    public readonly OptionType Type = type;
    public readonly OptionCategory Category = optionCategory;
}

public class Options
{
    private readonly int _offset; // incase OPTIONSVERSION is inside the main options file, like Wii U Edition.
    // A solution like the above should be replaced by reading the version and acting accordingly
    public readonly List<Option> Settings;
    private readonly byte[] _optionsFile;
    private readonly string _path;
    public SkinOptions SkinOptions;
    public BindOptions BindOptions;

    public Options(string path, int offset = 0)
    {
        this._offset = offset;
        this._path = path;
        _optionsFile = File.ReadAllBytes(path);
        this.SkinOptions = new SkinOptions(_optionsFile);
        this.BindOptions = new BindOptions(_optionsFile, offset);
        
        this.Settings = new List<Option>();
        // Settings are from the docs that I wrote at https://team-lodestone.github.io/Documentation/LCE/Game/Options.html
        // Also yes, this is probably not the most efficient or neatest way to do it.
        // Audio
        this.Settings.Add(new Option("Music", _offset  + 0x01, 0x00, 8, Option.OptionType.Slider, Option.OptionCategory.Audio));
        this.Settings.Add(new Option("Sound", _offset  + 0x02, 0x00, 8, Option.OptionType.Slider, Option.OptionCategory.Audio));
        this.Settings.Add(new Option("Cave Sounds", _offset  + 0x56, 0x03, 1, Option.OptionType.Boolean, Option.OptionCategory.Audio));
        this.Settings.Add(new Option("Minecart Sounds", _offset  + 0x56, 0x05, 1, Option.OptionType.Boolean, Option.OptionCategory.Audio));
        this.Settings.Add(new Option("Game Chat", _offset  + 0x56, 0x04, 1, Option.OptionType.Boolean, Option.OptionCategory.Audio));
        // Graphics
        this.Settings.Add(new Option("Render Clouds", _offset  + 0x54, 0x00, 1, Option.OptionType.Boolean, Option.OptionCategory.Graphics)); 
        this.Settings.Add(new Option("Custom Skin Animation", _offset  + 0x55, 0x01, 1, Option.OptionType.Boolean, Option.OptionCategory.Graphics)); 
        this.Settings.Add(new Option("Gamma", _offset  + 0x04, 0x00, 8, Option.OptionType.Slider, Option.OptionCategory.Graphics));
        this.Settings.Add(new Option("Show Glide Ghost", _offset  + 0x56, 0x06, 1, Option.OptionType.Boolean, Option.OptionCategory.Graphics));
        this.Settings.Add(new Option("Show Glide Ghost Path", _offset  + 0x57, 0x07, 1, Option.OptionType.Boolean, Option.OptionCategory.Graphics)); 
        // Game Options
        this.Settings.Add(new Option("Vertical Splitscreen", _offset  + 0x07, 0x00, 1, Option.OptionType.Boolean, Option.OptionCategory.GameOptions));
        this.Settings.Add(new Option("Auto Jump", _offset  + 0x57, 0x02, 1, Option.OptionType.Boolean, Option.OptionCategory.GameOptions)); 
        this.Settings.Add(new Option("View Bobbing", _offset  + 0x06, 0x02, 1, Option.OptionType.Boolean, Option.OptionCategory.GameOptions));
        this.Settings.Add(new Option("Flying View Rolling", _offset  + 0x57, 0x06, 1, Option.OptionType.ReverseBoolean, Option.OptionCategory.GameOptions)); 
        this.Settings.Add(new Option("Hints", _offset  + 0x07, 0x02, 1, Option.OptionType.Boolean, Option.OptionCategory.GameOptions));
        this.Settings.Add(new Option("Death Messages", _offset  + 0x55, 0x02, 1, Option.OptionType.Boolean, Option.OptionCategory.GameOptions)); 
        this.Settings.Add(new Option("Game Sensitivity", _offset  + 0x03, 0x00, 8, Option.OptionType.Slider, Option.OptionCategory.GameOptions));
        this.Settings.Add(new Option("Difficulty", _offset  + 0x06, 0x00, 2, Option.OptionType.Slider, Option.OptionCategory.GameOptions));
        // User Interface
        this.Settings.Add(new Option("Display Hud", _offset  + 0x54, 0x07, 1, Option.OptionType.Boolean, Option.OptionCategory.UserInterface));
        this.Settings.Add(new Option("Display Hand", _offset  + 0x55, 0x00, 1, Option.OptionType.Boolean, Option.OptionCategory.UserInterface)); 
        this.Settings.Add(new Option("Display Game Messages", _offset  + 0x57, 0x04, 1, Option.OptionType.Boolean, Option.OptionCategory.UserInterface)); 
        this.Settings.Add(new Option("Display Save Icon", _offset  + 0x57, 0x05, 1, Option.OptionType.Boolean, Option.OptionCategory.UserInterface));
        this.Settings.Add(new Option("Interface Opacity", _offset  + 0x51, 0x00, 8, Option.OptionType.Slider, Option.OptionCategory.UserInterface));
        this.Settings.Add(new Option("In-Game Tooltips", _offset  + 0x07, 0x07, 1, Option.OptionType.Boolean, Option.OptionCategory.UserInterface));
        this.Settings.Add(new Option("Animated Character", _offset  + 0x55, 0x07, 1, Option.OptionType.Boolean, Option.OptionCategory.UserInterface)); 
        this.Settings.Add(new Option("Interface Sensitivity", _offset  + 0x50, 0x00, 8, Option.OptionType.Slider, Option.OptionCategory.UserInterface));
        this.Settings.Add(new Option("In-Game Gamertags", _offset  + 0x06, 0x03, 1, Option.OptionType.Boolean, Option.OptionCategory.UserInterface));
        this.Settings.Add(new Option("Splitscreen Gamertags", _offset  + 0x07, 0x01, 1, Option.OptionType.Boolean, Option.OptionCategory.UserInterface)); 
        this.Settings.Add(new Option("Classic Crafting", _offset  + 0x56, 0x02, 1, Option.OptionType.Boolean, Option.OptionCategory.UserInterface)); 
        this.Settings.Add(new Option("Hud Size", _offset  + 0x55, 0x03, 2, Option.OptionType.Slider, Option.OptionCategory.UserInterface)); 
        this.Settings.Add(new Option("Hud Size Splitscreen", _offset  + 0x55, 0x05, 2, Option.OptionType.Slider, Option.OptionCategory.UserInterface)); 
        // Controls
        this.Settings.Add(new Option("Invert Look", _offset  + 0x06, 0x06, 1, Option.OptionType.Boolean, Option.OptionCategory.Controls));
        this.Settings.Add(new Option("Southpaw", _offset  + 0x06, 0x07, 1, Option.OptionType.Boolean, Option.OptionCategory.Controls));
        // Other
        this.Settings.Add(new Option("Unknown (maybe autosave interval)", _offset  + 0x07, 0x04, 4, Option.OptionType.Slider));
        this.Settings.Add(new Option("Unknown", _offset  + 0x07, 0x06, 2, Option.OptionType.Slider));
        this.Settings.Add(new Option("Unknown", _offset  + 0x56, 0x07, 1, Option.OptionType.Boolean)); // may be wrong? Had to change the offset since bits go past 7
        this.Settings.Add(new Option("Unknown", _offset  + 0x54, 0x01, 1, Option.OptionType.Boolean));
        this.Settings.Add(new Option("Unknown", _offset  + 0x54, 0x02, 1, Option.OptionType.Boolean));
        this.Settings.Add(new Option("Unknown", _offset  + 0x54, 0x03, 1, Option.OptionType.Boolean));
        this.Settings.Add(new Option("Unknown", _offset  + 0x54, 0x04, 2, Option.OptionType.Slider)); 
        this.Settings.Add(new Option("Unknown", _offset  + 0x56, 0x00, 1, Option.OptionType.Boolean));
        this.Settings.Add(new Option("Unknown", _offset  + 0x56, 0x01, 1, Option.OptionType.Boolean));
        this.Settings.Add(new Option("Unknown", _offset  + 0x57, 0x00, 2, Option.OptionType.Slider)); 
        this.Settings.Add(new Option("Unknown", _offset  + 0x57, 0x03, 1, Option.OptionType.Boolean)); 
        this.Settings.Add(new Option("Unknown", _offset  + 0x57, 0x00, 4, Option.OptionType.Slider));
    }

    public void Save()
    {
        File.WriteAllBytes(_path, _optionsFile);
    }

    public int Read(Option option)
    {
        return (_optionsFile[option.DataInfo.Offset] >> option.DataInfo.BitOffset) & ((1 << option.DataInfo.Size) - 1);
    }

    public void Write(Option option, int value)
    {
        _optionsFile[option.DataInfo.Offset] = (byte)((_optionsFile[option.DataInfo.Offset] & ~((1 << option.DataInfo.Size) - 1) << option.DataInfo.BitOffset) | ((value & ((1 << option.DataInfo.Size) - 1)) << option.DataInfo.BitOffset));
    }
}
