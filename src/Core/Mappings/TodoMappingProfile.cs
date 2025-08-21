using AutoMapper;
using EtabsExtensions.Core.DTOs;
using EtabsExtensions.Core.Models;

namespace EtabsExtensions.Core.Mappings;

public class TodoMappingProfile: Profile
{
    // TodoItem to TodoItemResponse mapping
    public TodoMappingProfile()
    {
        // TodoItem to TodoItemResponse
        CreateMap<TodoItem, TodoItemResponse>()
            .ForMember(dest => dest.HasDescription, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.Description)))
            .ForMember(dest => dest.DisplayCreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("MMM dd, yyyy HH:mm")))
            .ForMember(dest => dest.DisplayCompletedAt, opt => opt.MapFrom(src => src.CompletedAt.HasValue ? src.CompletedAt.Value.ToString("MMM dd, yyyy HH:mm") : null));

        // CreateTodoItemRequest to TodoItem
        CreateMap<CreateTodoItemRequest, TodoItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty));

        // UpdateTodoItemRequest to TodoItem
        CreateMap<UpdateTodoItemRequest, TodoItem>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.IsCompleted ? DateTime.UtcNow : (DateTime?)null))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty));
    }
}
