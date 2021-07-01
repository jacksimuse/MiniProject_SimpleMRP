-- 1. PrcResult에서 성공개수와 실패갯수를 다른 (가상)컬럼으로 분리 결과 가상의 테이블
select p.SchIdx, p.PrcDate,
		case p.PrcResult WHEN 1 then 1 end as PrcOK,
		case p.PrcResult when 0 then 1 end as PrcFail
from Process as p

-- 2. 합계집계 결과 가상테이블
select smr.SchIdx, smr.PrcDate,
	   sum(smr.prcok) 성공개수, sum(smr.prcfail) 실패개수
  from (select p.SchIdx, p.PrcDate,
			   case p.PrcResult WHEN 1 then 1 end PrcOK,
			   case p.PrcResult when 0 then 1 end PrcFail
		  from Process as p
		) smr
 GROUP by smr.SchIdx, smr.PrcDate

 -- 3.0 조인문
select *
  from Schedules Sch
 inner join Process prc
    on sch.SchIdx = prc.SchIdx

 -- 3.1 2번결과(가상테이블)와 Schedules 테이블 조인해서 원하는 결과 도출
 select sch.SchIdx, sch.PlantCode, sch.SchAmount, PrcDate, 성공개수, 실패개수
   from Schedules Sch
  inner join (
 select smr.SchIdx, smr.PrcDate,
	     sum(smr.prcok) 성공개수, sum(smr.prcfail) 실패개수
   from (select p.SchIdx, p.PrcDate,
			     case p.PrcResult WHEN 1 then 1 end PrcOK,
			     case p.PrcResult when 0 then 1 end PrcFail
		    from Process as p
		) smr
  GROUP by smr.SchIdx, smr.PrcDate
  ) prc
  on sch.SchIdx = prc.SchIdx

