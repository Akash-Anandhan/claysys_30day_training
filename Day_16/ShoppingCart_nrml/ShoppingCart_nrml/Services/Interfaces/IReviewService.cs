using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId);
        Task<ReviewDto> AddReviewAsync(int productId, CreateReviewDto dto);
    }
}

