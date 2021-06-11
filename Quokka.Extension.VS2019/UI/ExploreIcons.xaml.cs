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

namespace Quokka.Extension.VS2019.UI
{
    /// <summary>
    /// Interaction logic for ExploreIcons.xaml
    /// </summary>
    public partial class ExploreIcons : UserControl
    {
        public ExploreIcons()
        {
            // keep reference to interactivity assembly
            var anchor = typeof(System.Windows.Interactivity.Behavior);
            if (anchor == null)
                throw new Exception();

            InitializeComponent();
            DataContext = new ExploreIconsViewModel((a) => {
                Dispatcher.BeginInvoke(a);
            });
            SearchField.Focus();
        }
    }
}
