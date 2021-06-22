using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Quokka.Extension.VS2019.Services
{
    public class ExtensionIconResolver : IExtensionIconResolver
    {
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        ExtensionMethodIcon ToResourceIcon(ExtensionMethodIcon extensionIcon)
        {
            if (extensionIcon.IconType == typeof(TopLevelIcon))
            {
                switch ((TopLevelIcon)extensionIcon.IconValue)
                {
                    case TopLevelIcon.Translate: return RemixIcon.RiAliensFill;
                    case TopLevelIcon.BitStream: return SimpleIcons.SiLaunchpad;
                    case TopLevelIcon.Program: return Ionicons5.IoHardwareChipSharp;
                    case TopLevelIcon.Generic: return Grommet_Icons.GrTools;
                }
            }

            return extensionIcon;
        }


        public object Resolve(ExtensionMethodIcon extensionIcon)
        {
            extensionIcon = ToResourceIcon(extensionIcon);
            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();

            var iconIndex = extensionIcon.IconValue;
            var collectionName = extensionIcon.IconType.Name;


            var maxPerCollection = 255;
            var partIndex = iconIndex / 255;
            var iconPosition = iconIndex % 255;
            var startIndex = partIndex * maxPerCollection;

            var collectionPartName = $"{extensionIcon.IconType.Name}_{startIndex}_{startIndex + maxPerCollection - 1}";
            var resourceName = resources.Single(r => r.Contains(collectionPartName));

            var pngStream = assembly.GetManifestResourceStream(resourceName);
            using (var image = new Bitmap(pngStream))
            {
                using (var icon = new Bitmap(16, 16))
                {
                    for (var row = 0; row < 16; row++)
                    {
                        for (var col = 0; col < 16; col++)
                        {
                            var key = EnvironmentColors.ToolWindowTextBrushKey;
                            var color = VSColorTheme.GetThemedColor(key);
                            var isBright = color.GetBrightness() > 0.5;

                            var pixel = image.GetPixel(iconPosition * 16 + col, row);
                            var inverted = Color.FromArgb(pixel.ToArgb() ^ 0xFFFFFF);

                            icon.SetPixel(col, row, isBright ? inverted : pixel);
                        }
                    }

                    return BitmapToImageSource(icon);
                }
            }
        }
    }
}
