using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using Quokka.Extension.Scaffolding;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Quokka.Extension.VS2022.UI.ExploreIcons
{
    public class ExploreIconViewModel : ViewModel
    {
        public string Name { get; set; }
        public object ImageSource { get; set; }
        public ICommand OnClickCommand { get; set; }
    }

    public class ExploreIconsViewModel : ViewModel
    {
        readonly IExtensionIconResolver _extensionIconResolver;
        Subject<string> _searchTermSubject = new Subject<string>();
        Action<Action> _invokeOnUIThread;

        public ObservableCollection<ExploreIconViewModel> Icons { get; set; } = new ObservableCollection<ExploreIconViewModel>();
        string _searchTerm = "";
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                _searchTermSubject.OnNext(value);
                OnPropertyChanged();
            }
        }

        string _searchSummary = "";
        public string SearchSummary
        {
            get => _searchSummary;
            set
            {
                _searchSummary = value;
                OnPropertyChanged();
            }
        }
        
        string _snippet = "";
        public string Snippet
        {
            get => _snippet;
            set
            {
                _snippet = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; set; }
        public ExploreIconsViewModel(IExtensionIconResolver extensionIconResolver, Action<Action> invokeOnUIThread)
        {
            _extensionIconResolver = extensionIconResolver;
            _invokeOnUIThread = invokeOnUIThread;
            _searchTermSubject
                .ObserveOn(Scheduler.CurrentThread)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(s => _invokeOnUIThread(() => Search(s)));

            SearchCommand = new Command(() => Search(SearchTerm));
            Search("");
        }

        void OnIconSelected(Type iconType, int icon)
        {
            var values = Enum.GetValues(iconType);
            var value = values.GetValue(icon);

            Snippet = $@"using System;
using Quokka.Extension.Interop;
using System.Threading.Tasks;

namespace Extensions
{{
    [ExtensionClass]
    public class QuokkaExtensionClass
    {{
        [ExtensionMethod(icon: {iconType.Name}.{value})]
        public static Task QuokkaExtensionMethod()
        {{
            return Task.CompletedTask;
        }}
    }}
}}
";
        }

        void Search(string searchTerm)
        {
            Icons.Clear();

            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();

            searchTerm = searchTerm.ToLower();

            var matchingIcons = ExtensionCatalogue.IconTypes.SelectMany(iconsType =>
            {
                var names = Enum.GetNames(iconsType);
                var values = Enum.GetValues(iconsType).OfType<object>().Select(v => (int)v);

                var pairs = names.Zip(values, (n, v) => new { Type = iconsType, Name = n, Search = $"{iconsType.Name}.{n}".ToLower(), Value = v });
                var matching = pairs.Where(p => p.Search.Contains(searchTerm)).ToList();

                return matching;
            }).ToList();

            foreach (var pair in matchingIcons)
            {
                var iconViewModel = new ExploreIconViewModel()
                {
                    Name = $"{pair.Type.Name}.{pair.Name}",
                    ImageSource = _extensionIconResolver.Resolve(new ExtensionMethodIcon(pair.Type, pair.Value)),
                    OnClickCommand = new Command(() => OnIconSelected(pair.Type, pair.Value))
                };

                Icons.Add(iconViewModel);
                if (Icons.Count >= 100)
                    break;
            }

            SearchSummary = $"Showing {Icons.Count} of {matchingIcons.Count}";
        }
    }
}
