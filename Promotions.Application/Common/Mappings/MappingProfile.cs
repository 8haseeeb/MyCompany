using AutoMapper;
using Promotions.Application.PromoActions.Dtos;
using Promotions.Application.Products.Dtos;
using Promotions.Application.Participants.Dtos;
using Promotions.Application.DeliveryPoints.Dtos;
using Promotions.Application.CustomerRelations.Dtos;
using Promotions.Application.PromoArticles.Dtos;
using Promotions.Application.Measures.Dtos;
using Promotions.Application.ProductDetails.Dtos;
using Promotions.Domain.PromoActions;
using Promotions.Domain.Products;
using Promotions.Domain.Participants;
using Promotions.Domain.DeliveryPoints;
using Promotions.Domain.CustomerRelations;
using Promotions.Domain.Articles;
using Promotions.Domain.Measures;
using Promotions.Domain.ProductDetails;
using System.Linq;

namespace Promotions.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // PromoAction mappings
            CreateMap<PromoAction, PromoActionDto>().ReverseMap();
            CreateMap<PromoAction, CreatePromoActionDto>().ReverseMap();
            CreateMap<PromoAction, UpdatePromoActionDto>().ReverseMap();
            CreateMap<PromoAction, PromoActionDetailDto>()
                .ForMember(dest => dest.CodContractor, opt => opt.MapFrom(src => src.Participants.FirstOrDefault() != null ? src.Participants.FirstOrDefault()!.CodNode : "N/A"));

            // Product mappings
            CreateMap<PromoProduct, ProductDto>().ReverseMap();
            CreateMap<PromoProduct, CreateProductDto>().ReverseMap();
            CreateMap<PromoProduct, UpdateProductDto>().ReverseMap();
            CreateMap<PromoProduct, PromoProductDetailViewDto>().ReverseMap();

            // Participant mappings
            CreateMap<PromoParticipants, ParticipantDto>().ReverseMap();
            CreateMap<PromoParticipants, CreateParticipantDto>().ReverseMap();
            CreateMap<PromoParticipants, UpdateParticipantDto>().ReverseMap();

            // DeliveryPoint mappings
            CreateMap<PromoDeliveryPoint, DeliveryPointDto>().ReverseMap();
            CreateMap<PromoDeliveryPoint, CreateDeliveryPointDto>().ReverseMap();
            CreateMap<PromoDeliveryPoint, UpdateDeliveryPointDto>().ReverseMap();

            // CustomerRelation mappings
            CreateMap<CustomerRelation, CustomerRelationDto>().ReverseMap();
            CreateMap<CustomerRelation, CreateCustomerRelationDto>().ReverseMap();
            CreateMap<CustomerRelation, UpdateCustomerRelationDto>().ReverseMap();
            CreateMap<CustomerRelation, CustomerRelationDetailDto>().ReverseMap();

            // Article mappings
            CreateMap<PromoArticle, Promotions.Application.PromoActions.Dtos.PromoArticleDto>().ReverseMap();
            CreateMap<PromoArticle, Promotions.Application.PromoArticles.Dtos.PromoArticleDto>().ReverseMap();

            // Measure mappings
            CreateMap<PromoMeasureField, PromoMeasureFieldDto>().ReverseMap();
            CreateMap<PromoMeasureField, UpdatePromoMeasureFieldDto>().ReverseMap();

            // ProductDetail mappings
            CreateMap<PromoProductDetail, ProductDetailDto>().ReverseMap();
            CreateMap<PromoProductDetail, ProductDetailHierarchyDto>().ReverseMap();
        }
    }
}
