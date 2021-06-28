using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace MRPApp.View.Setting
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingList : Page
    {
        public SettingList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadGridData();

                InitErrorMessage();
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 StoreList Loaded : {ex}");
                throw ex;
            }
        }

        private void InitErrorMessage()
        {
            LblBasicCode.Visibility = LblCodeDesc.Visibility = LblCodeName.Visibility = Visibility.Hidden;
        }

        private void LoadGridData()
        {
            List<Model.Settings> settings = Logic.DataAccess.GetSettings();
            this.DataContext = settings;
        }


        private void BtnEditStore_Click(object sender, RoutedEventArgs e)
        {
            if (GrdData.SelectedItem == null)
            {
                Commons.ShowMessageAsync("창고수정", "수정할 창고를 선택하세요");
                return;
            }

            try
            {
               
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnEditStore_Click : {ex}");
                throw ex;
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
        }

        private async void BtnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidInputs() != true) return;

            var setting = new Model.Settings();
            setting.BasicCode = TxtBasicCode.Text;
            setting.CodeName = TxtCodeName.Text;
            setting.CodeDesc = TxtCodeDesc.Text;
            setting.RegDate = DateTime.Now;
            setting.RegID = "MRP";

            try
            {
                var result = Logic.DataAccess.SetSettings(setting);
                if (result == 0)
                {
                    Commons.LOGGER.Error("데이터 입력시 오류발생");
                    await Commons.ShowMessageAsync("오류", "데이터 입력실패!!");
                }
                else
                {
                    Commons.LOGGER.Info($"데이터 입력 성공 : {setting.BasicCode}"); // 로그
                    ClearInputs();
                    LoadGridData();
                }
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 {ex}");
            }
        }

        // 입력데이터 검증 메서드
        private bool IsValidInputs()
        {
            var isValid = true;
            InitErrorMessage();

            if (string.IsNullOrEmpty(TxtBasicCode.Text))
            {
                LblBasicCode.Visibility = Visibility.Visible;
                LblBasicCode.Text = "코드를 입력하세요.";
                isValid = false;
            }
            else if (Logic.DataAccess.GetSettings().Where(s=> s.BasicCode.Equals(TxtBasicCode.Text)).Count() > 0)
            {
                LblBasicCode.Visibility = Visibility.Visible;
                LblBasicCode.Text = "중복코드가 존재합니다.";
                isValid = false;
            }

            if (string.IsNullOrEmpty(TxtCodeName.Text))
            {
                LblCodeName.Visibility = Visibility.Visible;
                LblCodeName.Text = "코드명을 입력하세요.";
                isValid = false;
            }
            return isValid;
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var setting = GrdData.SelectedItem as Model.Settings;
            setting.CodeName = TxtCodeName.Text;
            setting.CodeDesc = TxtCodeDesc.Text;
            setting.ModDate = DateTime.Now;
            setting.ModID = "MRP";

            try
            {
                var result = Logic.DataAccess.SetSettings(setting);
                if (result == 0)
                {
                    Commons.LOGGER.Error("데이터 수정시 오류발생");
                    await Commons.ShowMessageAsync("오류", "데이터 수정실패!!");
                }
                else
                {
                    Commons.LOGGER.Info($"데이터 수정 성공 : {setting.BasicCode}"); // 로그
                    ClearInputs();
                    LoadGridData();
                }
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 {ex}");
            }
        }

        private void ClearInputs()
        {
            TxtBasicCode.IsReadOnly = false;
            TxtBasicCode.Background = new SolidColorBrush(Colors.White);

            TxtBasicCode.Text = TxtCodeName.Text = TxtCodeDesc.Text = string.Empty;
            TxtBasicCode.Focus();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var search = TxtSearch.Text.Trim();

            var settings = Logic.DataAccess.GetSettings().Where(s => s.CodeName.Contains(search)).ToList();
            this.DataContext = settings;
        }

        private void GrdData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                var setting = GrdData.SelectedItem as Model.Settings; // 선택된 데이터를 형변환
                TxtBasicCode.Text = setting.BasicCode;
                TxtCodeName.Text = setting.CodeName;
                TxtCodeDesc.Text = setting.CodeDesc;

                TxtBasicCode.IsReadOnly = true;
                TxtBasicCode.Background = new SolidColorBrush(Colors.LightGray);
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 {ex}");
                ClearInputs();
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidInputs() != true) return;

            var setting = GrdData.SelectedItem as Model.Settings;

            if (setting == null)
            {
                await Commons.ShowMessageAsync("삭제", "데이터 삭제시 오류발생");
                return;
            }
            else
            {
                try
                {
                    var result = Logic.DataAccess.Delsetting(setting);
                    if (result == 0)
                    {
                        Commons.LOGGER.Error("데이터 삭제시 오류 발생");
                        Commons.ShowMessageAsync("오류", "데이터 삭제 실패!!");
                    }
                    else
                    {
                        Commons.LOGGER.Info($"데이터 삭제 성공 : {setting.BasicCode}");
                        ClearInputs();
                        LoadGridData(); //수정된 값 다시 불러오는 역할
                    }
                }
                catch (Exception ex)
                {
                    Commons.LOGGER.Error($"예외 발생 {ex}");
                }
            }
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BtnSearch_Click(sender, e);
        }
    }
}
