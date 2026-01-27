import api from './api';

export const promotionService = {
    getPromotionHistory: async () => {
        const response = await api.get('/api/promotions/actions');
        return response.data;
    },

    getPromoActions: async () => {
        const response = await api.get('/api/promotions/actions');
        return response.data;
    },

    getParticipants: async () => {
        const response = await api.get('/api/participants/all');
        return response.data;
    },

    getDeliveryPoints: async () => {
        const response = await api.get('/api/promotions/delivery-points');
        return response.data;
    },

    getProducts: async () => {
        const response = await api.get('/api/promotions/products');
        return response.data;
    },

    getProductDetails: async () => {
        const response = await api.get('/api/promotions/product-details');
        return response.data;
    },

    getPromoArticles: async () => {
        const response = await api.get('/api/promotions/promo-articles');
        return response.data;
    },

    getPromoMeasures: async () => {
        const response = await api.get('/api/promotions/measures');
        return response.data;
    },

    deletePromotion: async (idAction) => {
        const response = await api.delete(`/api/promotions/actions?idAction=${idAction}`);
        return response.data;
    },

    updatePromotion: async (idAction, data) => {
        const response = await api.put(`/api/promotions/actions?idAction=${idAction}`, data);
        return response.data;
    },

    createAtomicPromotion: async (payload) => {
        const response = await api.post('/api/promotions/actions/atomic', payload);
        return response.data;
    },

    addProductsToPromotion: async (products) => {
        // Endpoint: POST /api/promotions/products
        // To "Link" an existing product, we actually CREATE a new record 
        // with the target idAction.

        const promises = products.map(p => {
            const payload = {
                codProduct: p.codProduct || p.CodProduct,
                codDisplay: p.codDisplay || p.CodDisplay,
                codDiv: p.codDiv || p.CodDiv,
                codMeasure: p.codMeasure || p.CodMeasure,
                qtyEstimated: Number(p.qtyEstimated || p.QtyEstimated || 0),
                perceDiscount1: Number(p.perceDiscount1 || p.PerceDiscount1 || 0),
                perceDiscount2: Number(p.perceDiscount2 || p.PerceDiscount2 || 0),
                numMeasure: Number(p.numMeasure || p.NumMeasure || 0),
                idAction: Number(p.idAction),
                // Preserve nested data when linking
                details: p.details || p.Details || [],
                measureFields: p.measureFields || p.MeasureFields || []
            };

            // Reusing the createProduct logic (which handles field mapping)
            return promotionService.createProduct(payload);
        });

        return Promise.all(promises);
    },

    createProduct: async (product) => {
        // ... (existing implementation)
        const payload = {
            codProduct: product.codProduct || product.CodProduct,
            codDisplay: product.codDisplay || product.CodDisplay,
            levProduct: product.levProduct || product.LevProduct || 1,
            codDiv: product.codDiv || product.CodDiv,
            codMeasure: product.codMeasure || product.CodMeasure,
            qtyEstimated: Number(product.qtyEstimated || product.QtyEstimated || 0),
            perceDiscount1: Number(product.perceDiscount1 || product.PerceDiscount1 || 0),
            perceDiscount2: Number(product.perceDiscount2 || product.PerceDiscount2 || 0),
            numMeasure: Number(product.numMeasure || product.NumMeasure || 0),
            idAction: Number(product.idAction || product.IdAction || 0),
            details: (product.details || product.Details || []).map(d => ({
                codNode: d.codNode || d.CodNode,
                codDiv: d.codDiv || d.CodDiv,
                flgInclusion: d.flgInclusion ?? d.FlgInclusion,
                articles: (d.articles || d.Articles || []).map(art => ({
                    codNode1: art.codNode1 || art.CodNode1,
                    codNode2: art.codNode2 || art.CodNode2,
                    codNodeN: art.codNodeN || art.CodNodeN,
                    codNode: art.codNode || art.CodNode || d.codNode || d.CodNode,
                    codDiv: art.codDiv || art.CodDiv || d.codDiv || d.CodDiv
                }))
            })),
            measureFields: (product.measureFields || product.MeasureFields || []).map(m => ({
                fieldName: m.fieldName || m.FieldName,
                formula: m.formula || m.Formula
            }))
        };
        const response = await api.post('/api/promotions/products', payload);
        return response.data;
    },

    updateProduct: async (idAction, codProduct, levProduct, codDisplay, data) => {
        const payload = {
            codDiv: data.codDiv || data.CodDiv,
            qtyEstimated: Number(data.qtyEstimated || data.QtyEstimated || 0),
            perceDiscount1: Number(data.perceDiscount1 || data.PerceDiscount1 || 0),
            perceDiscount2: Number(data.perceDiscount2 || data.PerceDiscount2 || 0),
            numMeasure: Number(data.numMeasure || data.NumMeasure || 0),
            codMeasure: data.codMeasure || data.CodMeasure
        };
        const response = await api.put(`/api/promotions/products?idAction=${idAction}&codProduct=${codProduct}&levProduct=${levProduct}&codDisplay=${codDisplay}`, payload);
        return response.data;
    },

    deleteProduct: async (idAction, codProduct, levProduct, codDisplay) => {
        const response = await api.delete(`/api/promotions/products?idAction=${idAction}&codProduct=${codProduct}&levProduct=${levProduct}&codDisplay=${codDisplay}`);
        return response.data;
    },

    updateParticipant: async (idAction, codParticipant, flgInclusion) => {
        const response = await api.put(`/api/actions/${idAction}/participants/${codParticipant}`, { flgInclusion });
        return response.data;
    },

    deleteParticipant: async (idAction, codParticipant) => {
        const response = await api.delete(`/api/actions/${idAction}/participants/${codParticipant}`);
        return response.data;
    },

    updateDeliveryPoint: async (idAction, codDeliveryPoint, flgInclusion) => {
        const response = await api.put(`/api/promotions/delivery-points/${idAction}/${codDeliveryPoint}`, { flgInclusion });
        return response.data;
    },

    deleteDeliveryPoint: async (idAction, codDeliveryPoint) => {
        const response = await api.delete(`/api/promotions/delivery-points/${idAction}/${codDeliveryPoint}`);
        return response.data;
    }
};
