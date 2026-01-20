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

    createAtomicPromotion: async (payload) => {
        const response = await api.post('/api/promotions/actions/atomic', payload);
        return response.data;
    }
};
