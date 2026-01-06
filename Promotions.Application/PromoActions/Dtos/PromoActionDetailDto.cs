using System;
using System.Collections.Generic;
using Promotions.Application.CustomerRelations.Dtos;
using Promotions.Application.DeliveryPoints.Dtos;
using Promotions.Application.Participants.Dtos;
using Promotions.Application.Measures.Dtos;

namespace Promotions.Application.PromoActions.Dtos
{
    public class PromoActionDetailDto
    {
        public int IdAction { get; set; }
        public string Name { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string CodContractor { get; set; } = null!;
        public DateTime DteStartSellIn { get; set; }
        public DateTime DteEndSellIn { get; set; }
        public DateTime DteStartSellOut { get; set; }
        public DateTime DteEndSellOut { get; set; }
        public string? DocumentKey { get; set; }
        public DateTime? DteToShost { get; set; }
        public int? LevParticipants { get; set; }

        public CustomerRelationDetailDto? CustomerRelation { get; set; }
        public List<PromoProductDetailViewDto> Products { get; set; } = new();
    }

    public class CustomerRelationDetailDto : CustomerRelationDto
    {
        public List<ParticipantDto> Participants { get; set; } = new();
        public List<DeliveryPointDto> DeliveryPoints { get; set; } = new();
    }

    public class PromoProductDetailViewDto
    {
        public string CodProduct { get; set; } = null!;
        public int LevProduct { get; set; }
        public string CodDisplay { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public decimal QtyEstimated { get; set; }
        public decimal? PerceDiscount1 { get; set; }
        public decimal? PerceDiscount2 { get; set; }
        public decimal? NumMeasure { get; set; }
        public string? CodMeasure { get; set; }

        public List<PromoMeasureFieldDto> MeasureFields { get; set; } = new();
        public List<ProductDetailHierarchyDto> Details { get; set; } = new();
    }

    public class ProductDetailHierarchyDto
    {
        public string CodNode { get; set; } = null!;
        public bool FlgInclusion { get; set; }
        public List<PromoArticleDto> Articles { get; set; } = new();
    }

    public class PromoArticleDto
    {
        public int IdAction { get; set; }
        public string CodProduct { get; set; } = null!;
        public int LevProduct { get; set; }
        public string CodDisplay { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string CodNode { get; set; } = null!;
        public string? CodNode1 { get; set; }
        public string? CodNode2 { get; set; }
        public string? CodNodeN { get; set; }
    }
}
