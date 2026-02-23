using System;

namespace Promotions.Domain.Products
{
    public class PromoProduct
    {
        public int IdAction { get; private set; }
        public string CodProduct { get; private set; } = null!;
        public int LevProduct { get; private set; }
        public string CodDisplay { get; private set; } = null!;
        public string? CodDiv { get; private set; }
        public decimal QtyEstimated { get; private set; }
        public decimal? PerceDiscount1 { get; private set; }
        public decimal? PerceDiscount2 { get; private set; }
        public decimal? NumMeasure { get; private set; }
        public string? CodMeasure { get; private set; }

        public virtual PromoActions.PromoAction Action { get; private set; } = null!;
        public virtual System.Collections.Generic.ICollection<ProductDetails.PromoProductDetail> Details { get; private set; } = new System.Collections.Generic.List<ProductDetails.PromoProductDetail>();

        private PromoProduct() { }

        public PromoProduct(int idAction, string codProduct, int levProduct, string codDisplay, string? codDiv)
        {
            if (idAction <= 0) throw new ArgumentException("IdAction must be positive.");
            if (string.IsNullOrWhiteSpace(codProduct)) throw new ArgumentException("CodProduct is required.");
            if (string.IsNullOrWhiteSpace(codDiv)) throw new ArgumentException("CodDiv is required.");

            IdAction = idAction;
            CodProduct = codProduct;
            LevProduct = levProduct;
            CodDisplay = codDisplay;
            CodDiv = codDiv;
        }

        public void UpdateQuantities(decimal qtyEstimated, decimal? numMeasure, string? codMeasure)
        {
            if (qtyEstimated < 0) throw new ArgumentException("Estimated quantity cannot be negative.");
            QtyEstimated = qtyEstimated;
            NumMeasure = numMeasure;
            CodMeasure = codMeasure;
        }

        public void UpdateDiscounts(decimal? discount1, decimal? discount2)
        {
            if (discount1 < 0 || discount1 > 100) throw new ArgumentException("Discount 1 must be between 0 and 100.");
            if (discount2 < 0 || discount2 > 100) throw new ArgumentException("Discount 2 must be between 0 and 100.");
            
            PerceDiscount1 = discount1;
            PerceDiscount2 = discount2;
        }

        public void UpdateDivision(string? codDiv)
        {
            if (string.IsNullOrWhiteSpace(codDiv)) throw new ArgumentException("CodDiv is required.");
            CodDiv = codDiv;
        }

        public void AddDetail(ProductDetails.PromoProductDetail detail)
        {
            if (detail == null) throw new ArgumentNullException(nameof(detail));
            Details.Add(detail);
        }
    }
}
