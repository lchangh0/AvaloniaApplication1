using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using GenLib;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ap2Tool.ViewModels
{
    public partial class FontListViewModel : ViewModelBase
    {
        public string Title { get; set; } = "Font List";

        public ObservableCollection<CFont> Fonts { get; }

        public FontListViewModel()
        {
            Fonts = new ObservableCollection<CFont>();
        }

        public class CFont
        {
            public int Number { get; set; }
            public string? FontName { get; set; }
            public string? FontFamilyName { get; set; }
        }

        [RelayCommand]
        public void HandleCommandScan()
        {
            List<string> fontNames = CSkiaSharpLib.GetAllFontNames();
            fontNames.Sort();

            Fonts.Clear();

            int n = 0;
            foreach (string strFontName in fontNames)
            {
                n++;

                CSkiaSharpLib.CFontAttribute fontAttr = new CSkiaSharpLib.CFontAttribute();
                int iFontSize = 10;
                SKFont font = CSkiaSharpLib.GetFont(strFontName, iFontSize, fontAttr);

                CFont fontItem = new CFont()
                { 
                    Number = n,
                    FontName = strFontName,
                    FontFamilyName = font?.Typeface?.FamilyName,
                };
                Fonts.Add(fontItem);
            }
        }

    }
}
