import React, { useState, useEffect } from 'react';
import { Users, Eye, Search, ChevronRight, MoreVertical, Edit2, Trash2, Check, X, Plus } from 'lucide-react';
import './CustomerRelation.css'; // Reusing established table styles
import { promotionService } from '../services/promotionService';

const Participant = ({ canEdit = false }) => {
    const [showForm, setShowForm] = useState(false);
    const [participants, setParticipants] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
    const [searchCriterion, setSearchCriterion] = useState('Select an option');
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    const [activeActionMenu, setActiveActionMenu] = useState(null);
    const [showEditModal, setShowEditModal] = useState(false);
    const [editingParticipant, setEditingParticipant] = useState(null);
    const [editFlagInclusion, setEditFlagInclusion] = useState(true);

    // Pagination State
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(5);

    // Promotion Linking States
    const [promotions, setPromotions] = useState([]);
    const [showPromotionModal, setShowPromotionModal] = useState(false);
    const [promotionSearchTerm, setPromotionSearchTerm] = useState('');
    const [showParticipantModal, setShowParticipantModal] = useState(false);
    const [participantSearchTerm, setParticipantSearchTerm] = useState('');
    const [selectedParticipants, setSelectedParticipants] = useState([]);
    const [customerRelations, setCustomerRelations] = useState([]);

    // Form Data
    const [formData, setFormData] = useState({
        promotion: null,
        isNewParticipant: false,
        codParticipant: '',
        codHier: '',
        codDiv: '',
        codNode: '',
        idLevel: 0,
        parentNode: '',
        startDate: '',
        endDate: '',
        flgInclusion: false
    });

    useEffect(() => {
        fetchParticipants();
        fetchCustomerRelations();
    }, []);

    useEffect(() => {
        if (showPromotionModal && promotions.length === 0) {
            fetchPromotions();
        }
    }, [showPromotionModal]);

    const fetchParticipants = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const data = await promotionService.getParticipants();
            setParticipants(data || []);
        } catch (err) {
            console.error("Error fetching participants:", err);
            setError(err.message || "Failed to fetch participants");
        } finally {
            setIsLoading(false);
        }
    };

    const fetchPromotions = async () => {
        try {
            const data = await promotionService.getPromoActions();
            setPromotions(data || []);
        } catch (error) {
            console.error("Error fetching promotions:", error);
        }
    };

    const fetchCustomerRelations = async () => {
        try {
            const { customerService } = await import('../services/customerService');
            const data = await customerService.getCustomers();
            setCustomerRelations(data || []);
        } catch (error) {
            console.error("Error fetching customer relations:", error);
        }
    };

    const handleEdit = (participant) => {
        setEditingParticipant(participant);
        setEditFlagInclusion(participant.flgInclusion ?? participant.FlgInclusion ?? true);
        setShowEditModal(true);
        setActiveActionMenu(null);
    };

    const handleUpdate = async () => {
        const idAction = editingParticipant.idAction || editingParticipant.IdAction;
        const codParticipant = editingParticipant.codParticipant || editingParticipant.CodParticipant;

        try {
            await promotionService.updateParticipant(idAction, codParticipant, editFlagInclusion);
            alert("Participant updated successfully!");
            setShowEditModal(false);
            fetchParticipants();
        } catch (err) {
            console.error("Error updating participant:", err);
            alert("Failed to update participant: " + err.message);
        }
    };

    const handleDelete = async (participant) => {
        const idAction = participant.idAction || participant.IdAction;
        const codParticipant = participant.codParticipant || participant.CodParticipant;

        if (window.confirm(`Are you sure you want to delete participant ${codParticipant}?`)) {
            try {
                await promotionService.deleteParticipant(idAction, codParticipant);
                alert("Participant deleted successfully!");
                fetchParticipants();
            } catch (err) {
                console.error("Error deleting participant:", err);
                alert("Failed to delete participant: " + err.message);
            }
        }
        setActiveActionMenu(null);
    };

    const handlePromotionSelect = (promo) => {
        setFormData(prev => ({
            ...prev,
            promotion: promo
        }));
        setShowPromotionModal(false);
    };

    const toggleParticipantSelection = (participant) => {
        const isSelected = selectedParticipants.some(p => (p.codParticipant || p.CodParticipant) === (participant.codParticipant || participant.CodParticipant));
        if (isSelected) {
            setSelectedParticipants(prev => prev.filter(p => (p.codParticipant || p.CodParticipant) !== (participant.codParticipant || participant.CodParticipant)));
        } else {
            setSelectedParticipants(prev => [...prev, participant]);
        }
    };

    const removeSelectedParticipant = (indexToRemove) => {
        setSelectedParticipants(prev => prev.filter((_, idx) => idx !== indexToRemove));
    };

    const handleInputChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : value
        }));
    };

    const handleSubmit = async () => {
        if (!formData.promotion) {
            alert("Please select a promotion.");
            return;
        }

        if (formData.isNewParticipant) {
            // NEW PARTICIPANT LOGIC
            if (!formData.codParticipant) {
                alert("Please enter a Participant Code.");
                return;
            }

            try {
                const promoId = formData.promotion.idAction || formData.promotion.IdAction;

                const newParticipantPayload = {
                    idAction: promoId,
                    codParticipant: formData.codParticipant,
                    codHier: formData.codHier,
                    codDiv: formData.codDiv,
                    codNode: formData.codNode,
                    idLevel: parseInt(formData.idLevel) || 0,
                    flgInclusion: formData.flgInclusion,
                    dteStart: formData.startDate || new Date().toISOString()
                };

                await promotionService.createParticipant(newParticipantPayload);
                await fetchParticipants();

                alert(`Successfully Created & Linked New Participant: ${newParticipantPayload.codParticipant}`);
            } catch (error) {
                console.error("Error creating/linking participant:", error);
                const errorMsg = error.response?.data ? JSON.stringify(error.response.data) : error.message;
                alert("Failed to save participant: " + errorMsg);
            }

        } else {
            // EXISTING PARTICIPANT LOGIC
            if (selectedParticipants.length === 0) {
                alert("Please select at least one participant.");
                return;
            }

            try {
                const promoId = formData.promotion.idAction || formData.promotion.IdAction;

                const participantsToLink = selectedParticipants.map(p => ({
                    ...p,
                    idAction: parseInt(promoId)
                }));

                await promotionService.addParticipantsToPromotion(participantsToLink);
                await fetchParticipants();

                alert(`Successfully linked ${selectedParticipants.length} participant(s) to Promotion #${promoId}!`);
            } catch (error) {
                console.error("Error linking participants:", error);
                const errorMsg = error.response?.data ? JSON.stringify(error.response.data) : error.message;
                alert("Failed to link participants: " + errorMsg);
            }
        }

        // Reset form
        setShowForm(false);
        setFormData({
            promotion: null,
            isNewParticipant: false,
            codParticipant: '',
            codHier: '',
            codDiv: '',
            codNode: '',
            idLevel: 0,
            parentNode: '',
            startDate: '',
            endDate: '',
            flgInclusion: false
        });
        setSelectedParticipants([]);
    };

    const filteredData = participants.filter(p => {
        if (searchCriterion === 'Select an option' || !searchTerm) return true;

        const term = searchTerm.toLowerCase();
        switch (searchCriterion) {
            case 'ID Action': return (p.idAction || p.IdAction || '').toString().toLowerCase().includes(term);
            case 'Cod Participant': return (p.codParticipant || p.CodParticipant || '').toString().toLowerCase().includes(term);
            case 'Code Hier': return (p.codHier || p.CodHier || '').toString().toLowerCase().includes(term);
            case 'Code Div': return (p.codDiv || p.CodDiv || '').toString().toLowerCase().includes(term);
            case 'Code Node': return (p.codNode || p.CodNode || '').toString().toLowerCase().includes(term);
            case 'ID Level': return (p.idLevel ?? p.IdLevel ?? '').toString().toLowerCase().includes(term);
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

    const filteredPromotions = promotions.filter(p => {
        if (!promotionSearchTerm) return true;
        const term = promotionSearchTerm.toLowerCase();
        return (p.name || p.Name || '').toLowerCase().includes(term) ||
            (p.idAction || p.IdAction || '').toString().toLowerCase().includes(term);
    });

    const filteredModalParticipants = participants.filter(p => {
        if (!participantSearchTerm) return true;
        const term = participantSearchTerm.toLowerCase();
        return (p.codParticipant || p.CodParticipant || '').toString().toLowerCase().includes(term) ||
            (p.codNode || p.CodNode || '').toString().toLowerCase().includes(term);
    });

    return (
        <div className="cr-container">
            {!showForm ? (
                <div className="cr-table-container fade-in">
                    <div className="search-by-section">
                        <h3 className="search-by-title">Participants List</h3>
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
                                            {['Select an option', 'ID Action', 'Cod Participant', 'Code Hier', 'Code Div', 'Code Node', 'ID Level'].map(opt => (
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
                                <button className="search-action-btn" onClick={fetchParticipants}>
                                    <Eye size={18} /> Refresh
                                </button>
                            </div>

                            {canEdit && (
                                <button className="create-btn" onClick={() => setShowForm(true)}>
                                    <Plus size={18} />
                                    Add Participant
                                </button>
                            )}
                        </div>

                        <div className="view-table-wrapper">
                            <table className="view-data-table">
                                <thead>
                                    <tr>
                                        <th>ID Action</th>
                                        <th>Cod Participant</th>
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
                                        <tr><td colSpan="8" style={{ textAlign: 'center', padding: '40px' }}>Loading Participants...</td></tr>
                                    ) : currentItems.length > 0 ? (
                                        currentItems.map((row, idx) => (
                                            <tr key={idx}>
                                                <td>{row.idAction || row.IdAction}</td>
                                                <td>{row.codParticipant || row.CodParticipant}</td>
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
                                                            setActiveActionMenu(activeActionMenu === idx ? null : idx);
                                                        }}
                                                    >
                                                        <MoreVertical size={18} />
                                                    </button>

                                                    {activeActionMenu === idx && (
                                                        <div className="action-menu fade-in">
                                                            {canEdit ? (
                                                                <>
                                                                    <button className="dropdown-item" onClick={() => handleEdit(row)}>
                                                                        <Edit2 size={14} style={{ marginRight: '8px' }} /> Edit
                                                                    </button>
                                                                    <button className="dropdown-item delete" onClick={() => handleDelete(row)}>
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
                                    ) : (
                                        <tr><td colSpan="8" style={{ textAlign: 'center', padding: '40px' }}>No participants found.</td></tr>
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
                /* ADD PARTICIPANT FORM */
                <div className="form-card fade-in">
                    <div className="form-header">
                        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                            <div className="icon-wrapper">
                                <Users size={22} />
                            </div>
                            <h2 className="form-title">Add Participant</h2>
                        </div>
                        <button className="close-btn" onClick={() => setShowForm(false)}>
                            <X size={20} />
                        </button>
                    </div>

                    <div className="form-body">
                        <div className="form-section">
                            <h3 className="section-title">Participant Details</h3>
                            <p className="section-subtitle">Select a promotion and the participants to add.</p>

                            {/* Promotion Selection */}
                            <div className="form-row">
                                <div className="form-col">
                                    <label className="form-label">Select Promotion <span className="required">*</span></label>
                                    <div
                                        className="form-input"
                                        style={{ cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'space-between', backgroundColor: '#f9fafb' }}
                                        onClick={() => setShowPromotionModal(true)}
                                    >
                                        <span style={{ color: formData.promotion ? '#111827' : '#9ca3af' }}>
                                            {formData.promotion ? (formData.promotion.name || formData.promotion.Name) : 'Click to select a promotion'}
                                        </span>
                                        <ChevronRight size={16} color="#9ca3af" />
                                    </div>
                                </div>
                            </div>

                            {/* Mode Selection: Existing vs New */}
                            <div className="form-row" style={{ marginBottom: '24px' }}>
                                <div style={{ display: 'flex', gap: '16px', borderBottom: '1px solid #e5e7eb', width: '100%' }}>
                                    <button
                                        onClick={() => { setSelectedParticipants([]); setFormData(prev => ({ ...prev, isNewParticipant: false })); }}
                                        style={{
                                            padding: '10px 16px',
                                            color: !formData.isNewParticipant ? '#9333ea' : '#6b7280',
                                            fontWeight: !formData.isNewParticipant ? '600' : '500',
                                            background: 'none', border: 'none', borderBottom: !formData.isNewParticipant ? '2px solid #9333ea' : 'transparent', cursor: 'pointer'
                                        }}
                                    >
                                        Select Existing Participants
                                    </button>
                                    <button
                                        onClick={() => { setSelectedParticipants([]); setFormData(prev => ({ ...prev, isNewParticipant: true })); }}
                                        style={{
                                            padding: '10px 16px',
                                            color: formData.isNewParticipant ? '#9333ea' : '#6b7280',
                                            fontWeight: formData.isNewParticipant ? '600' : '500',
                                            background: 'none', border: 'none', borderBottom: formData.isNewParticipant ? '2px solid #9333ea' : 'transparent', cursor: 'pointer'
                                        }}
                                    >
                                        Create New Participant
                                    </button>
                                </div>
                            </div>

                            {!formData.isNewParticipant ? (
                                /* EXISTING PARTICIPANTS FLOW */
                                <div className="form-row" style={{ display: 'block' }}>
                                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '8px' }}>
                                        <label className="form-label" style={{ marginBottom: 0 }}>Selected Participants <span className="required">*</span></label>
                                        <button
                                            type="button"
                                            style={{ color: '#9333ea', border: 'none', background: 'none', cursor: 'pointer', fontWeight: '600', fontSize: '15px' }}
                                            onClick={() => setShowParticipantModal(true)}
                                        >
                                            + Select Participants
                                        </button>
                                    </div>

                                    {selectedParticipants.length > 0 ? (
                                        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px', padding: '12px', backgroundColor: '#f9fafb', borderRadius: '8px', border: '1px solid #e5e7eb' }}>
                                            {selectedParticipants.map((participant, idx) => (
                                                <div
                                                    key={idx}
                                                    style={{
                                                        display: 'flex', alignItems: 'center', gap: '8px',
                                                        padding: '6px 12px', backgroundColor: '#ede9fe', color: '#7e22ce',
                                                        borderRadius: '6px', fontSize: '14px', fontWeight: '500'
                                                    }}
                                                >
                                                    <span>{participant.codParticipant || participant.CodParticipant}</span>
                                                    <button
                                                        type="button"
                                                        onClick={() => removeSelectedParticipant(idx)}
                                                        style={{ background: 'none', border: 'none', padding: 0, cursor: 'pointer', display: 'flex', alignItems: 'center' }}
                                                    >
                                                        <X size={14} />
                                                    </button>
                                                </div>
                                            ))}
                                        </div>
                                    ) : (
                                        <div style={{ padding: '20px', textAlign: 'center', color: '#9ca3af', backgroundColor: '#f9fafb', borderRadius: '8px', border: '1px dashed #d1d5db' }}>
                                            No participants selected. Click "+ Select Participants" to choose.
                                        </div>
                                    )}
                                </div>
                            ) : (
                                /* NEW PARTICIPANT FLOW */
                                <>
                                    <div className="form-row">
                                        <div className="form-col">
                                            <label className="form-label">Participant Code <span className="required">*</span></label>
                                            <input
                                                type="text"
                                                name="codParticipant"
                                                className="form-input"
                                                value={formData.codParticipant}
                                                onChange={handleInputChange}
                                                placeholder="Enter participant code"
                                            />
                                        </div>
                                        <div className="form-col">
                                            <label className="form-label">Code Hier</label>
                                            <input
                                                type="text"
                                                name="codHier"
                                                className="form-input"
                                                value={formData.codHier}
                                                onChange={handleInputChange}
                                                placeholder="Enter hierarchy code"
                                            />
                                        </div>
                                    </div>

                                    <div className="form-row">
                                        <div className="form-col">
                                            <label className="form-label">Code Div</label>
                                            <input
                                                type="text"
                                                name="codDiv"
                                                className="form-input"
                                                value={formData.codDiv}
                                                onChange={handleInputChange}
                                                placeholder="Enter division code"
                                            />
                                        </div>
                                        <div className="form-col">
                                            <label className="form-label">Code Node</label>
                                            <input
                                                type="text"
                                                name="codNode"
                                                className="form-input"
                                                value={formData.codNode}
                                                onChange={handleInputChange}
                                                placeholder="Enter node code"
                                            />
                                        </div>
                                    </div>

                                    <div className="form-row">
                                        <div className="form-col">
                                            <label className="form-label">Parent Node</label>
                                            <input
                                                type="text"
                                                name="parentNode"
                                                className="form-input"
                                                value={formData.parentNode || ''}
                                                onChange={handleInputChange}
                                                placeholder="Enter parent node"
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
                                            <label className="form-label">Start Date <span className="required">*</span></label>
                                            <input
                                                type="date"
                                                name="startDate"
                                                className="form-input"
                                                value={formData.startDate || ''}
                                                onChange={handleInputChange}
                                            />
                                        </div>
                                        <div className="form-col">
                                            <label className="form-label">End Date <span className="required">*</span></label>
                                            <input
                                                type="date"
                                                name="endDate"
                                                className="form-input"
                                                value={formData.endDate || ''}
                                                onChange={handleInputChange}
                                            />
                                        </div>
                                    </div>

                                    <div className="form-row">
                                        <div className="form-col">
                                            <label className="form-label" style={{ display: 'flex', alignItems: 'center', gap: '8px', cursor: 'pointer' }}>
                                                <input
                                                    type="checkbox"
                                                    name="flgInclusion"
                                                    checked={formData.flgInclusion}
                                                    onChange={handleInputChange}
                                                    style={{ cursor: 'pointer', width: '18px', height: '18px' }}
                                                />
                                                Flag Inclusion
                                            </label>
                                        </div>
                                    </div>
                                </>
                            )}
                        </div>

                        <div className="submit-btn-wrapper">
                            <button className="submit-btn" onClick={handleSubmit}>
                                {formData.isNewParticipant ? 'Create & Link Participant' : 'Link Selected Participants'} <Check size={18} />
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Edit Modal */}
            {showEditModal && (
                <div className="modal-overlay">
                    <div className="modal-content fade-in" style={{ maxWidth: '400px' }}>
                        <div className="modal-header">
                            <h3 className="modal-title">Edit Participant: {editingParticipant.codParticipant || editingParticipant.CodParticipant}</h3>
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

            {/* Promotion Selection Modal */}
            {showPromotionModal && (
                <div className="modal-overlay" onClick={() => setShowPromotionModal(false)}>
                    <div className="modal-content" style={{ maxWidth: '700px' }} onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                <div className="icon-wrapper">
                                    <Users size={20} />
                                </div>
                                <h2 className="modal-title">Select Promotion</h2>
                            </div>
                            <button className="close-btn" onClick={() => setShowPromotionModal(false)}>
                                <X size={20} />
                            </button>
                        </div>
                        <div className="modal-body">
                            <div className="search-controls" style={{ padding: '0', marginBottom: '20px' }}>
                                <input
                                    type="text"
                                    className="search-term-input"
                                    placeholder="Search by Name or ID"
                                    value={promotionSearchTerm}
                                    onChange={(e) => setPromotionSearchTerm(e.target.value)}
                                />
                            </div>
                            <div className="view-table-wrapper" style={{ maxHeight: '400px', overflowY: 'auto' }}>
                                <table className="view-data-table">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Name</th>
                                            <th>Division</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {filteredPromotions.length > 0 ? (
                                            filteredPromotions.map((promo, idx) => (
                                                <tr
                                                    key={idx}
                                                    onClick={() => handlePromotionSelect(promo)}
                                                    style={{ cursor: 'pointer' }}
                                                    className="hover:bg-purple-50"
                                                >
                                                    <td>{promo.idAction || promo.IdAction}</td>
                                                    <td>{promo.name || promo.Name}</td>
                                                    <td>{promo.codDiv || promo.CodDiv}</td>
                                                </tr>
                                            ))
                                        ) : (
                                            <tr><td colSpan="3" style={{ textAlign: 'center', padding: '20px' }}>No promotions found.</td></tr>
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Participant Selection Modal */}
            {showParticipantModal && (
                <div className="modal-overlay" onClick={() => setShowParticipantModal(false)}>
                    <div className="modal-content" style={{ maxWidth: '800px' }} onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                <div className="icon-wrapper" style={{ background: '#E0F2FE', color: '#0EA5E9' }}>
                                    <Users size={20} />
                                </div>
                                <h2 className="modal-title">Select Participants to Add</h2>
                            </div>
                            <button className="close-btn" onClick={() => setShowParticipantModal(false)}>
                                <X size={20} />
                            </button>
                        </div>
                        <div className="modal-body">
                            <div className="search-controls" style={{ padding: '0', marginBottom: '20px', gap: '12px' }}>
                                <input
                                    type="text"
                                    className="search-term-input"
                                    placeholder="Search Participants by Code or Name"
                                    value={participantSearchTerm}
                                    onChange={(e) => setParticipantSearchTerm(e.target.value)}
                                />
                                <button className="create-btn" onClick={() => setShowParticipantModal(false)}>
                                    Confirm Selection ({selectedParticipants.length})
                                </button>
                            </div>
                            <div className="view-table-wrapper" style={{ maxHeight: '400px', overflowY: 'auto' }}>
                                <table className="view-data-table">
                                    <thead>
                                        <tr>
                                            <th style={{ width: '50px' }}>Select</th>
                                            <th>Code</th>
                                            <th>Hier</th>
                                            <th>Div</th>
                                            <th>Node</th>
                                            <th>Level</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {filteredModalParticipants.length > 0 ? (
                                            filteredModalParticipants.map((participant, idx) => {
                                                const isSelected = selectedParticipants.some(p => (p.codParticipant || p.CodParticipant) === (participant.codParticipant || participant.CodParticipant));
                                                return (
                                                    <tr
                                                        key={idx}
                                                        onClick={() => toggleParticipantSelection(participant)}
                                                        style={{ cursor: 'pointer', backgroundColor: isSelected ? '#f0fdf4' : 'inherit' }}
                                                        className="hover:bg-gray-50"
                                                    >
                                                        <td style={{ textAlign: 'center' }}>
                                                            <div style={{
                                                                width: '18px',
                                                                height: '18px',
                                                                border: isSelected ? 'none' : '2px solid #d1d5db',
                                                                borderRadius: '4px',
                                                                backgroundColor: isSelected ? '#22c55e' : 'white',
                                                                display: 'flex',
                                                                alignItems: 'center',
                                                                justifyContent: 'center',
                                                                color: 'white',
                                                                margin: '0 auto'
                                                            }}>
                                                                {isSelected && <Check size={12} strokeWidth={4} />}
                                                            </div>
                                                        </td>
                                                        <td>{participant.codParticipant || participant.CodParticipant}</td>
                                                        <td>{participant.codHier || participant.CodHier}</td>
                                                        <td>{participant.codDiv || participant.CodDiv}</td>
                                                        <td>{participant.codNode || participant.CodNode}</td>
                                                        <td>{participant.idLevel ?? participant.IdLevel}</td>
                                                    </tr>
                                                );
                                            })
                                        ) : (
                                            <tr><td colSpan="6" style={{ textAlign: 'center', padding: '20px' }}>No participants found.</td></tr>
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Participant;
