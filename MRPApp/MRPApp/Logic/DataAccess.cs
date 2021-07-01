using MRPApp.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPApp.Logic
{
    public class DataAccess
    {
        // 세팅테이블에서 데이터 가져오기
        public static List<Settings> GetSettings()
        {
            List<Model.Settings> settings;

            using (var ctx = new MRPEntities()) 
                settings = ctx.Settings.ToList(); // SELECT

            return settings;
        }

        internal static int SetSettings(Settings item)
        {
            using (var ctx = new MRPEntities())
            {
                ctx.Settings.AddOrUpdate(item); //INSERT or UPDATE
                return ctx.SaveChanges(); // COMMIT
            }
        }

        internal static int Delsetting(Settings item)
        {
            using (var ctx = new MRPEntities())
            {
                var obj = ctx.Settings.Find(item.BasicCode);    // 검색한 실제 데이터를 삭제
                ctx.Settings.Remove(obj); // DELETE
                return ctx.SaveChanges();
            }
        }

        internal static List<Schedules> GetSchedules()
        {
            List<Model.Schedules> list;

            using (var ctx = new MRPEntities())
                list = ctx.Schedules.ToList();

            return list;
        }

        internal static int SetSchedule(Schedules item)
        {
            using (var ctx = new MRPEntities())
            {
                ctx.Schedules.AddOrUpdate(item); //INSERT or UPDATE
                return ctx.SaveChanges(); // COMMIT
            }
        }

        internal static List<Process> GetProcesses()
        {
            List<Model.Process> list;

            using (var ctx = new MRPEntities())
                list = ctx.Process.ToList();

            return list;
        }

        internal static int SetProcess(Process item)
        {
            using (var ctx = new MRPEntities())
            {
                ctx.Process.AddOrUpdate(item); // INSERT | UPDATE
                return ctx.SaveChanges(); // COMMIT
            }
        }

        internal static List<Report> GetReportDatas(string startDate, string endDate, string plantCODE)
        {
            var connString = ConfigurationManager.ConnectionStrings["MRPConnString"].ToString();
            var list = new List<Report>();
            var lastObj = new Model.Report(); // 추가 : 최종 Report값 담는 변수

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();    // 중요!!
                var sqlQuery = $@"select sch.SchIdx, sch.PlantCode, sch.SchAmount, PrcDate, PrcOKAmount, PrcFailAmount
                                    from Schedules Sch
                                   inner join (
                                  select smr.SchIdx, smr.PrcDate,
	                                     sum(smr.prcok) PrcOKAmount, sum(smr.prcfail) PrcFailAmount
                                    from (select p.SchIdx, p.PrcDate,
			                                     case p.PrcResult WHEN 1 then 1 else 0 end PrcOK,
			                                     case p.PrcResult when 0 then 1 else 0 end PrcFail
		                                    from Process as p
		                               ) smr
                                   GROUP by smr.SchIdx, smr.PrcDate
                                   ) prc
                                      on sch.SchIdx = prc.SchIdx
                                   where sch.PlantCode = '{plantCODE}'
                                     and prc.PrcDate between '{startDate}' and '{endDate}'";

                var cmd = new SqlCommand(sqlQuery, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var tmp = new Report
                    {
                        SchIdx = (int)reader["SchIdx"],
                        PlantCode = reader["PlantCode"].ToString(),
                        PrcDate = DateTime.Parse(reader["PrcDate"].ToString()),
                        SchAmount = (int)reader["SchAmount"],
                        PrcOKAmount = (int)reader["PrcOKAmount"],
                        PrcFailAmount = (int)reader["PrcFailAmount"],
                    };
                    list.Add(tmp);
                    lastObj = tmp; // 마지막 값을 할당
                }

                // 시작일부터 종료일까지 없는 값 만들어주는 로직
                var DtStart = DateTime.Parse(startDate);
                var DtEnd = DateTime.Parse(endDate);
                var DtCurrent = DtStart;

                while (DtCurrent < DtEnd)
                {
                    var count = list.Where(c => c.PrcDate.Equals(DtCurrent)).Count();
                    if (count == 0)
                    {
                        // 새로운 Report(없는 날짜)
                        var tmp = new Report
                        {
                            SchIdx = lastObj.SchIdx,
                            PlantCode = lastObj.PlantCode,
                            PrcDate = DtCurrent,
                            SchAmount = 0,
                            PrcOKAmount = 0,
                            PrcFailAmount = 0
                        };
                        list.Add(tmp);
                    }
                    DtCurrent = DtCurrent.AddDays(1); // 날하루 증가
                }
            }

            list.Sort((reportA, reportB) => reportA.PrcDate.CompareTo(reportB.PrcDate)); // 가장오래된 날짜부터 오름차순 정령
            return list;
        }
    }
}
