using System;
using System.Linq;
using OneBeyond.Studio.DataAccess.EFCore.Projections;
using OneBeyond.Studio.Domain.SharedKernel.Entities.Dto;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Infrastructure.Data.Users.Projections;

public class UserProjection
    : IEntityTypeProjection<UserBase, UserNameDto>,
    IEntityTypeProjection<UserBase, ListUsersDto>,
    IEntityTypeProjection<UserBase, GetUserDto>,
    IEntityTypeProjection<UserBase, UserIsActiveDto>,
    IEntityTypeProjection<UserBase, LookupItemDto<Guid>>
{
    IQueryable<UserNameDto> IEntityTypeProjection<UserBase, UserNameDto>.Project(IQueryable<UserBase> entityQuery, ProjectionContext context)
    {
        return entityQuery.Select(user => new UserNameDto
        {
            UserId = user.Id,
            UserName = user.UserName
        });
    }
    IQueryable<ListUsersDto> IEntityTypeProjection<UserBase, ListUsersDto>.Project(IQueryable<UserBase> entityQuery, ProjectionContext context) 
    {
        return entityQuery.Select(user => new ListUsersDto
        {
            Id = user.Id,
            LoginId = user.LoginId,
            Email = user.Email,
            UserName = user.UserName,
            TypeId = user.TypeId,
            RoleId = user.RoleId,
            IsActive = user.IsActive,
            IsLockedOut = user.IsLockedOut
        });
    }
    IQueryable<GetUserDto> IEntityTypeProjection<UserBase, GetUserDto>.Project(IQueryable<UserBase> entityQuery, ProjectionContext context)
    {
        return entityQuery.Select(user => new GetUserDto
        {
            Id = user.Id,
            LoginId = user.LoginId,
            Email = user.Email,
            UserName = user.UserName,
            TypeId = user.TypeId,
            RoleId = user.RoleId,
            IsActive = user.IsActive,
            IsLockedOut = user.IsLockedOut
        });
    }
    IQueryable<UserIsActiveDto> IEntityTypeProjection<UserBase, UserIsActiveDto>.Project(IQueryable<UserBase> entityQuery, ProjectionContext context)
    {
        return entityQuery.Select(user => new UserIsActiveDto
        {
            Id = user.Id,
            IsActive = user.IsActive
        });
    }
    IQueryable<LookupItemDto<Guid>> IEntityTypeProjection<UserBase, LookupItemDto<Guid>>.Project(IQueryable<UserBase> entityQuery, ProjectionContext context) 
    {
        return entityQuery.Select(user => new LookupItemDto<Guid>
        {
            Id = user.Id,
            Name = user.UserName
        });
    }
}
