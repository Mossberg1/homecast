using AutoMapper;
using StreamingApplication.Data.DTOs.Media;
using StreamingApplication.Data.DTOs.Movie;
using StreamingApplication.Data.Entities;
using StreamingApplication.Forms;

namespace StreamingApplication.Profiles;

public class MappingProfile : Profile {
    public MappingProfile() {
        CreateMap<RegisterForm, ApplicationUser>();
        
        _MediaMappings();
        _MovieMappings();
    }


    private void _MediaMappings() {
        CreateMap<Media, MediaDTO>().ReverseMap();
        CreateMap<Media, MediaCreateDTO>().ReverseMap();
        CreateMap<Media, MediaUpdateDTO>().ReverseMap();
        CreateMap<Media, MediaUploadDTO>().ReverseMap();
        CreateMap<Media, MediaListDTO>().ReverseMap();
    }


    private void _MovieMappings() {
        CreateMap<Movie, MovieDTO>().ReverseMap();
        CreateMap<Movie, MovieCreateDTO>().ReverseMap();
        CreateMap<Movie, MovieUpdateDTO>().ReverseMap();
        CreateMap<Movie, MovieUploadDTO>().ReverseMap();
        CreateMap<Movie, MovieListDTO>().ReverseMap();
    }
}