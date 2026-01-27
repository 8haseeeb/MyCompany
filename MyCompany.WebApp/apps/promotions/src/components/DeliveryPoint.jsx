import React, { useState, useEffect } from 'react';
import { MapPin, Eye, Search, ChevronRight, MoreVertical, Edit2, Trash2, Check, X } from 'lucide-react';
import './CustomerRelation.css'; // Reusing established table styles
import { promotionService } from '../services/promotionService';

const DeliveryPoint = () => {
    const [deliveryPoints, setDeliveryPoints] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
    const [searchCriterion, setSearchCriterion] = useState('Select an option');
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    const [activeKebab, setActiveKebab] = useState(null);
    const [showEditModal, setShowEditModal] = useState(false);
    const [editingDeliveryPoint, setEditingDeliveryPoint] = useState(null);
    const [editFlagInclusion, setEditFlagInclusion] = useState(true);

    useEffect(() => {
        fetchDeliveryPoints();
    }, []);

    const fetchDeliveryPoints = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const data = await promotionService.getDeliveryPoints();
            setDeliveryPoints(data || []);
        } catch (err) {
            console.error("Error fetching delivery points:", err);
            setError(err.message || "Failed to fetch delivery points");
        } finally {
            setIsLoading(false);
        }
    };

    const handleEdit = (dp) => {
        setEditingDeliveryPoint(dp);
        setEditFlagInclusion(dp.flgInclusion ?? dp.FlgInclusion ?? true);
        setShowEditModal(true);
        setActiveKebab(null);
    };

    const handleUpdate = async () => {
        const idAction = editingDeliveryPoint.idAction || editingDeliveryPoint.IdAction;
        const codDeliveryPoint = editingDeliveryPoint.codDeliveryPoint || editingDeliveryPoint.CodDeliveryPoint;

        try {
            await promotionService.updateDeliveryPoint(idAction, codDeliveryPoint, editFlagInclusion);
            alert("Delivery Point updated successfully!");
            setShowEditModal(false);
            fetchDeliveryPoints();
        } catch (err) {
            console.error("Error updating delivery point:", err);
            alert("Failed to update delivery point: " + err.message);
        }
    };

    const handleDelete = async (dp) => {
        const idAction = dp.idAction || dp.IdAction;
        const codDeliveryPoint = dp.codDeliveryPoint || dp.CodDeliveryPoint;

        if (window.confirm(`Are you sure you want to delete delivery point ${codDeliveryPoint}?`)) {
            try {
                await promotionService.deleteDeliveryPoint(idAction, codDeliveryPoint);
                alert("Delivery Point deleted successfully!");
                fetchDeliveryPoints();
            } catch (err) {
                console.error("Error deleting delivery point:", err);
                alert("Failed to delete delivery point: " + err.message);
            }
        }
        setActiveKebab(null);
    };

    const filteredData = deliveryPoints.filter(dp => {
        if (searchCriterion === 'Select an option' || !searchTerm) return true;

        const term = searchTerm.toLowerCase();
        switch (searchCriterion) {
            case 'ID Action': return (dp.idAction || dp.IdAction || '').toString().toLowerCase().includes(term);
            case 'Cod Delivery Point': return (dp.codDeliveryPoint || dp.CodDeliveryPoint || '').toString().toLowerCase().includes(term);
            case 'Code Hier': return (dp.codHier || dp.CodHier || '').toString().toLowerCase().includes(term);
            case 'Code Div': return (dp.codDiv || dp.CodDiv || '').toString().toLowerCase().includes(term);
            case 'Code Node': return (dp.codNode || dp.CodNode || '').toString().toLowerCase().includes(term);
            case 'ID Level': return (dp.idLevel ?? dp.IdLevel ?? '').toString().toLowerCase().includes(term);
            default: return true;
        }
    });

    return (
        <div className="cr-container">
            <div className="cr-table-container fade-in">
                <div className="search-by-section">
                    <h3 className="search-by-title">Delivery Points List</h3>
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
                                        {['Select an option', 'ID Action', 'Cod Delivery Point', 'Code Hier', 'Code Div', 'Code Node', 'ID Level'].map(opt => (
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
                            <div className="search-input-wrapper" style={{ position: 'relative', flex: 1 }}>
                                <input
                                    type="text"
                                    className="search-term-input"
                                    placeholder="Search Term..."
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                    style={{ paddingLeft: '40px' }}
                                />
                                <Search size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: '#94a3b8' }} />
                            </div>
                            <button className="search-action-btn" onClick={fetchDeliveryPoints}>
                                <Eye size={18} /> Refresh
                            </button>
                        </div>
                    </div>

                    <div className="view-table-wrapper">
                        <table className="view-data-table">
                            <thead>
                                <tr>
                                    <th>ID Action</th>
                                    <th>Cod Delivery Point</th>
                                    <th>Code Hier</th>
                                    <th>Code Div</th>
                                    <th>Code Node</th>
                                    <th>ID Level</th>
                                    <th>Flag Inclusion</th>
                                    <th style={{ width: '80px', textAlign: 'center' }}>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {isLoading ? (
                                    <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>Loading Delivery Points...</td></tr>
                                ) : filteredData.length > 0 ? (
                                    filteredData.map((row, idx) => (
                                        <tr key={idx}>
                                            <td>{row.idAction || row.IdAction}</td>
                                            <td>{row.codDeliveryPoint || row.CodDeliveryPoint}</td>
                                            <td>{row.codHier || row.CodHier}</td>
                                            <td>{row.codDiv || row.CodDiv}</td>
                                            <td>{row.codNode || row.CodNode}</td>
                                            <td>{row.idLevel ?? row.IdLevel}</td>
                                            <td>{row.flgInclusion ?? row.FlgInclusion ? 'Yes' : 'No'}</td>
                                            <td style={{ textAlign: 'center', position: 'relative' }}>
                                                <button
                                                    className="action-menu-btn"
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        setActiveKebab(activeKebab === idx ? null : idx);
                                                    }}
                                                >
                                                    <MoreVertical size={18} />
                                                </button>

                                                {activeKebab === idx && (
                                                    <div className="kebab-dropdown fade-in">
                                                        <button className="dropdown-item" onClick={() => handleEdit(row)}>
                                                            <Edit2 size={14} style={{ marginRight: '8px' }} /> Edit
                                                        </button>
                                                        <button className="dropdown-item delete" onClick={() => handleDelete(row)}>
                                                            <Trash2 size={14} style={{ marginRight: '8px' }} /> Delete
                                                        </button>
                                                    </div>
                                                )}
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>No delivery points found.</td></tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            {/* Edit Modal */}
            {showEditModal && (
                <div className="modal-overlay">
                    <div className="modal-content fade-in" style={{ maxWidth: '400px' }}>
                        <div className="modal-header">
                            <h3 className="modal-title">Edit Delivery Point: {editingDeliveryPoint.codDeliveryPoint || editingDeliveryPoint.CodDeliveryPoint}</h3>
                            <button className="close-btn" onClick={() => setShowEditModal(false)}>
                                <X size={20} />
                            </button>
                        </div>
                        <div className="modal-body">
                            <div className="form-group">
                                <label className="form-label">Flag Inclusion</label>
                                <div style={{ display: 'flex', gap: '20px', marginTop: '10px' }}>
                                    <label style={{ display: 'flex', alignItems: 'center', gap: '8px', cursor: 'pointer' }}>
                                        <input
                                            type="radio"
                                            checked={editFlagInclusion === true}
                                            onChange={() => setEditFlagInclusion(true)}
                                            style={{ cursor: 'pointer' }}
                                        />
                                        Yes (Include)
                                    </label>
                                    <label style={{ display: 'flex', alignItems: 'center', gap: '8px', cursor: 'pointer' }}>
                                        <input
                                            type="radio"
                                            checked={editFlagInclusion === false}
                                            onChange={() => setEditFlagInclusion(false)}
                                            style={{ cursor: 'pointer' }}
                                        />
                                        No (Exclude)
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div className="modal-footer" style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '24px', paddingTop: '16px', borderTop: '1px solid #e5e7eb' }}>
                            <button className="btn-secondary" onClick={() => setShowEditModal(false)}>Cancel</button>
                            <button className="btn-primary" onClick={handleUpdate}>Save Changes</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default DeliveryPoint;
