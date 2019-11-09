using System.ComponentModel.DataAnnotations;

namespace ShelfSimple.Api.Dtos
{
    public class RegisterSellerRequest
    {
        [Required, MaxLength(100)]
        public string DisplayName { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; }

        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; }
    }

    public class UpdateProfileRequest
    {
        [Required, MaxLength(100)]
        public string DisplayName { get; set; }

        [MaxLength(1000)]
        public string Bio { get; set; }
    }

    public class SellerDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Slug { get; set; }
        public string Bio { get; set; }
    }
}
