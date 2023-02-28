using System;
using OneBeyond.Studio.Domain.SharedKernel.Entities.Dto;
using OneBeyond.Studio.Obelisk.Application.Features.Users.Dto;
using OneBeyond.Studio.Obelisk.Domain.Features.Dummies.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Dummies.Mappings;

internal sealed class DummyProfile : AutoMapper.Profile
{
    public DummyProfile()
    {
        CreateMap<Dummy, DummyDto>()
            .ForMember(
                dto => dto.Id,
                opt => opt.MapFrom(user => user.Id))
            .ForMember(
                dto => dto.StringValue,
                opt => opt.MapFrom(user => user.StringValue))
            .ForMember(
                dto => dto.NumericValue,
                opt => opt.MapFrom(user => user.NumericValue))
            .ForMember(
                dto => dto.BoolValue,
                opt => opt.MapFrom(user => user.BoolValue))
            .ForMember(
                dto => dto.DateTimeValue,
                opt => opt.MapFrom(user => user.DateTimeValue))
            .ForMember(
                dto => dto.FKValueId,
                opt => opt.MapFrom(user => user.FKValueId));
    }
}
