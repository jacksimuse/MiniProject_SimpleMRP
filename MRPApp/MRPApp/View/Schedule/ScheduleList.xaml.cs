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

namespace MRPApp.View.Schedule
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScheduleList : Page
    {
        public ScheduleList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadControlData(); // 콤보박스 데이터 로딩
                LoadGridData(); // 테이블데이터 그리드 표시
                InitErrorMessage();
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 StoreList Loaded : {ex}");
                throw ex;
            }
        }

        private void LoadControlData()
        {
            var plantCode = Logic.DataAccess.GetSettings().Where(c => c.BasicCode.Contains("PC01")).ToList();
            CboPlantCode.ItemsSource = plantCode;
            CboGridPlantCode.ItemsSource = plantCode;

            var facilityIds = Logic.DataAccess.GetSettings().Where(c => c.BasicCode.Contains("FAC1")).ToList();
            CboFacilityID.ItemsSource = facilityIds;
        }

        private void InitErrorMessage()
        {
            LblPlantCode.Visibility = LblSchDate.Visibility = LblSchLoadTime.Visibility = LblSchStartTime.Visibility =
                LblSchEndTime.Visibility = LblFacilityID.Visibility = LblSchAmount.Visibility = Visibility.Hidden;
        }

        private void LoadGridData()
        {
            List<Model.Schedules> list = Logic.DataAccess.GetSchedules();
            this.DataContext = list;
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

            var item = new Model.Schedules();
            item.PlantCode = CboPlantCode.SelectedValue.ToString();
            item.SchDate = DateTime.Parse(DtpSchDate.Text);
            item.SchLoadTime = int.Parse(TxtSchLoadTime.Text);

            if (TmpSchStartTime.SelectedDateTime != null)
                item.SchStartTime = TmpSchStartTime.SelectedDateTime.Value.TimeOfDay;
            if (TmpSchEndTime.SelectedDateTime != null)
                item.SchEndTime = TmpSchEndTime.SelectedDateTime.Value.TimeOfDay;

            item.SchFacilityID = CboFacilityID.SelectedValue.ToString();
            item.SchAmount = (int)NudSchAmount.Value;

            item.RegDate = DateTime.Now;
            item.RegID = "MRP";

            try
            {
                var result = Logic.DataAccess.SetSchedule(item);
                if (result == 0)
                {
                    Commons.LOGGER.Error("데이터 입력시 오류발생");
                    await Commons.ShowMessageAsync("오류", "데이터 입력실패!!");
                }
                else
                {
                    Commons.LOGGER.Info($"데이터 입력 성공 : {item.SchIdx}"); // 로그
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

            if (CboPlantCode.SelectedValue == null)
            {
                LblPlantCode.Visibility = Visibility.Visible;
                LblPlantCode.Text = "공장을 선택하세요";
                isValid = false;
            }

            if (string.IsNullOrEmpty(DtpSchDate.Text))
            {
                LblSchDate.Visibility = Visibility.Visible;
                LblSchDate.Text = "공정일을 입력하세요";
                isValid = false;
            }

            if (CboPlantCode.SelectedValue != null && !string.IsNullOrEmpty(DtpSchDate.Text))
            {
                // 공장별로 공정일일 DB값이 있으면 입력되면 안됨
                // PC010001 (수원) 2021-06-24

                var result = Logic.DataAccess.GetSchedules().Where(s => s.PlantCode.Equals(CboPlantCode.SelectedValue.ToString()))
                    .Where(d => d.SchDate.Equals(DateTime.Parse(DtpSchDate.Text))).Count();
                if (result > 0)
                {
                    LblSchDate.Visibility = Visibility.Visible;
                    LblSchDate.Text = "해당공장 공정일에 계획이 이미있습니다.";
                    isValid = false;
                }
            }

            if (string.IsNullOrEmpty(TxtSchLoadTime.Text))
            {
                LblSchLoadTime.Visibility = Visibility.Visible;
                LblSchLoadTime.Text = "로드타임을 입력하세요";
                isValid = false;
            }
            
            if (CboFacilityID.SelectedValue == null)
            {
                LblFacilityID.Visibility = Visibility.Visible;
                LblFacilityID.Text = "공정설비를 선택하세요";
                isValid = false;
            }

            if (NudSchAmount.Value <= 0)
            {
                LblSchAmount.Visibility = Visibility.Visible;
                LblSchAmount.Text = "계획수량은 0개 이상입니다.";
                isValid = false;
            }

            return isValid;
        }

        // 수정데이터 검증 메서드
        private bool IsValidUpdates()
        {
            var isValid = true;
            InitErrorMessage();

            if (CboPlantCode.SelectedValue == null)
            {
                LblPlantCode.Visibility = Visibility.Visible;
                LblPlantCode.Text = "공장을 선택하세요";
                isValid = false;
            }

            if (string.IsNullOrEmpty(DtpSchDate.Text))
            {
                LblSchDate.Visibility = Visibility.Visible;
                LblSchDate.Text = "공정일을 입력하세요";
                isValid = false;
            }

            // if (CboPlantCode.SelectedValue != null && !string.IsNullOrEmpty(DtpSchDate.Text))
            //{
            //    // 공장별로 공정일일 DB값이 있으면 입력되면 안됨
            //    // PC010001 (수원) 2021-06-24

            //    var result = Logic.DataAccess.GetSchedules().Where(s => s.PlantCode.Equals(CboPlantCode.SelectedValue.ToString()))
            //        .Where(d => d.SchDate.Equals(DateTime.Parse(DtpSchDate.Text))).Count();
            //    if (result > 0)
            //    {
            //        LblSchDate.Visibility = Visibility.Visible;
            //        LblSchDate.Text = "해당공장 공정일에 계획이 이미있습니다.";
            //        isValid = false;
            //    }
            //}

            if (string.IsNullOrEmpty(TxtSchLoadTime.Text))
            {
                LblSchLoadTime.Visibility = Visibility.Visible;
                LblSchLoadTime.Text = "로드타임을 입력하세요";
                isValid = false;
            }

            if (CboFacilityID.SelectedValue == null)
            {
                LblFacilityID.Visibility = Visibility.Visible;
                LblFacilityID.Text = "공정설비를 선택하세요";
                isValid = false;
            }

            if (NudSchAmount.Value <= 0)
            {
                LblSchAmount.Visibility = Visibility.Visible;
                LblSchAmount.Text = "계획수량은 0개 이상입니다.";
                isValid = false;
            }

            return isValid;
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidUpdates() != true) return;

            var item = GrdData.SelectedItem as Model.Schedules;
            item.PlantCode = CboPlantCode.SelectedValue.ToString();
            item.SchDate = DateTime.Parse(DtpSchDate.Text);
            item.SchLoadTime = int.Parse(TxtSchLoadTime.Text);
            if (TmpSchStartTime.SelectedDateTime != null)
                item.SchStartTime = TmpSchStartTime.SelectedDateTime.Value.TimeOfDay;
            if (TmpSchEndTime.SelectedDateTime != null)
                item.SchEndTime = TmpSchEndTime.SelectedDateTime.Value.TimeOfDay;
            item.SchFacilityID = CboFacilityID.SelectedValue.ToString();
            item.SchAmount = (int)NudSchAmount.Value;

            item.ModDate = DateTime.Now;
            item.ModID = "MRP";

            try
            {
                var result = Logic.DataAccess.SetSchedule(item);
                if (result == 0)
                {
                    Commons.LOGGER.Error("데이터 수정시 오류발생");
                    await Commons.ShowMessageAsync("오류", "데이터 수정실패!!");
                }
                else
                {
                    Commons.LOGGER.Info($"데이터 수정 성공 : {item.SchIdx}"); // 로그
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
            TxtSchIdx.Text = "";
            CboPlantCode.SelectedItem = null;
            DtpSchDate.Text = "";
            TxtSchLoadTime.Text = "";
            TmpSchStartTime.SelectedDateTime = null;
            TmpSchEndTime.SelectedDateTime = null;
            CboFacilityID.SelectedItem = null;
            NudSchAmount.Value = 0;

            CboPlantCode.Focus();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var search = DtpSearchDate.Text;
            var list = Logic.DataAccess.GetSchedules().Where(s => s.SchDate.Equals(DateTime.Parse(search))).ToList(); 
            // list로 통일시켜서 복사해서 사용할때 일일이 바꾸지 않도록 작업
            this.DataContext = list;
        }

        private void GrdData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            ClearInputs();
            try
            {
                var item = GrdData.SelectedItem as Model.Schedules; // 선택된 데이터를 형변환
                
                TxtSchIdx.Text = item.SchIdx.ToString();
                CboPlantCode.SelectedValue = item.PlantCode;
                DtpSchDate.Text = item.SchDate.ToString();
                TxtSchLoadTime.Text = item.SchLoadTime.ToString();
                if (item.SchStartTime != null)
                    TmpSchStartTime.SelectedDateTime = new DateTime(item.SchStartTime.Value.Ticks);
                if (item.SchEndTime != null)
                    TmpSchEndTime.SelectedDateTime = new DateTime(item.SchEndTime.Value.Ticks);
                CboFacilityID.SelectedValue = item.SchFacilityID;
                NudSchAmount.Value = item.SchAmount;

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
