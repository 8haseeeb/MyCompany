import React, { useState, useEffect } from 'react';
import { Users, Eye, Search, ChevronRight, MoreVertical, Edit2, Trash2, Check, X } from 'lucide-react';
import './CustomerRelation.css'; // Reusing established table styles
import { promotionService } from '../services/promotionService';

const Participant = () => {
    const [participants, setParticipants] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
    const [searchCriterion, setSearchCriterion] = useState('Select an option');
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    const [activeKebab, setActiveKebab] = useState(null);
    const [showEditModal, setShowEditModal] = useState(false);
    const [editingParticipant, setEditingParticipant] = useState(null);
    const [editFlagInclusion, setEditFlagInclusion] = useState(true);

    useEffect(() => {
        fetchParticipants();
    }, []);

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

    const handleEdit = (participant) => {
        setEditingParticipant(participant);
        setEditFlagInclusion(participant.flgInclusion ?? participant.FlgInclusion ?? true);
        setShowEditModal(true);
        setActiveKebab(null);
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
        setActiveKebab(null);
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

    return (
        <div className="cr-container">
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
                                    <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>Loading Participants...</td></tr>
                                ) : filteredData.length > 0 ? (
                                    filteredData.map((row, idx) => (
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
                                    <tr><td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>No participants found.</td></tr>
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
        </div>
    );
};

export default Participant;
