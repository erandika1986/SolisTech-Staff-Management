namespace StaffApp.Application.DTOs.User
{
    public class UserBankAccountDTO
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public bool IsPrimaryAccount { get; set; }
    }
}
