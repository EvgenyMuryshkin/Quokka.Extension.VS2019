using Quokka.Extension.Interface;
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

namespace Quokka.Extension.VS2022.UI.ExploreIcons
{
    /// <summary>
    /// Interaction logic for ExploreIcons.xaml
    /// </summary>
    public partial class ExploreIconsView : UserControl
    {
        public ExploreIconsView()
        {
            // keep reference to interactivity assembly
            var anchor = typeof(System.Windows.Interactivity.Behavior);
            if (anchor == null)
                throw new Exception();

            InitializeComponent();
            DataContext = new ExploreIconsViewModel(
                QuokkaExtensionVS2022Package.Instance.Resolve<IExtensionIconResolver>(),
                (a) => {
                Dispatcher.BeginInvoke(a);
            });
            SearchField.Focus();
        }
    }
}
