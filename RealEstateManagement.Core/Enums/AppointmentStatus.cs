namespace RealEstateProject.Core.Enums
{
    public enum AppointmentStatus
    {
        Pending = 0,    // 待確認 (預設)
        Confirmed = 1,  // 已確認 (房仲已接受)
        Completed = 2,  // 已完成 (已帶看)
        Cancelled = 3   // 已取消 (房仲拒絕、客戶取消、或因房源下架而自動取消)
    }
}