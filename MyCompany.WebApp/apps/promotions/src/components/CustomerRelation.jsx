import React, { useState, useEffect } from 'react';
import { Users, Plus, UserPlus, Check, X, Eye } from 'lucide-react';
import './CustomerRelation.css';
import { customerService } from '../services/customerService';

const CustomerRelation = () => {
    const [showForm, setShowForm] = useState(false);
    const [formData, setFormData] = useState({
        codHier: '',
        codDiv: '',
        codNode: '',
        idLevel: 0,
        dteStart: '',
        codParentNode: '',
        dteEnd: ''
    });

    const [searchCriterion, setSearchCriterion] = useState('Select an option');
    const [searchTerm, setSearchTerm] = useState('');
    const [customers, setCustomers] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [fetchError, setFetchError] = useState(null);

    useEffect(() => {
        fetchCustomers();
    }, []);

    const fetchCustomers = async () => {
        setIsLoading(true);
        setFetchError(null);
        try {
            const data = await customerService.getCustomers();
            setCustomers(data || []);
        } catch (error) {
            console.error("Error fetching customers:", error);
            setFetchError(error.message || "Failed to fetch data");
        } finally {
            setIsLoading(false);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async () => {
        try {
            await customerService.createCustomer(formData);
            alert("Customer Created Successfully!");
            setShowForm(false);
            fetchCustomers(); // Refresh the list
        } catch (error) {
            console.error("Error creating customer:", error);
            alert("Failed to create customer. Check console for details.");
        }
    };

    return (
        <div className="cr-container">


            {!showForm ? (
                <div className="cr-table-container fade-in">
                    <div className="search-by-section">
                        <h3 className="search-by-title">Search By</h3>
                        <div className="search-controls">
                            <div style={{ display: 'flex', gap: '12px', flex: 1 }}>
                                <select
                                    className="criterion-select"
                                    value={searchCriterion}
                                    onChange={(e) => setSearchCriterion(e.target.value)}
                                >
                                    <option>Select an option</option>
                                    <option>Code Hier</option>
                                    <option>Code Div</option>
                                    <option>Code Node</option>
                                    <option>Level</option>
                                </select>
                                <input
                                    type="text"
                                    className="search-term-input"
                                    placeholder="Search Term"
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                />
                                <button className="search-action-btn">
                                    <Eye size={18} />
                                    Search
                                </button>
                            </div>

                            {!showForm && (
                                <button className="create-btn" onClick={() => setShowForm(true)}>
                                    <Plus size={18} />
                                    Create Customer
                                </button>
                            )}
                        </div>

                        {/* Data Table */}
                        <div className="view-table-wrapper">
                            <table className="view-data-table">
                                <thead>
                                    <tr>
                                        <th>Code Hier</th>
                                        <th>Code Div</th>
                                        <th>Code Node</th>
                                        <th>Level</th>
                                        <th>Start Date</th>
                                        <th>Parent Node</th>
                                        <th>End Date</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {isLoading ? (
                                        <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>Loading Customers...</td></tr>
                                    ) : customers.length > 0 ? (
                                        customers.map((customer, idx) => (
                                            <tr key={idx}>
                                                <td>{customer.codHier || customer.CodHier}</td>
                                                <td>{customer.codDiv || customer.CodDiv}</td>
                                                <td>{customer.codNode || customer.CodNode}</td>
                                                <td>{customer.idLevel ?? customer.IdLevel}</td>
                                                <td>{customer.dteStart || customer.DteStart ? new Date(customer.dteStart || customer.DteStart).toLocaleDateString() : '-'}</td>
                                                <td>{customer.codParentNode || customer.CodParentNode}</td>
                                                <td>{customer.dteEnd || customer.DteEnd ? new Date(customer.dteEnd || customer.DteEnd).toLocaleDateString() : '-'}</td>
                                            </tr>
                                        ))
                                    ) : fetchError ? (
                                        <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px', color: '#ef4444' }}>Error: {fetchError} (Check if Backend is running)</td></tr>
                                    ) : (
                                        <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>No customers found in the database.</td></tr>
                                    )}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            ) : (
                <div className="form-card fade-in">
                    <div className="form-header">
                        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                            <div className="icon-wrapper">
                                <UserPlus size={22} />
                            </div>
                            <h2 className="form-title">Create Customer</h2>
                        </div>
                        <button className="close-btn" onClick={() => setShowForm(false)}>
                            <X size={20} />
                        </button>
                    </div>

                    <div className="form-body">
                        <div className="form-section">
                            <h3 className="section-title">Customer Details</h3>
                            <p className="section-subtitle">Enter the hierarchy and node information for the new customer.</p>

                            <div className="form-row">
                                <div className="form-col">
                                    <label className="form-label">Code Hier <span className="required">*</span></label>
                                    <input
                                        type="text"
                                        name="codHier"
                                        className="form-input"
                                        value={formData.codHier}
                                        onChange={handleInputChange}
                                        placeholder="Enter hierarchy code"
                                    />
                                </div>
                                <div className="form-col">
                                    <label className="form-label">Code Div <span className="required">*</span></label>
                                    <input
                                        type="text"
                                        name="codDiv"
                                        className="form-input"
                                        value={formData.codDiv}
                                        onChange={handleInputChange}
                                        placeholder="Enter division code"
                                    />
                                </div>
                            </div>

                            <div className="form-row">
                                <div className="form-col">
                                    <label className="form-label">Code Node <span className="required">*</span></label>
                                    <input
                                        type="text"
                                        name="codNode"
                                        className="form-input"
                                        value={formData.codNode}
                                        onChange={handleInputChange}
                                        placeholder="Enter node code"
                                    />
                                </div>
                                <div className="form-col">
                                    <label className="form-label">ID Level</label>
                                    <input
                                        type="number"
                                        name="idLevel"
                                        className="form-input"
                                        value={formData.idLevel}
                                        onChange={handleInputChange}
                                        placeholder="0"
                                    />
                                </div>
                            </div>

                            <div className="form-row">
                                <div className="form-col">
                                    <label className="form-label">Parent Node</label>
                                    <input
                                        type="text"
                                        name="codParentNode"
                                        className="form-input"
                                        value={formData.codParentNode}
                                        onChange={handleInputChange}
                                        placeholder="Enter parent node code"
                                    />
                                </div>
                            </div>

                            <div className="form-row">
                                <div className="form-col">
                                    <label className="form-label">Start Date <span className="required">*</span></label>
                                    <input
                                        type="datetime-local"
                                        name="dteStart"
                                        className="form-input"
                                        value={formData.dteStart}
                                        onChange={handleInputChange}
                                    />
                                </div>
                                <div className="form-col">
                                    <label className="form-label">End Date <span className="required">*</span></label>
                                    <input
                                        type="datetime-local"
                                        name="dteEnd"
                                        className="form-input"
                                        value={formData.dteEnd}
                                        onChange={handleInputChange}
                                    />
                                </div>
                            </div>

                            <div className="submit-btn-wrapper">
                                <button className="submit-btn bg-green-500 hover:bg-green-600" onClick={handleSubmit}>
                                    Create Customer <Check size={18} />
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default CustomerRelation;
