

/****** Object:  StoredProcedure [cTest].[Utility_GetRegularDateVal]    Script Date: 2014/6/23 17:39:31 ******/
IF EXISTS (
  SELECT * 
    FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = N'cTest'
     AND SPECIFIC_NAME = N'Utility_GetRegularDateVal' 
)
   DROP PROCEDURE [cTest].[Utility_GetRegularDateVal] 
GO

CREATE PROCEDURE [cTest].[Utility_GetRegularDateVal] 
	@seqname varchar(32),
	@length int,
	@currdate varchar(10),
	@step int,
	@code varchar(32) OUTPUT
AS
begin	
	SET NOCOUNT ON
	declare @datetime nvarchar(50)
	declare @StepValue int
	declare @CurentVal int
	declare @PreFixString nvarchar(50)
	declare @PostFixString nvarchar(50)
	declare @DataTimeString nvarchar(50)
	-- 按照当前日期获取流水号
	select @PreFixString=PreFixString, @PostFixString=PostFixString from [cTest].[cfg_SysSequence] WITH (ROWLOCK,XLOCK) where SequenceName = @seqname
	select @CurentVal=currentvalue, @StepValue=StepValue from [cTest].[cfg_SysSequence] where SequenceName = @seqname and currdate = @currdate
	select @datetime = replace(@currdate,'-','')
	if(@step > 0)
		select @StepValue = @step
	if (@CurentVal  is null)
		begin
			begin tran T1
			update [cTest].[cfg_SysSequence] set currentvalue = @StepValue, currdate = @currdate where SequenceName = @seqname
			if(@PostFixString is null)
			begin
				set @code = @PreFixString + @datetime + right(replicate(0,@length) + '1', @length)
				select SeedType = @seqname, SeedValue = @code, SeedMin = 1, SeedMax = @StepValue
			end
			else
			begin
				set @code = @PreFixString + @datetime + right(replicate(0,@length) + '1', @length) + @PostFixString
				select SeedType = @seqname, SeedValue = @code, SeedMin = 1, SeedMax = @StepValue
			end
			commit tran T1
		end
	else
		begin
			begin tran T2
			update [cTest].[cfg_SysSequence] set currentvalue = @CurentVal + @StepValue  where SequenceName = @seqname
			if(@PostFixString is null)
			begin
				set @code = @PreFixString + @datetime + right(replicate(0,@length) + cast((@CurentVal + @StepValue) as varchar), @length)
				select SeedType = @seqname, SeedValue = @code, SeedMin = @CurentVal + 1, SeedMax = @CurentVal + @StepValue
			end
			else
			begin
				set @code = @PreFixString + @datetime + right(replicate(0,@length) + cast((@CurentVal + @StepValue) as varchar), @length) + @PostFixString
				select SeedType = @seqname, SeedValue = @code, SeedMin = @CurentVal + 1, SeedMax = @CurentVal + @StepValue
			end
			commit tran T2
		end
end

GO
 