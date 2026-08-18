using Application.DTOs.Auth;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IImageStorageService _imageStorageService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher, IImageStorageService imageStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _imageStorageService = imageStorageService;
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
