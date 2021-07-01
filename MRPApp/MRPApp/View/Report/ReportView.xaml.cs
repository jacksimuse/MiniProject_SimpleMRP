using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MRPApp.View.Report
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReportView : Page
    {
        public ReportView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitControls();

                //DisplayChart();
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 ReportView Loaded : {ex}");
                throw ex;
            }
        }

        private void DisplayChart(List<Model.Report> list)
        {
            int[] schAmount = list.Select(a => (int)a.SchAmount).ToArray();
            int[] prcOKAmounts = list.Select(a => (int)a.PrcOKAmount).ToArray();
            int[] prcFailAmounts = list.Select(a => (int)a.PrcFailAmount).ToArray();

            var series1 = new LiveCharts.Wpf.ColumnSeries
            {
                Title = "계획수량",
                Fill = new SolidColorBrush(Colors.Green), 
                Values = new LiveCharts.ChartValues<int>(schAmount)
            };
            var series2 = new LiveCharts.Wpf.ColumnSeries
            {
                Title = "성공수량",
                Fill = new SolidColorBrush(Colors.Blue),

                Values = new LiveCharts.ChartValues<int>(prcOKAmounts)
            };
            var series3 = new LiveCharts.Wpf.ColumnSeries
            {
                Title = "실패수량",
                Fill = new SolidColorBrush(Colors.Red),

                Values = new LiveCharts.ChartValues<int>(prcFailAmounts)
            };

            // 차트할당
            ChtReport.Series.Clear();
            ChtReport.Series.Add(series1);
            ChtReport.Series.Add(series2);
            ChtReport.Series.Add(series3);
            // x축 좌표값을 날짜로 표시
            ChtReport.AxisX.First().Labels = list.Select(a => a.PrcDate.ToString("yyyy-MM-dd")).ToList();
        }

        private void InitControls()
        {
            DtpSearchStartDate.SelectedDate = DateTime.Now.AddDays(-7);
            DtpSearchDate.SelectedDate = DateTime.Now;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidInputs())
            {
                var startDate = ((DateTime)DtpSearchStartDate.SelectedDate).ToString("yyyy-MM-dd");
                var endDate = ((DateTime)DtpSearchDate.SelectedDate).ToString("yyyy-MM-dd");
                var searchResult = Logic.DataAccess.GetReportDatas(startDate, endDate, Commons.PLANTCODE);
                DisplayChart(searchResult);
            }
        }

        // 날짜가 빠져있거나, StartDate가 EndDate보다 최신이면 검색하면 안됨
        private bool IsValidInputs()
        {
            var result = true;

            if (DtpSearchStartDate.SelectedDate == null || DtpSearchDate.SelectedDate == null)
            {
                Commons.ShowMessageAsync("검색", "검색 할 일자를 선택하세요.");
                result = false;
            }

            if (DtpSearchStartDate.SelectedDate > DtpSearchDate.SelectedDate)
            {
                Commons.ShowMessageAsync("검색", "시작 일자가 종료 일자보다 최신 일 수 없습니다.");
                result = false;
            }

            return result;
        }
    }
}
