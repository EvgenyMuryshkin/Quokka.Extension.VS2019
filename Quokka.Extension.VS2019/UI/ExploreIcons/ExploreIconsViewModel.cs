using Quokka.Extension.Interop;
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

namespace Quokka.Extension.VS2019.UI.ExploreIcons
{
    public class ExploreIconViewModel : ViewModel
    {
        public string Name { get; set; }
        public BitmapImage ImageSource { get; set; }
        public ICommand OnClickCommand { get; set; }
    }

    public class ExploreIconsViewModel : ViewModel
    {
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
        public ExploreIconsViewModel(Action<Action> invokeOnUIThread)
        {
            _invokeOnUIThread = invokeOnUIThread;
            _searchTermSubject
                .ObserveOn(Scheduler.CurrentThread)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(s => _invokeOnUIThread(() => Search(s)));

            SearchCommand = new Command(() => Search(SearchTerm));
            Search("");
        }

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
            foreach (var iconsType in ExtensionCatalogue.IconTypes)
            {
                var names = Enum.GetNames(iconsType);
                var values = Enum.GetValues(iconsType).OfType<object>().Select(v => (int)v);

                var pairs = names.Zip(values, (n,v) => new { Type = iconsType, Name = n, Search = $"{iconsType.Name}.{n}".ToLower(), Value = v });
                var matching = pairs.Where(p => p.Search.Contains(searchTerm)).ToList();
                foreach (var pair in matching)
                {
                    var iconViewModel = new ExploreIconViewModel()
                    {
                        Name = $"{iconsType.Name}.{pair.Name}",
                        OnClickCommand = new Command(() => OnIconSelected(pair.Type, pair.Value))
                    };

                    var iconIndex = pair.Value;
                    var collectionName = iconsType.Name;


                    var maxPerCollection = 255;
                    var partIndex = iconIndex / 255;
                    var iconPosition = iconIndex % 255;
                    var startIndex = partIndex * maxPerCollection;

                    var collectionPartName = $"{iconsType.Name}_{startIndex}_{startIndex + maxPerCollection - 1}";
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
                                    icon.SetPixel(col, row, image.GetPixel(iconPosition * 16 + col, row));
                                }
                            }

                            iconViewModel.ImageSource = BitmapToImageSource(icon);
                        }
                    }
                    
                    Icons.Add(iconViewModel);
                    if (Icons.Count >= 100)
                        break;
                }

                if (Icons.Count >= 100)
                    break;
            }

            SearchSummary = $"Showing {Icons.Count} of {ExtensionCatalogue.TotalIconsCount}";
        }
    }
}
