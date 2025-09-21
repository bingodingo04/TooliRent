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
            // Category/Tool
            CreateMap<Category, CategoryReadDto>();
            CreateMap<Tool, ToolReadDto>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
                .ForCtorParam("Name", o => o.MapFrom(s => s.Name))
                .ForCtorParam("CategoryName", o => o.MapFrom(s => s.Category.Name))
                .ForCtorParam("Status", o => o.MapFrom(s => s.Status))
                .ForCtorParam("SerialNumber", o => o.MapFrom(s => s.SerialNumber))
                .ForCtorParam("Condition", o => o.MapFrom(s => s.Condition))
                .ForCtorParam("Description", o => o.MapFrom(s => s.Description));

            CreateMap<ToolCreateUpdateDto, Tool>();

            // BookingItem -> BookingItemReadDto (viktig!)
            CreateMap<BookingItem, BookingItemReadDto>()
                .ForCtorParam("ToolId", o => o.MapFrom(s => s.ToolId))
                .ForCtorParam("ToolName", o => o.MapFrom(s => s.Tool.Name)); // kräver att Tool är laddad

            // Booking -> BookingReadDto
            CreateMap<Booking, BookingReadDto>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
                .ForCtorParam("StartAt", o => o.MapFrom(s => s.StartAt))
                .ForCtorParam("EndAt", o => o.MapFrom(s => s.EndAt))
                .ForCtorParam("Status", o => o.MapFrom(s => s.Status))
                .ForCtorParam("PickedUpAt", o => o.MapFrom(s => s.PickedUpAt))
                .ForCtorParam("ReturnedAt", o => o.MapFrom(s => s.ReturnedAt))
                .ForCtorParam("Items", o => o.MapFrom(s => s.Items)); // nu finns item-mappningen
        }
    }
}

