namespace StaffApp.Application.DTOs.Project
{
    public class ProjectMemberDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }

        public string MemberId { get; set; }
        public string MemberName { get; set; }

        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public string AllocatedDate { get; set; }
        public string DeAllocatedDate { get; set; }
    }
}
