import api from './api';

export const customerService = {
    getCustomers: async () => {
        const response = await api.get('/api/promotions/customer-relations');
        return response.data;
    },

    createCustomer: async (customerData) => {
        const response = await api.post('/api/promotions/customer-relations', customerData);
        return response.data;
    },

    updateCustomer: async (codHier, codDiv, codNode, idLevel, dteStart, updateData) => {
        const params = new URLSearchParams({
            codHier,
            codDiv,
            codNode,
            idLevel,
            dteStart
        }).toString();
        const response = await api.put(`/api/promotions/customer-relations?${params}`, updateData);
        return response.data;
    },

    deleteCustomer: async (codHier, codDiv, codNode, idLevel, dteStart) => {
        const params = new URLSearchParams({
            codHier,
            codDiv,
            codNode,
            idLevel,
            dteStart
        }).toString();
        const response = await api.delete(`/api/promotions/customer-relations?${params}`);
        return response.data;
    }
};
