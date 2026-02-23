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
            // Global configuration for Rich Domain Model (Private Setters)
            // Note: In modern AutoMapper, this is typically handled by setting ShouldMapProperty in the Profile or MapperConfiguration
            // But we can also be explicit here.

            // PromoAction mappings
            CreateMap<PromoActionDto, PromoAction>()
                .ConstructUsing(src => new PromoAction(src.IdAction, src.Name, src.CodDiv))
                .AfterMap((src, dest) => {
                    dest.UpdateSellInDates(src.DteStartSellIn, src.DteEndSellIn);
                    dest.UpdateSellOutDates(src.DteStartSellOut, src.DteEndSellOut);
                    dest.SetHostDate(src.DteToShost);
                    dest.UpdateBasicInfo(src.Name, src.CodDiv, src.DocumentKey, src.LevParticipants);
                });

            CreateMap<CreatePromoActionDto, PromoAction>()
                .ConstructUsing(src => new PromoAction(src.IdAction, src.Name, src.CodDiv))
                .AfterMap((src, dest) => {
                    dest.UpdateSellInDates(src.DteStartSellIn, src.DteEndSellIn);
                    dest.UpdateSellOutDates(src.DteStartSellOut, src.DteEndSellOut);
                    dest.SetHostDate(src.DteToShost);
                    dest.UpdateBasicInfo(src.Name, src.CodDiv, src.DocumentKey, src.LevParticipants);
                });

            CreateMap<AtomicCreatePromoActionDto, PromoAction>()
                .ConstructUsing(src => new PromoAction(src.IdAction, src.Name, src.CodDiv))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
                .ForMember(dest => dest.DeliveryPoints, opt => opt.MapFrom(src => src.DeliveryPoints))
                .AfterMap((src, dest) => {
                    dest.UpdateSellInDates(src.DteStartSellIn, src.DteEndSellIn);
                    dest.UpdateSellOutDates(src.DteStartSellOut, src.DteEndSellOut);
                    dest.SetHostDate(src.DteToShost);
                    dest.UpdateBasicInfo(src.Name, src.CodDiv, src.DocumentKey, src.LevParticipants);
                });

            CreateMap<PromoAction, PromoActionDto>();
            CreateMap<PromoAction, CreatePromoActionDto>();
            CreateMap<PromoAction, UpdatePromoActionDto>();
            CreateMap<PromoAction, PromoActionDetailDto>()
                .ForMember(dest => dest.CodContractor, opt => opt.MapFrom(src => src.Participants.FirstOrDefault() != null ? src.Participants.FirstOrDefault()!.CodNode : "N/A"));

            // Product mappings
            CreateMap<ProductDto, PromoProduct>()
                .ConstructUsing((src, context) => new PromoProduct(
                    src.IdAction > 0 ? src.IdAction : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodProduct, src.LevProduct, src.CodDisplay, src.CodDiv))
                .AfterMap((src, dest) => {
                    dest.UpdateQuantities(src.QtyEstimated, src.NumMeasure, src.CodMeasure);
                    dest.UpdateDiscounts(src.PerceDiscount1, src.PerceDiscount2);
                })
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));
            
            CreateMap<CreateProductDto, PromoProduct>()
                .ConstructUsing((src, context) => new PromoProduct(
                    (src.IdAction ?? 0) > 0 ? (src.IdAction ?? 0) : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodProduct ?? "", src.LevProduct ?? 0, src.CodDisplay ?? "", src.CodDiv))
                .AfterMap((src, dest) => {
                    dest.UpdateQuantities(src.QtyEstimated, src.NumMeasure, src.CodMeasure);
                    dest.UpdateDiscounts(src.PerceDiscount1, src.PerceDiscount2);
                })
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));

            CreateMap<AtomicCreateProductDto, PromoProduct>()
                .ConstructUsing((src, context) => new PromoProduct(
                    (src.IdAction ?? 0) > 0 ? (src.IdAction ?? 0) : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodProduct ?? "", src.LevProduct ?? 0, src.CodDisplay ?? "", src.CodDiv))
                .AfterMap((src, dest) => {
                    dest.UpdateQuantities(src.QtyEstimated, src.NumMeasure, src.CodMeasure);
                    dest.UpdateDiscounts(src.PerceDiscount1, src.PerceDiscount2);
                })
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));

            CreateMap<PromoProduct, ProductDto>();
            CreateMap<PromoProduct, CreateProductDto>();
            CreateMap<PromoProduct, UpdateProductDto>();
            CreateMap<PromoProduct, PromoProductDetailViewDto>();

            // Participant mappings
            CreateMap<ParticipantDto, PromoParticipants>()
                .ConstructUsing((src, context) => new PromoParticipants(
                    src.IdAction > 0 ? src.IdAction : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodParticipant, src.FlgInclusion, src.CodHier ?? "", src.CodDiv ?? "", src.CodNode ?? "", src.IdLevel, src.DteStart));
            
            CreateMap<CreateParticipantDto, PromoParticipants>()
                .ConstructUsing((src, context) => new PromoParticipants(
                    context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0, 
                    src.CodParticipant, src.FlgInclusion, src.CodHier ?? "", src.CodDiv ?? "", src.CodNode ?? "", src.IdLevel ?? 0, src.DteStart.GetValueOrDefault()));

            CreateMap<PromoParticipants, ParticipantDto>();
            CreateMap<PromoParticipants, CreateParticipantDto>();
            CreateMap<PromoParticipants, UpdateParticipantDto>();

            // DeliveryPoint mappings
            CreateMap<DeliveryPointDto, PromoDeliveryPoint>()
                .ConstructUsing((src, context) => new PromoDeliveryPoint(
                    src.IdAction > 0 ? src.IdAction : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodDeliveryPoint, src.FlgInclusion, src.CodHier ?? "", src.CodDiv ?? "", src.CodNode ?? "", src.IdLevel, src.DteStart));
            
            CreateMap<CreateDeliveryPointDto, PromoDeliveryPoint>()
                .ConstructUsing((src, context) => new PromoDeliveryPoint(
                    context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0, 
                    src.CodDeliveryPoint, src.FlgInclusion, src.CodHier ?? "", src.CodDiv ?? "", src.CodNode ?? "", src.IdLevel ?? 0, src.DteStart.GetValueOrDefault()));


            CreateMap<PromoDeliveryPoint, DeliveryPointDto>();
            CreateMap<PromoDeliveryPoint, CreateDeliveryPointDto>();
            CreateMap<PromoDeliveryPoint, UpdateDeliveryPointDto>();

            // CustomerRelation mappings
            CreateMap<CustomerRelationDto, CustomerRelation>()
                .ConstructUsing(src => new CustomerRelation(src.CodHier, src.CodDiv, src.CodNode, src.IdLevel, src.DteStart, src.CodParentNode));

            CreateMap<CreateCustomerRelationDto, CustomerRelation>()
                .ConstructUsing(src => new CustomerRelation(src.CodHier, src.CodDiv, src.CodNode, src.IdLevel, src.DteStart, src.CodParentNode));

            CreateMap<CustomerRelation, CustomerRelationDto>();
            CreateMap<CustomerRelation, CreateCustomerRelationDto>();
            CreateMap<CustomerRelation, UpdateCustomerRelationDto>();
            CreateMap<CustomerRelation, CustomerRelationDetailDto>();

            // Article mappings (using fully qualified names due to namespace collision)
            CreateMap<Promotions.Application.PromoArticles.Dtos.PromoArticleDto, PromoArticle>()
                .ConstructUsing((src, context) => new PromoArticle(
                    src.IdAction > 0 ? src.IdAction : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodProduct, src.LevProduct, src.CodDisplay, src.CodDiv, src.CodNode, src.CodNode1, src.CodNode2, src.CodNodeN));
            
            CreateMap<Promotions.Application.PromoActions.Dtos.PromoArticleDto, PromoArticle>()
                .ConstructUsing((src, context) => new PromoArticle(
                    src.IdAction > 0 ? src.IdAction : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodProduct, src.LevProduct, src.CodDisplay, src.CodDiv, src.CodNode, src.CodNode1, src.CodNode2, src.CodNodeN));
            
            CreateMap<CreatePromoArticleDto, PromoArticle>()
                .ConstructUsing((src, context) => new PromoArticle(
                    (src.IdAction ?? 0) > 0 ? (src.IdAction ?? 0) : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodProduct ?? "", src.LevProduct ?? 0, src.CodDisplay ?? "", src.CodDiv ?? "", src.CodNode ?? "", src.CodNode1, src.CodNode2, src.CodNodeN));

            CreateMap<AtomicCreatePromoArticleDto, PromoArticle>()
                .ConstructUsing((src, context) => new PromoArticle(
                    (src.IdAction ?? 0) > 0 ? (src.IdAction ?? 0) : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodProduct ?? "", src.LevProduct ?? 0, src.CodDisplay ?? "", src.CodDiv ?? "", src.CodNode ?? "", src.CodNode1, src.CodNode2, src.CodNodeN));

            CreateMap<PromoArticle, Promotions.Application.PromoArticles.Dtos.PromoArticleDto>();
            CreateMap<PromoArticle, Promotions.Application.PromoActions.Dtos.PromoArticleDto>();
            CreateMap<PromoArticle, CreatePromoArticleDto>();

            // Measure mappings
            CreateMap<PromoMeasureFieldDto, PromoMeasureField>()
                .ConstructUsing(src => new PromoMeasureField(src.CodDiv, src.CodMeasure, src.FieldName, src.Formula));

            CreateMap<PromoMeasureField, PromoMeasureFieldDto>();
            CreateMap<PromoMeasureField, UpdatePromoMeasureFieldDto>();

            // ProductDetail mappings
            CreateMap<ProductDetailDto, PromoProductDetail>()
                .ConstructUsing((src, context) => new PromoProductDetail(
                    src.IdAction > 0 ? src.IdAction : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodProduct, src.LevProduct, src.CodDisplay, src.CodNode, src.CodDiv, src.FlgInclusion));
            
            CreateMap<AtomicCreateProductDetailDto, PromoProductDetail>()
                .ConstructUsing((src, context) => new PromoProductDetail(
                    (src.IdAction ?? 0) > 0 ? (src.IdAction ?? 0) : (context.Items.ContainsKey("IdAction") ? (int)context.Items["IdAction"] : 0), 
                    src.CodProduct ?? "", src.LevProduct ?? 0, src.CodDisplay ?? "", src.CodNode ?? "", src.CodDiv ?? "", src.FlgInclusion));

            CreateMap<PromoProductDetail, ProductDetailDto>();
            CreateMap<PromoProductDetail, ProductDetailHierarchyDto>();
        }
    }
}
