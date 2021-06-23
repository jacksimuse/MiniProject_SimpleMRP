using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MRPApp.View.Store
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddStore : Page
    {
        public AddStore()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LblStoreName.Visibility = LblStoreLocation.Visibility = Visibility.Hidden;
            TxtStoreID.Text = TxtStoreName.Text = TxtStoreLocation.Text = "";
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private bool IsValid = true; // 지역변수 --> 전역변수

        public bool IsValidInput()
        {
            if (string.IsNullOrEmpty(TxtStoreName.Text))
            {
                LblStoreName.Visibility = Visibility.Visible;
                LblStoreName.Text = "창고명을 입력하세요";
                IsValid = false;
            }
            else
            {
                //var cnt = Logic.DataAccess.GetStores().Where(u => u.StoreName.Equals(TxtStoreName.Text)).Count();
                //if (cnt > 0)
                //{
                //    LblStoreName.Visibility = Visibility.Visible;
                //    LblStoreName.Text = "중복된 창고명이 존재합니다";
                //    IsValid = false;
                //}
            }

            if (string.IsNullOrEmpty(TxtStoreLocation.Text))
            {
                LblStoreLocation.Visibility = Visibility.Visible;
                LblStoreLocation.Text = "창고위치를 입력하세요";
                IsValid = false;
            }

            return IsValid;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true; // 입력된 값이 모두 만족하는지 판별하는 플래그
            LblStoreName.Visibility = LblStoreLocation.Visibility = Visibility.Hidden;

            //var store = new Model.Store();
            //isValid = IsValidInput(); // 유효성체크(필수)

            //if (isValid)
            //{
            //    //MessageBox.Show("DB 입력처리!");
            //    store.StoreName = TxtStoreName.Text;
            //    store.StoreLocation = TxtStoreLocation.Text;

            //    try
            //    {
            //        var result = Logic.DataAccess.SetStore(store);  // Logic.DataAccess.SetUser(user);
            //        if (result == 0)
            //        {
            //            // 수정 안됨
            //            Commons.LOGGER.Error("AddStore.xaml.cs 창고정보 저장오류 발생");
            //            Commons.ShowMessageAsync("오류", "저장시 오류가 발생했습니다");                       
            //            return;
            //        }
            //        else
            //        {
            //            NavigationService.Navigate(new StoreList());
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Commons.LOGGER.Error($"예외발생 : {ex}");
            //    }
            //}
        }

        private void TxtStoreName_LostFocus(object sender, RoutedEventArgs e)
        {
            IsValidInput();
        }

        private void TxtStoreLocation_LostFocus(object sender, RoutedEventArgs e)
        {
            IsValidInput();
        }
    }
}
