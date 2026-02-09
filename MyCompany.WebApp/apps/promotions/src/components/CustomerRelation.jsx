import React, { useState, useEffect } from 'react';
import { Users, Plus, UserPlus, Check, X, Eye, ChevronRight, MoreVertical, Edit2, Trash2 } from 'lucide-react';
import './CustomerRelation.css';
import { customerService } from '../services/customerService';

const CustomerRelation = ({ userRole }) => {
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
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    const [customers, setCustomers] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [fetchError, setFetchError] = useState(null);
    const [activeActionMenu, setActiveActionMenu] = useState(null);
    const [showEditModal, setShowEditModal] = useState(false);
    const [editingCustomer, setEditingCustomer] = useState(null);
    const [editFormData, setEditFormData] = useState({
        codParentNode: '',
        dteEnd: ''
    });

    // Pagination State
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(5);

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



    const filteredData = customers.filter(customer => {
        if (searchCriterion === 'Select an option' || !searchTerm) return true;

        const term = searchTerm.toLowerCase();
        switch (searchCriterion) {
            case 'Code Hier': return (customer.codHier || customer.CodHier || '').toString().toLowerCase().includes(term);
            case 'Code Div': return (customer.codDiv || customer.CodDiv || '').toString().toLowerCase().includes(term);
            case 'Code Node': return (customer.codNode || customer.CodNode || '').toString().toLowerCase().includes(term);
            case 'Level': return (customer.idLevel ?? customer.IdLevel ?? '').toString().toLowerCase().includes(term);
            default: return true;
        }
    });

    // Pagination Logic
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentItems = filteredData.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(filteredData.length / itemsPerPage);

    const handlePageChange = (pageNumber) => {
        setCurrentPage(pageNumber);
        setActiveActionMenu(null);
    };

    const handleItemsPerPageChange = (e) => {
        setItemsPerPage(Number(e.target.value));
        setCurrentPage(1);
    };

    // Reset pagination on search
    useEffect(() => {
        setCurrentPage(1);
    }, [searchTerm, searchCriterion]);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async () => {
        alert("Frontend: Submit Clicked! Starting API call...");
        debugger;
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

    const handleEdit = (customer) => {
        setEditingCustomer(customer);
        setEditFormData({
            codParentNode: customer.codParentNode || customer.CodParentNode || '',
            dteEnd: (customer.dteEnd || customer.DteEnd || '').split('T')[0]
        });
        setShowEditModal(true);
        setActiveActionMenu(null);
    };

    const handleUpdate = async () => {
        const codHier = editingCustomer.codHier || editingCustomer.CodHier;
        const codDiv = editingCustomer.codDiv || editingCustomer.CodDiv;
        const codNode = editingCustomer.codNode || editingCustomer.CodNode;
        const idLevel = editingCustomer.idLevel ?? editingCustomer.IdLevel;
        const dteStart = editingCustomer.dteStart || editingCustomer.DteStart;

        try {
            await customerService.updateCustomer(codHier, codDiv, codNode, idLevel, dteStart, editFormData);
            alert("Customer updated successfully!");
            setShowEditModal(false);
            fetchCustomers();
        } catch (error) {
            console.error("Error updating customer:", error);
            alert("Failed to update customer: " + error.message);
        }
    };

    const handleDelete = async (customer) => {
        const codHier = customer.codHier || customer.CodHier;
        const codDiv = customer.codDiv || customer.CodDiv;
        const codNode = customer.codNode || customer.CodNode;
        const idLevel = customer.idLevel ?? customer.IdLevel;
        const dteStart = customer.dteStart || customer.DteStart;

        if (window.confirm(`Are you sure you want to delete customer ${codNode}?`)) {
            try {
                await customerService.deleteCustomer(codHier, codDiv, codNode, idLevel, dteStart);
                alert("Customer deleted successfully!");
                fetchCustomers();
            } catch (error) {
                console.error("Error deleting customer:", error);
                alert("Failed to delete customer: " + error.message);
            }
        }
        setActiveActionMenu(null);
    };

    return (
        <div className="cr-container">


            {!showForm ? (
                <div className="cr-table-container fade-in">
                    <div className="search-by-section">
                        <h3 className="search-by-title">Search By</h3>
                        <div className="search-controls">
                            <div style={{ display: 'flex', gap: '12px', flex: 1 }}>
                                <div className="custom-dropdown-container" style={{ width: '200px' }}>
                                    <div
                                        className={`custom-dropdown-header ${isDropdownOpen ? 'active' : ''}`}
                                        onClick={() => setIsDropdownOpen(!isDropdownOpen)}
                                    >
                                        <span>{searchCriterion}</span>
                                        <ChevronRight
                                            size={18}
                                            style={{
                                                transform: isDropdownOpen ? 'rotate(90deg)' : 'rotate(0deg)',
                                                transition: 'transform 0.2s ease',
                                                color: '#9333ea'
                                            }}
                                        />
                                    </div>

                                    {isDropdownOpen && (
                                        <div className="custom-dropdown-list">
                                            {['Select an option', 'Code Hier', 'Code Div', 'Code Node', 'Level'].map(opt => (
                                                <div
                                                    key={opt}
                                                    className={`custom-dropdown-item ${searchCriterion === opt ? 'selected' : ''}`}
                                                    onClick={() => {
                                                        setSearchCriterion(opt);
                                                        setIsDropdownOpen(false);
                                                    }}
                                                >
                                                    {opt}
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
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

                            {!showForm && userRole === 'Admin' && (
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
                                        <th style={{ width: '80px', textAlign: 'center' }}>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {isLoading ? (
                                        <tr><td colSpan="8" style={{ textAlign: 'center', padding: '40px' }}>Loading Customers...</td></tr>
                                    ) : currentItems.length > 0 ? (
                                        currentItems.map((customer, idx) => (
                                            <tr key={idx}>
                                                <td>{customer.codHier || customer.CodHier}</td>
                                                <td>{customer.codDiv || customer.CodDiv}</td>
                                                <td>{customer.codNode || customer.CodNode}</td>
                                                <td>{customer.idLevel ?? customer.IdLevel}</td>
                                                <td>{customer.dteStart || customer.DteStart ? new Date(customer.dteStart || customer.DteStart).toLocaleDateString() : '-'}</td>
                                                <td>{customer.codParentNode || customer.CodParentNode}</td>
                                                <td>{customer.dteEnd || customer.DteEnd ? new Date(customer.dteEnd || customer.DteEnd).toLocaleDateString() : '-'}</td>
                                                <td style={{ textAlign: 'center', position: 'relative' }}>
                                                    <button
                                                        className="action-menu-btn"
                                                        onClick={(e) => {
                                                            e.stopPropagation();
                                                            setActiveActionMenu(activeActionMenu === idx ? null : idx);
                                                        }}
                                                    >
                                                        <MoreVertical size={18} />
                                                    </button>

                                                    {activeActionMenu === idx && (
                                                        <div className="action-menu fade-in">
                                                            {userRole === 'Admin' ? (
                                                                <>
                                                                    <button className="dropdown-item" onClick={() => handleEdit(customer)}>
                                                                        <Edit2 size={14} style={{ marginRight: '8px' }} /> Edit
                                                                    </button>
                                                                    <button className="dropdown-item delete" onClick={() => handleDelete(customer)}>
                                                                        <Trash2 size={14} style={{ marginRight: '8px' }} /> Delete
                                                                    </button>
                                                                </>
                                                            ) : (
                                                                <div className="dropdown-item disabled" style={{ fontSize: '12px', color: '#94a3b8', cursor: 'not-allowed', padding: '8px 12px' }}>
                                                                    Admin only
                                                                </div>
                                                            )}
                                                        </div>
                                                    )}
                                                </td>
                                            </tr>
                                        ))
                                    ) : fetchError ? (
                                        <tr><td colSpan="8" style={{ textAlign: 'center', padding: '40px', color: '#ef4444' }}>Error: {fetchError} (Check if Backend is running)</td></tr>
                                    ) : (
                                        <tr><td colSpan="8" style={{ textAlign: 'center', padding: '40px' }}>No customers found in the database.</td></tr>
                                    )}
                                </tbody>
                            </table>
                        </div>

                        {/* Pagination UI */}
                        <div className="pagination-container">
                            <div className="pagination-info">
                                Showing {currentItems.length > 0 ? indexOfFirstItem + 1 : 0} to {Math.min(indexOfLastItem, filteredData.length)} of {filteredData.length} results
                            </div>
                            <div className="pagination-controls">
                                <div className="per-page-wrapper">
                                    <select className="per-page-select" value={itemsPerPage} onChange={handleItemsPerPageChange}>
                                        {[5, 10, 20, 50].map(val => (
                                            <option key={val} value={val}>{val} per page</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="pagination-buttons">
                                    <button
                                        className="page-btn nav-btn"
                                        onClick={() => handlePageChange(currentPage - 1)}
                                        disabled={currentPage === 1}
                                    >
                                        Previous
                                    </button>
                                    {[...Array(totalPages)].map((_, i) => (
                                        <button
                                            key={i + 1}
                                            className={`page-btn ${currentPage === i + 1 ? 'active' : ''}`}
                                            onClick={() => handlePageChange(i + 1)}
                                        >
                                            {i + 1}
                                        </button>
                                    ))}
                                    <button
                                        className="page-btn nav-btn"
                                        onClick={() => handlePageChange(currentPage + 1)}
                                        disabled={currentPage === totalPages || totalPages === 0}
                                    >
                                        Next
                                    </button>
                                </div>
                            </div>
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

            {/* Edit Modal */}
            {showEditModal && (
                <div className="modal-overlay">
                    <div className="modal-content fade-in" style={{ maxWidth: '500px' }}>
                        <div className="modal-header">
                            <h3 className="modal-title">Edit Customer Node: {editingCustomer.codNode || editingCustomer.CodNode}</h3>
                            <button className="close-btn" onClick={() => setShowEditModal(false)}>
                                <X size={20} />
                            </button>
                        </div>
                        <div className="modal-body">
                            <div className="form-grid">
                                <div className="form-group" style={{ marginBottom: '16px' }}>
                                    <label className="form-label">Parent Node</label>
                                    <input
                                        className="form-input"
                                        value={editFormData.codParentNode}
                                        onChange={(e) => setEditFormData({ ...editFormData, codParentNode: e.target.value })}
                                        placeholder="Enter parent node code"
                                    />
                                </div>
                                <div className="form-group">
                                    <label className="form-label">End Date</label>
                                    <input
                                        type="date"
                                        className="form-input"
                                        value={editFormData.dteEnd}
                                        onChange={(e) => setEditFormData({ ...editFormData, dteEnd: e.target.value })}
                                    />
                                </div>
                            </div>
                        </div>
                        <div className="modal-footer" style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '24px', paddingTop: '16px', borderTop: '1px solid #e5e7eb' }}>
                            <button className="btn-secondary" onClick={() => setShowEditModal(false)} style={{ padding: '8px 16px', borderRadius: '6px', border: '1px solid #d1d5db', background: '#fff', cursor: 'pointer' }}>Cancel</button>
                            <button className="btn-primary" onClick={handleUpdate} style={{ padding: '8px 16px', borderRadius: '6px', border: 'none', background: '#9333ea', color: '#fff', cursor: 'pointer' }}>Save Changes</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default CustomerRelation;
