using System;
using OneBeyond.Studio.Domain.SharedKernel.Entities.Dto;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.Mappings;

internal sealed class UserProfile : AutoMapper.Profile
{
    public UserProfile()
    {
        CreateMap<UserBase, ListUsersDto>()
            .ForMember(
                dto => dto.Id,
                opt => opt.MapFrom(user => user.Id))
            .ForMember(
                dto => dto.LoginId,
                opt => opt.MapFrom(user => user.LoginId))
            .ForMember(
                dto => dto.Email,
                opt => opt.MapFrom(user => user.Email))
            .ForMember(
                dto => dto.UserName,
                opt => opt.MapFrom(user => user.UserName))
            .ForMember(
                dto => dto.TypeId,
                opt => opt.MapFrom(user => user.TypeId))
            .ForMember(
                dto => dto.RoleId,
                opt => opt.MapFrom(user => user.RoleId))
            .ForMember(
                dto => dto.IsActive,
                opt => opt.MapFrom(user => user.IsActive))
            .ForMember(
                dto => dto.IsLockedOut,
                opt => opt.MapFrom(user => user.IsLockedOut));

        CreateMap<UserBase, GetUserDto>()
            .ForMember(
                dto => dto.Id,
                opt => opt.MapFrom(user => user.Id))
            .ForMember(
                dto => dto.LoginId,
                opt => opt.MapFrom(user => user.LoginId))
            .ForMember(
                dto => dto.Email,
                opt => opt.MapFrom(user => user.Email))
            .ForMember(
                dto => dto.UserName,
                opt => opt.MapFrom(user => user.UserName))
            .ForMember(
                dto => dto.TypeId,
                opt => opt.MapFrom(user => user.TypeId))
            .ForMember(
                dto => dto.RoleId,
                opt => opt.MapFrom(user => user.RoleId))
            .ForMember(
                dto => dto.IsActive,
                opt => opt.MapFrom(user => user.IsActive))
            .ForMember(
                dto => dto.IsLockedOut,
                opt => opt.MapFrom(user => user.IsLockedOut));

        CreateMap<UserBase, UserIsActiveDto>()
            .ForMember(
                dto => dto.Id,
                opt => opt.MapFrom(user => user.Id))
            .ForMember(
                dto => dto.IsActive,
                opt => opt.MapFrom(user => user.IsActive));

        CreateMap<UserBase, LookupItemDto<Guid>>()
            .ForMember(
                dto => dto.Id,
                options => options.MapFrom(user => user.Id))
            .ForMember(
                dto => dto.Name,
                options => options.MapFrom(user => user.UserName));

        CreateMap<UserBase, UserNameDto>()
            .ForMember(
                dto => dto.UserId,
                options => options.MapFrom(user => user.Id))
            .ForMember(
                dto => dto.UserName,
                options => options.MapFrom(user => user.UserName));
    }
}
