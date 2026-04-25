using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCartApp.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Token { get; set; }
        
        [Required]
        public string JwtId { get; set; }
        
        public bool IsUsed { get; set; }
        
        public bool IsRevoked { get; set; }
        
        public DateTime AddedDate { get; set; }
        
        public DateTime ExpiryDate { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
