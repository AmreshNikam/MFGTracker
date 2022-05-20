using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Reorts
    {
    }
    public class ModelList
    {
        public string Name { get; set; }
        public string Part_name { get; set; }
        public string Customer_material_number { get; set; }
        public string Subpart_Name { get; set; }
        public int? Tag_time { get; set; }
    }
    public class InspectionList
    {
        public string DefectbarCode { get; set; }
        public string Message { get; set; }
        public string From_Station { get; set; }
        public string To_Station { get; set; }
        public string Image { get; set; }
    }
    public class UseManagement
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile_number { get; set; }
        public string Active_Inactive { get; set; }
        public string RoleName { get; set; }
        public string json { get; set; }
    }
    public class StoragesReport
    {
        public string Stores_Name { get; set; }
        public string Section { get; set; }
        public string Rack { get; set; }
        public string Shelf { get; set; }
        public string Bins { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
    }
    public class Child
    {
        public string text { get; set; }
        public int value { get; set; }
        public bool @checked { get; set; }
        public List<Child> children { get; set; }
    }

    public class Root
    {
        public string text { get; set; }
        public int value { get; set; }
        public bool @checked { get; set; }
        public List<Child> children { get; set; }
    }
    public class LoadingUnloadingDayWise
    {
        public int? Round { get; set; }
        public int? SkidNumber { get; set; }
        public string Position { get; set; }
        public string Loading_By { get; set; }
        public DateTime? Loading_Date_Time { get; set; }
        public string Loading_Shift { get; set; }
        public string Customer_Name { get; set; }
        public string Subpart_Name { get; set; }
        public string ColorcolName { get; set; }
        public string Barcode { get; set; }
        public DateTime? Unloading_Date_Time { get; set; }
        public string Unloading_By { get; set; }
        public string UnLoading_Shift { get; set; }
        public string Send_Location { get; set; }
        public string Reason { get; set; }
        public string ERPNumber { get; set; }
    }
    public class LoadingColorwiseSummary
    {
        public DateTime? Date { get; set; }
        public string Loading_Shift { get; set; }
        public int? Round { get; set; }
        public int? Quantity { get; set; }
        public int? From_Skid { get; set; }
        public int? To_Skid { get; set; }
        public int? Total_Skid { get; set; }
        public Double? Average_Round { get; set; }
        public string ColorName { get; set; }
    }
    public class LoadingSkidSummary
    {
        public DateTime? LoadingDate { get; set; }
        public string Loading_Shift { get; set; }
        public int? Quantity { get; set; }
        public int? From_Skid { get; set; }
        public int? To_Skid { get; set; }
        public int? Total_Skid { get; set; }
        public Double? Average_Round { get; set; }
        public int? From_Round { get; set; }
        public int? To_Round { get; set; }
    }
    public class DispatchDetailBarcodewise
    {
        public string DispatchPlanID { get; set; }
        public DateTime? PlanEnteryDate { get; set; }
        public DateTime? Plan_Execution_Date { get; set; }
        public string Plan_Added_by { get; set; }
        public string BarCode { get; set; }
        public string Customer_Name { get; set; }
        public string Part_name { get; set; }
        public string Subpart_Name { get; set; }
        public string ColorcolName { get; set; }
        public string AssemblyName { get; set; }
        public string Ship_In { get; set; }
        public string Shipment_Type { get; set; }
        public string Shift { get; set; }
        public DateTime? ScanDateTime { get; set; }
        public string Scan_By { get; set; }
    }
    public class DispatchDetailModelwise
    {
        public string DisatchID { get; set; }
        public DateTime? DateOfPlan { get; set; }
        public string Customer_Name { get; set; }
        public string Part_name { get; set; }
        public string Subpart_Name { get; set; }
        public string ColorcolName { get; set; }
        public string AssemblyName { get; set; }
        public int Quantity { get; set; }
    }
    public class DisatchSummary
    {
        public string DisatchID { get; set; }
        public DateTime? DateOfPlan { get; set; }
        public string Customer_Name { get; set; }
        public int Quantity { get; set; }
    }
    public class BarcodeTrack
    {
        public string Barcode { get; set; }
        public string CurrentLocation { get; set; }
        public string Customer_Name { get; set; }
        public string Part_name { get; set; }
        public string Subpart_Name { get; set; }
        public DateTime? MouldPlanEnteryDate { get; set; }
        public DateTime? MouldPlanningDate { get; set; }
        public string MouldPlanAddedBy { get; set; }
        public DateTime? MouldPlanDateOfExecution { get; set; }
        public string MouldPlanExecutedBy { get; set; }
        public string Shift { get; set; }
        public DateTime? BarcodePrintDateTime { get; set; }
        public string PrintedBy { get; set; }
        public DateTime? BarcodeScanDate { get; set; }
        public string MouldWorkstation_name { get; set; }
        public string MouldDefectCode { get; set; }
        public string MouldDefectMessage { get; set; }
        public string MouldFromStation { get; set; }
        public string MouldToStation { get; set; }
        public string BacodeScanBy { get; set; }
        public string MouldStorelocation { get; set; }
        public DateTime? PaintPlanEnteryDate { get; set; }
        public DateTime? PaintPlanningDate { get; set; }
        public string PaintPlanAddedBy { get; set; }
        public int? Round { get; set; }
        public DateTime? PaintPlanDateOfExecution { get; set; }
        public int? ActualRoundNo { get; set; }
        public string PaintPlanExecutedBy { get; set; }
        public DateTime? LoadingScanDate { get; set; }
        public string PaintWorkstation_name { get; set; }
        public string PaintDefectCode { get; set; }
        public string PaintMessage { get; set; }
        public string PaintFromStation { get; set; }
        public string PaintToStation { get; set; }
        public string LoadingBy { get; set; }
        public string PaintStorelocation { get; set; }
        public int? SkidNumber { get; set; }
        public string Position { get; set; }
        public DateTime? UnloadScanDate { get; set; }
        public string UnloadWorkstation_name { get; set; }
        public string UnloadDefectCode { get; set; }
        public string UnloadDefectMessage { get; set; }
        public string UnloadFromStation { get; set; }
        public string UnloadToStation { get; set; }
        public string UnloadedBy { get; set; }
        public string UnloadStorelocation { get; set; }
        public DateTime? AsseblyScanDate { get; set; }
        public string AsseblyWorkstation_name { get; set; }
        public string AsseblyDefectCode { get; set; }
        public string AsseblyMessage { get; set; }
        public string AsseblyFromStation { get; set; }
        public string AsseblyToStation { get; set; }
        public string AsseblyScanBy { get; set; }
        public string AsseblyStorelocation { get; set; }
        public string DisatchID { get; set; }
        public DateTime? DispatchPlanEnteryDate { get; set; }
        public DateTime? DispatchPlanningDate { get; set; }
        public string DispatchPlanAddedBy { get; set; }
        public DateTime? DispatchScanDateTime { get; set; }
        public string DispatchScanBy { get; set; }

    }
    public class BarcodeTrackerQuery
    {
        public string Barcode { get; set; }
        public string CurrentLocation { get; set; }
        public string Customer_Name { get; set; }
        public string Part_name { get; set; }
        public string Subpart_Name { get; set; }
        public DateTime? MouldPlanEnteryDate { get; set; }
        public DateTime? MouldPlanningDate { get; set; }
        public string MouldPlanAddedBy { get; set; }
        public DateTime? MouldPlanDateOfExecution { get; set; }
        public string MouldPlanExecutedBy { get; set; }
        public string Shift { get; set; }
        public string Workstation_name { get; set; }
        public string DefectCode { get; set; }
        public string Message { get; set; }
        public string FromStation { get; set; }
        public string ToStation { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string Store { get; set; }
        public string Name { get; set; }
        public string ExecutionType { get; set; }
        public DateTime? BarcodePrintDateTime { get; set; }
        public string PrintedBy { get; set; }
        public DateTime? PaintPlanEnteryDate { get; set; }
        public DateTime? PaintPlanningDate { get; set; }
        public string PaintPlanAddedBy { get; set; }
        public int? Round { get; set; }
        public DateTime? PaintPlanDateOfExecution { get; set; }
        public int? ActualRoundNo { get; set; }
        public string PaintPlanExecutedBy { get; set; }
        public int? SkidNumber { get; set; }
        public string Position { get; set; }
        public string DisatchID { get; set; }
        public DateTime? DispatchPlanEnteryDate { get; set; }
        public DateTime? DispatchPlanningDate { get; set; }
        public string DispatchPlanAddedBy { get; set; }
        public DateTime? ScanDateTime { get; set; }
        public string DispatchScanBy { get; set; }
    }
    public class AssemblyDetails
    {
        public DateTime? Date { get; set; }
        public string Shift { get; set; }
        public string Customer_Name { get; set; }
        public string Part_name { get; set; }
        public string Subpart_Name { get; set; }
        public string ColorcolName { get; set; }
        public string Barcodes { get; set; }
        public string Workstation_name { get; set; }
        public int? SubTotal { get; set; }
        public double? Percentage { get; set; }
    }
    public class StockStorewise
    {
        public string Customer_Name { get; set; }
        public string Part_name { get; set; }
        public string Subpart_Name { get; set; }
        public string ColorcolName { get; set; }
        public string AssemblyName { get; set; }
        public string Stores_Name { get; set; }
        public int Instock { get; set; }
        public string Barcodes { get; set; }
    }
    public class StockTakeDetails
    {
        public string Barcode { get; set; }
        public string Subpart_Name { get; set; }
        public string Actual_Color { get; set; }
        public string Actual_Assembly { get; set; }
        public string Actual_Location { get; set; }
        public string Actual_Section { get; set; }
        public string Actual_Rack { get; set; }
        public string Actual_Shelf { get; set; }
        public string Actual_Bins { get; set; }
        public string AfterStockTaking_Color { get; set; }
        public string AfterStockTaking_Assembly { get; set; }
        public string AfterStockTaking_Location { get; set; }
        public string AfterStockTaking_Section { get; set; }
        public string AfterStockTaking_Rack { get; set; }
        public string AfterStockTaking_Shelf { get; set; }
        public string AfterStockTaking_Bins { get; set; }
        public string Remark { get; set; }

    }
    public class StockTakeSummary
    {
        public string Subpart_Name { get; set; }
        public string Actual_Color { get; set; }
        public string Actual_Assembly { get; set; }
        public string Actual_Location { get; set; }
        public string Remark { get; set; }
        public int? Count { get; set; }
    }
    public class Ageing
    {
        public string Stores_Name { get; set; }
        public int Age_limit_in_8_Hrs { get; set; }
        public int Age_limit_in_16_Hrs { get; set; }
        public int Age_limit_in_24_Hrs { get; set; }
        public int More_than_1_day { get; set; }
        public int More_than_2_days { get; set; }
    }
}
