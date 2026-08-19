using Application.DTOs;
using Application.DTOs.Auth;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOtpService _otpService;
        private readonly IImageStorageService _imageStorageService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IImageStorageService imageStorageService, IOtpService otpService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
            _otpService = otpService;
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

        public async Task<IEnumerable<UserDto>> GetAllActiveUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllUsersWithoutFilter();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateUserDto dto)
        {
            string profileUrl = "";
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user is null) return false;

                var oldProfileUrl = user.ProfileUrl;
                var imageProvided = dto.Image is not null;

                if (imageProvided)
                {
                    var extension = Path.GetExtension(dto.ImageName);
                    var imageName = $"profile{extension}";

                    profileUrl = await _imageStorageService.UploadAsync(dto.Image!, $"User/{user.ImageFolderId}/images",
                                                                          imageName, dto.ContentType!);
                    user.ProfileUrl = profileUrl;
                }

                if (dto.FirstName is not null) user.FirstName = dto.FirstName;
                if (dto.LastName is not null) user.LastName = dto.LastName;

                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                if (imageProvided && !string.Equals(oldProfileUrl, profileUrl, StringComparison.OrdinalIgnoreCase))
                {
                    await _imageStorageService.DeleteAsync(oldProfileUrl);
                }
                return true;
            }
            catch
            {
                if (!string.IsNullOrEmpty(profileUrl))
                {
                    await _imageStorageService.DeleteAsync(profileUrl);
                }
                throw;
            }
        }
    }
}
