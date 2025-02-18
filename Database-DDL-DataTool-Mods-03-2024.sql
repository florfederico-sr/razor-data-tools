--Database changes for deployment of DataTool modifications/additions

--staging.Tunecore_Earnings

ALTER TABLE [staging].[Tunecore_Earnings] ADD  [FILEHASH]NVARCHAR(200)
ALTER TABLE [staging].[Tunecore_Earnings] ADD  [Artist_Id]bigint


--staging.BMI_Earnings


--staging.ASCAP_Earnings