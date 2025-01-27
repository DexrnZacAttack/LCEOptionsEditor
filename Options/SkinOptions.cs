using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCEOptionsEditor;

public class SkinOptions
{
    private readonly byte[] _optionsFile;
    private uint[] _favorites = new uint[10];
    private uint _selectedSkin = 0;
    private uint _selectedCape = 0;

    public SkinOptions(byte[] optionsFile)
    {
        this._optionsFile = optionsFile;
        _selectedSkin = BitConverter.ToUInt32(optionsFile, 0x4c);
        _selectedCape = BitConverter.ToUInt32(optionsFile, 0x5c);
        
        // read favorites
        for (int i = 0; i < 10; i++)
        {
            _favorites[i] = BitConverter.ToUInt32(optionsFile, 0x60 + (i * 4));
        }
    }

    public uint getSelectedSkin()
    {
        return _selectedSkin;
    }
    
    public void setSelectedSkin(uint id)
    {
        BitConverter.GetBytes(id).CopyTo(_optionsFile, 0x4c);
        _selectedSkin = id;
    }
    
    public uint getSelectedCape()
    {
        return _selectedCape;
    }
    
    public void setSelectedCape(uint id)
    {
        BitConverter.GetBytes(id).CopyTo(_optionsFile, 0x5c);
        _selectedCape = id;
    }
    
    public uint getFavoriteSkin(byte index)
    {
        return _favorites[index];
    }
    
    public void setFavoriteSkin(byte index, uint id)
    {
        BitConverter.GetBytes(id).CopyTo(_optionsFile, 0x60 + (index * 4));
        _favorites[index] = id;
    }
}
