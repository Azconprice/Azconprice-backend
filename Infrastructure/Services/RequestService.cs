using Application.Models.DTOs;
using Application.Models.DTOs.Pagination;
using Application.Repositories;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class RequestService(IRequestRepository repository, IMapper mapper) : IRequestService
    {
        private readonly IRequestRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<RequestShowDTO?> CreateRequestAsync(RequestDTO requestDto)
        {
            // Map DTO to entity
            var entity = _mapper.Map<Request>(requestDto);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = DateTime.UtcNow;

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<RequestShowDTO>(entity);
        }

        public async Task<bool> DeleteRequestAsync(string requestId)
        {
            var entity = await _repository.GetAsync(requestId);
            if (entity == null)
                return false;

            _repository.Remove(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<RequestShowDTO>> GetAllRequestsAsync(PaginationRequest request)
        {
            var query = _repository.Query().OrderByDescending(r => r.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResult<RequestShowDTO>
            {
                Items = _mapper.Map<IEnumerable<RequestShowDTO>>(items),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<RequestShowDTO?> GetRequestByIdAsync(string requestId)
        {
            var entity = await _repository.GetAsync(requestId);
            return entity == null ? null : _mapper.Map<RequestShowDTO>(entity);
        }

        public async Task<PaginatedResult<RequestShowDTO>> GetRequestByTypeAsync(PaginationRequest request, string type)
        {
            // Parse and validate type
            var requestType = RequestTypeExtensions.Parse(type);

            var query = _repository.Query()
                .Where(r => r.Type == requestType)
                .OrderByDescending(r => r.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResult<RequestShowDTO>
            {
                Items = _mapper.Map<IEnumerable<RequestShowDTO>>(items),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}