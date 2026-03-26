import api from './api';

export const promotionService = {
    getPromotionHistory: async () => {
        const response = await api.get('/api/v1/promotions/actions');
        return response.data;
    },

    getPromoActions: async () => {
        const response = await api.get('/api/v1/promotions/actions');
        return response.data;
    },

    getParticipants: async () => {
        const response = await api.get('/api/v1/participants/all');
        return response.data;
    },

    getDeliveryPoints: async () => {
        const response = await api.get('/api/v1/promotions/delivery-points');
        return response.data;
    },

    getProducts: async () => {
        const response = await api.get('/api/v1/promotions/products');
        return response.data;
    },

    getProductDetails: async () => {
        const response = await api.get('/api/v1/promotions/product-details');
        return response.data;
    },

    getPromoArticles: async () => {
        const response = await api.get('/api/v1/promotions/promo-articles');
        return response.data;
    },

    getPromoMeasures: async () => {
        const response = await api.get('/api/v1/promotions/measures');
        return response.data;
    },

    deletePromotion: async (idAction) => {
        const response = await api.delete(`/api/v1/promotions/actions?idAction=${idAction}`);
        return response.data;
    },

    createAtomicPromotion: async (payload) => {
        const response = await api.post('/api/v1/promotions/actions/atomic', payload);
        return response.data;
    },

    getDashboardStats: async () => {
        const response = await api.get('/api/v1/promotions/dashboard/metrics');
        return response.data;
    }
};
