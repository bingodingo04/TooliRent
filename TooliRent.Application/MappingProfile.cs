using AutoMapper;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryReadDto>();
            CreateMap<Tool, ToolReadDto>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));
            CreateMap<ToolCreateUpdateDto, Tool>();

            CreateMap<Booking, BookingReadDto>()
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items.Select(i =>
                    new BookingItemReadDto(i.ToolId, i.Tool.Name)).ToList()));
        }
    }
}
