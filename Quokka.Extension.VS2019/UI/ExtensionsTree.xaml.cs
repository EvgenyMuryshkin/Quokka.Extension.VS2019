using Microsoft.VisualStudio.PlatformUI;
using Quokka.Extension.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quokka.Extension.VS2019
{
    /// <summary>
    /// Interaction logic for ExtensionMethodsTree.xaml
    /// </summary>
    public partial class ExtensionsTree : UserControl
    {
        public ExtensionsTree()
        {
            InitializeComponent();
            if (QuokkaExtensionVS2019Package.Instance == null)
                return;

            DataContext = QuokkaExtensionVS2019Package.Instance.Resolve<ExtensionsTreeViewModel>();
        }
    }
}
