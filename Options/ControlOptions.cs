using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCEOptionsEditor;

public struct BindInfo
{
    public int Offset;
    public BindOption.ControllerButton DefaultBind;
}

public class BindOption(
    string name,
    int offset,
    BindOption.ControllerButton defaultBind,
    BindOption.BindCategory bindCategory = BindOption.BindCategory.Standard)
{
    
    public enum BindCategory : byte
    {
        Standard,
        Creative,
        MiniGameLobby,
        Spectating,
        GlideMiniGame
    }

    public enum ControllerButton
    {
        BUTTON_NONE,
        BUTTON_A,
        BUTTON_B,
        BUTTON_X,
        BUTTON_Y,
        BUTTON_DP_LEFT,
        BUTTON_DP_RIGHT,
        BUTTON_DP_UP,
        BUTTON_DP_DOWN,
        BUTTON_RB,
        BUTTON_LB,
        BUTTON_RT,
        BUTTON_LT,
        BUTTON_RS,
        BUTTON_LS,
    }

    public readonly string Name = name;
    public BindInfo BindInfo = new()
    {
        Offset = offset,
        DefaultBind = defaultBind,
    };
    public readonly BindCategory Category = bindCategory;
}

public class BindOptions
{
    private readonly int _offset; // incase OPTIONSVERSION is inside the main options file, like Wii U Edition.
    public readonly List<BindOption> Bindings;
    private readonly byte[] _optionsFile;

    public BindOptions(byte[] optionsFile, int offset)
    {
        this._offset = offset + 0xA4;
        this._optionsFile = optionsFile;
        this.Bindings = new List<BindOption>();
        // Bindings are from the docs that I wrote at https://team-lodestone.github.io/Documentation/LCE/Game/Options.html
        // Standard
        this.Bindings.Add(new BindOption("Use", _offset + 1, BindOption.ControllerButton.BUTTON_LT, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Action", _offset + 2, BindOption.ControllerButton.BUTTON_RT, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Jump", _offset, BindOption.ControllerButton.BUTTON_A, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Drop", _offset + 6, BindOption.ControllerButton.BUTTON_B, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Inventory", _offset + 5, BindOption.ControllerButton.BUTTON_Y, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Crafting", _offset + 8, BindOption.ControllerButton.BUTTON_X, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Cycle Held Item Left", _offset + 3, BindOption.ControllerButton.BUTTON_LB, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Cycle Held Item Right", _offset + 4, BindOption.ControllerButton.BUTTON_RB, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Sneak/Dismount", _offset + 7, BindOption.ControllerButton.BUTTON_RS, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Sprint", _offset + 14, BindOption.ControllerButton.BUTTON_NONE, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Pick Block", _offset + 15, BindOption.ControllerButton.BUTTON_NONE, BindOption.BindCategory.Standard));
        this.Bindings.Add(new BindOption("Change Camera Mode", _offset + 9, BindOption.ControllerButton.BUTTON_LS, BindOption.BindCategory.Standard));
        // Creative
        this.Bindings.Add(new BindOption("Fly Left", _offset + 10, BindOption.ControllerButton.BUTTON_DP_LEFT, BindOption.BindCategory.Creative));
        this.Bindings.Add(new BindOption("Fly Right", _offset + 11, BindOption.ControllerButton.BUTTON_DP_RIGHT, BindOption.BindCategory.Creative));
        this.Bindings.Add(new BindOption("Fly Up", _offset + 12, BindOption.ControllerButton.BUTTON_DP_UP, BindOption.BindCategory.Creative));
        this.Bindings.Add(new BindOption("Fly Down", _offset + 13, BindOption.ControllerButton.BUTTON_DP_DOWN, BindOption.BindCategory.Creative));
        // Mini Game Lobby
        this.Bindings.Add(new BindOption("Confirm Ready", _offset + 20, BindOption.ControllerButton.BUTTON_X, BindOption.BindCategory.MiniGameLobby));
        this.Bindings.Add(new BindOption("Vote", _offset + 21, BindOption.ControllerButton.BUTTON_B, BindOption.BindCategory.MiniGameLobby));
        // Spectating
        this.Bindings.Add(new BindOption("Previous Player", _offset + 16, BindOption.ControllerButton.BUTTON_LB, BindOption.BindCategory.Spectating));
        this.Bindings.Add(new BindOption("Next Player", _offset + 17, BindOption.ControllerButton.BUTTON_RB, BindOption.BindCategory.Spectating));
        this.Bindings.Add(new BindOption("Spectate Noise", _offset + 18, BindOption.ControllerButton.BUTTON_RT, BindOption.BindCategory.Spectating));
        this.Bindings.Add(new BindOption("Cancel Spectating", _offset + 19, BindOption.ControllerButton.BUTTON_B, BindOption.BindCategory.Spectating));
        // Glide Mini Game
        this.Bindings.Add(new BindOption("Restart Section", _offset + 22, BindOption.ControllerButton.BUTTON_X, BindOption.BindCategory.GlideMiniGame));
        this.Bindings.Add(new BindOption("Restart Race", _offset + 23, BindOption.ControllerButton.BUTTON_B, BindOption.BindCategory.GlideMiniGame));
        this.Bindings.Add(new BindOption("Look Behind", _offset + 24, BindOption.ControllerButton.BUTTON_Y, BindOption.BindCategory.GlideMiniGame));
    }

    public BindOption.ControllerButton Read(BindOption option)
    {
        return (BindOption.ControllerButton)_optionsFile[option.BindInfo.Offset];
    }
    
    public void Reset(BindOption option)
    {
        _optionsFile[option.BindInfo.Offset] = (byte)option.BindInfo.DefaultBind;
    }

    public void Write(BindOption option, BindOption.ControllerButton button)
    {
        _optionsFile[option.BindInfo.Offset] = (byte)button;
    }
}
