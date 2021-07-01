-- 1. PrcResult���� ���������� ���а����� �ٸ� (����)�÷����� �и� ��� ������ ���̺�
select p.SchIdx, p.PrcDate,
		case p.PrcResult WHEN 1 then 1 end as PrcOK,
		case p.PrcResult when 0 then 1 end as PrcFail
from Process as p

-- 2. �հ����� ��� �������̺�
select smr.SchIdx, smr.PrcDate,
	   sum(smr.prcok) ��������, sum(smr.prcfail) ���а���
  from (select p.SchIdx, p.PrcDate,
			   case p.PrcResult WHEN 1 then 1 end PrcOK,
			   case p.PrcResult when 0 then 1 end PrcFail
		  from Process as p
		) smr
 GROUP by smr.SchIdx, smr.PrcDate

 -- 3.0 ���ι�
select *
  from Schedules Sch
 inner join Process prc
    on sch.SchIdx = prc.SchIdx

 -- 3.1 2�����(�������̺�)�� Schedules ���̺� �����ؼ� ���ϴ� ��� ����
 select sch.SchIdx, sch.PlantCode, sch.SchAmount, PrcDate, ��������, ���а���
   from Schedules Sch
  inner join (
 select smr.SchIdx, smr.PrcDate,
	     sum(smr.prcok) ��������, sum(smr.prcfail) ���а���
   from (select p.SchIdx, p.PrcDate,
			     case p.PrcResult WHEN 1 then 1 end PrcOK,
			     case p.PrcResult when 0 then 1 end PrcFail
		    from Process as p
		) smr
  GROUP by smr.SchIdx, smr.PrcDate
  ) prc
  on sch.SchIdx = prc.SchIdx

