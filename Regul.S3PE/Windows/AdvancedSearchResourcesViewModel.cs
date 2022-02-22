using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Onebeld.Extensions;
using Regul.Base;
using Regul.S3PE.Structures;
using Regul.S3PI;
using Regul.S3PI.Interfaces;

namespace Regul.S3PE.Windows
{
	public class AdvancedSearchResourcesViewModel : ViewModelBase
    {
        private string _keyedWords;
        private ObservableCollection<Resource> _foundResources = new ObservableCollection<Resource>();
        private Resource _selectedResoruce;
        private bool _isIndeterminate;
        private bool _isRunningSearching;
        
        private SynchronizationContext _synchronizationContext;

        public ObservableCollection<Resource> FoundResources
        {
            get => _foundResources;
            set => RaiseAndSetIfChanged(ref _foundResources, value);
        }

        public Resource SelectedResource
        {
            get => _selectedResoruce;
            set
            {
                RaiseAndSetIfChanged(ref _selectedResoruce, value);
                
                CloseWindowWithResult(value);
            }
        }

        private bool IsIndeterminate
        {
            get => _isIndeterminate;
            set => RaiseAndSetIfChanged(ref _isIndeterminate, value);
        }

        public AvaloniaList<Resource> Resources;
        public IPackage Package;

        public string KeyedWords
        {
            get => _keyedWords;
            set => RaiseAndSetIfChanged(ref _keyedWords, value);
        }

        public AdvancedSearchResourcesViewModel()
        {
            _synchronizationContext = SynchronizationContext.Current;
        }

        public void CloseWindow()
        {
            AdvancedSearchResources foundAdvancedSearchResources = WindowsManager.FindModalWindow<AdvancedSearchResources>();
            WindowsManager.OtherModalWindows.Remove(foundAdvancedSearchResources);

            foundAdvancedSearchResources?.Close(null);
        }
        
        private void CloseWindowWithResult(Resource resource)
        {
            if (resource == null) return;
            
            AdvancedSearchResources foundAdvancedSearchResources = WindowsManager.FindModalWindow<AdvancedSearchResources>();
            WindowsManager.OtherModalWindows.Remove(foundAdvancedSearchResources);

            foundAdvancedSearchResources?.Close(resource);
        }

        private async void InitSearch()
        {
            _isRunningSearching = true;
            
            FoundResources.Clear();

            IsIndeterminate = true;

            string[] keys;

            if (KeyedWords.Contains(","))
                keys = KeyedWords.Replace(" ", "").Split(',');
            else if (KeyedWords.Contains(";"))
                keys = KeyedWords.Replace(" ", "").Split(';');
            else
                keys = KeyedWords.Split(' ');

            await Task.Run(() =>
            {
                foreach (Resource resource in Resources)
                {
                    IResource res = WrapperDealer.GetResource(Package, resource.IndexEntry);

                    string text = new StreamReader(res.Stream).ReadToEnd();

                    foreach (string key in keys)
                    {
                        if (text.Contains(key))
                        {
                            _synchronizationContext.Send(state =>
                            {
                                FoundResources.Add(resource);
                            }, "");

                            break;
                        }
                    }
                }
            });
            
            IsIndeterminate = false;
            _isRunningSearching = false;
        }
    }
}