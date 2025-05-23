using AutoMapper;
using ROOMLY.DTOs.AccountDTO;
using ROOMLY.DTOs.RoomDTO;
using ROOMLY.DTOs.user;
using ROOMLY.DTOs.favourite;
using ROOMLY.models;
using WebApplication1.models;

namespace ROOMLY.Mapconfig
{
    public class MapConfig:Profile
    {
        //mapping room
        public MapConfig() {
            CreateMap<Room, ROOMdto>().AfterMap((src, des) =>
            {


                des.RoomTypeName = src.RoomType.Name;
                des.Status = (int)src.Status;


            }).ReverseMap();

            CreateMap<ReservationCreateDto, Reservation>().ReverseMap();
            CreateMap<ReservationCreateUserDto, Reservation>();


            CreateMap<Room,RoomCreateDto>().ReverseMap();

            CreateMap<ApplicationUser, UsersDTO>().ReverseMap();

            CreateMap<Favourite, AddFavoriteDto>().ReverseMap();

            CreateMap<Room, RoomDetailsDto>()
    .ForMember(dest => dest.GalleryImages, opt => opt.MapFrom(src => src.RoomImages.Select(i => i.ImageUrl).ToList()));

            CreateMap<Favourite, FavoriteWithRoomDto>()
    .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
    .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.Room.RoomType.Name))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Room.Status));


            //mapping reservation

            CreateMap<Reservation,ReservationDto>().AfterMap((src,dest)=>
            {
            dest.RoomNumber=src.Room.RoomNumber;
                dest.UserName = src.User.UserName;
            
            }
            ).ReverseMap();

            CreateMap<Reservation,ReservationCreateDto>().ReverseMap();


            //mapping roomtype
            CreateMap<RoomType,RoomTypeDto>().ReverseMap();
            CreateMap<RoomType,RoomTypeCreateDto>().ReverseMap();

           

        }
    }
}
