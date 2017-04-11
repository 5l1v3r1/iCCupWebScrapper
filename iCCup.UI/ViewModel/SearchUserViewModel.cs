﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using iCCup.BL.Contracts;
using iCCup.DATA.Models;
using iCCup.UI.Tabablz;

namespace iCCup.UI.ViewModel
{
    public class SearchUserViewModel : ViewModelBase
    {
        private readonly IScrapperService _scrapper;

        public RelayCommand SearchPlayerCommand =>
            new RelayCommand(async () => await Task.Factory.StartNew(async () => await SearchPlayerTask()));

        public RelayCommand NextPageCommand =>
            new RelayCommand(async () => await SearchNavigate(true));

        public RelayCommand PrevPageCommand =>
            new RelayCommand(async () => await SearchNavigate(false));

        public RelayCommand GetPlayerProfileCommand =>
            new RelayCommand(() =>
            {
                var profile = _scrapper.GetUserGameProfile(SelectedUserSearch.Url);
            });

        public SearchUserViewModel(IScrapperService scrapper, HeaderViewModel hvm)
        {
            _scrapper = scrapper;

            Hvm = hvm;
            Header = PlayerName ?? "Search";
            Players = new ObservableCollection<UserSearch>();
        }

        private async Task SearchPlayerTask()
        {
            var searchResults = await _scrapper.SearchPlayer(PlayerName ?? "");

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Players.Clear();
                NextPage = searchResults.Item2;
                PrevPage = searchResults.Item3;
            });

            foreach (var player in searchResults.Item1)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => Players.Add(player));
            }
        }

        private async Task SearchNavigate(bool ahead)
        {
            var searchResults = await _scrapper.SearchPlayer(ahead
                ? new Uri(NextPage)
                : new Uri(PrevPage));

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Players.Clear();
                NextPage = searchResults.Item2;
                PrevPage = searchResults.Item3;
            });

            foreach (var player in searchResults.Item1)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => Players.Add(player));
            }
        }

        #region Props

        private HeaderViewModel _hvm;
        public HeaderViewModel Hvm
        {
            get { return _hvm; }
            set { _hvm = value; RaisePropertyChanged(() => Hvm);}
        }

        public string Header
        {
            get { return Hvm.Header; }
            set { Hvm.Header = value; RaisePropertyChanged(() => Hvm); }
        }

        private string _playerName;
        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                _playerName = value;
                if (!string.Equals(_playerName, string.Empty))
                {
                    SearchPlayerCommand.Execute(true);
                    RaisePropertyChanged(() => Header);
                }
                RaisePropertyChanged(() => PlayerName);
            }
        }

        private UserSearch _selectedUserSearch = new UserSearch { Nickname = "Search" };
        public UserSearch SelectedUserSearch
        {
            get { return _selectedUserSearch; }
            set { _selectedUserSearch = value; RaisePropertyChanged(() => SelectedUserSearch); }
        }

        private ObservableCollection<UserSearch> _players;
        public ObservableCollection<UserSearch> Players
        {
            get { return _players; }
            set { _players = value; RaisePropertyChanged(() => Players); }
        }

        private string _nextPage;
        public string NextPage
        {
            get { return _nextPage; }
            set
            {
                AllowNextPage = !Equals(value, string.Empty);

                _nextPage = value;
                RaisePropertyChanged(() => NextPage);
            }
        }

        // Currently used for disable button.
        private Boolean _allowNextPage;
        public Boolean AllowNextPage
        {
            get { return _allowNextPage; }
            set { _allowNextPage = value; RaisePropertyChanged(() => AllowNextPage); }
        }

        private string _prevPage;
        public string PrevPage
        {
            get { return _prevPage; }
            set
            {
                AllowPrevPage = !Equals(value, string.Empty);

                _prevPage = value;
                RaisePropertyChanged(() => PrevPage);
            }
        }

        // Currently used for disable button.
        private Boolean _allowPrevPage;
        public Boolean AllowPrevPage
        {
            get { return _allowPrevPage; }
            set { _allowPrevPage = value; RaisePropertyChanged(() => AllowPrevPage); }
        }

        #endregion
    }
}
