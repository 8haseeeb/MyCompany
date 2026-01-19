import api from './api';

export const customerService = {
    getCustomers: async () => {
        const response = await api.get('/api/promotions/customer-relations');
        return response.data;
    },

    createCustomer: async (customerData) => {
        const response = await api.post('/api/promotions/customer-relations', customerData);
        return response.data;
    }
};
