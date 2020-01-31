﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
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

namespace mcv2.MainViewPlugin
{
    /// <summary>
    /// Interaction logic for CommentPostPanel.xaml
    /// </summary>
    public partial class YouTubeCommentPostPanel : UserControl
    {
        public YouTubeCommentPostPanel()
        {
            InitializeComponent();
        }
    }
    class YouTubeCommentPostPanelViewModel : ViewModelBase
    {
        private readonly IModel _host;

        public bool CanPostComment { get; set; } = true;//常にtrueにして、投稿できなかったらエラーを吐けばいいのでは？流石に未ログインは投稿できないようにしたい
        public ICommand PostCommentCommand { get; }
        public string Comment { get; set; }
        public YouTubeCommentPostPanelViewModel()
        {

        }
        public YouTubeCommentPostPanelViewModel(IModel host)
        {
            _host = host;
            PostCommentCommand = new RelayCommand(PostComment);
        }
        private void PostComment()
        {
            _host.PostCommentAsync(new YouTubeLiveCommentDataToPost
            {
            });
        }
        //ログイン状況を照会する
    }
    public interface IYouTubeLiveCommentDataToPost : ICommentDataToPost
    {

    }
    class YouTubeLiveCommentDataToPost : IYouTubeLiveCommentDataToPost
    {
        public string Message { get; set; }
    }
}
