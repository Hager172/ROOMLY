namespace ROOMLY.DTOs.user
{
    public class UsersDTO
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public string Role { get; set; } // لو بتعرضي الدور (Admin/User)

        public DateTime CreatedAt { get; set; } // تاريخ إنشاء الحساب

        public bool IsEmailConfirmed { get; set; } // هل الإيميل متأكد؟

        // لو في بيانات إضافية خاصة بمشروعك ممكن تضيفيها هنا
    }
}
