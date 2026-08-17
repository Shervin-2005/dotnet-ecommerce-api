using Application.DTOs.Auth;
using Application.Interfaces;
using AutoMapper;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto?> GetByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user is null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<bool> SoftDeleteAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) return false;

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateUserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) return false;

            if (dto.FirstName is not null) user.FirstName = dto.FirstName;
            if (dto.LastName is not null) user.LastName = dto.LastName;
            if (!string.IsNullOrEmpty(dto.Password)) user.PasswordHash = _passwordHasher.Hash(dto.Password);

            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
