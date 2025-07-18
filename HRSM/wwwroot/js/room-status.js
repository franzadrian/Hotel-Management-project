// NUCLEAR ALERT BLOCKER
(function() {
    console.log('INSTALLING NUCLEAR ALERT BLOCKER IN ROOM-STATUS.JS');
    
    // Store original methods
    var _originalAlert = window.alert;
    
    // Override alert to be a no-op function for certain messages
    window.alert = function(message) {
        if (typeof message === 'string' && 
            (message.includes('room availability') || 
             message.includes('Room availability') || 
             message.includes('rooms are now available') ||
             message.includes('available for booking') ||
             message.includes('available rooms') ||
             message.includes('Available rooms') ||
             message.toLowerCase().includes('room') && message.toLowerCase().includes('availab'))) {
            console.log('🚫 NUCLEAR BLOCKER (room-status.js): Prevented alert:', message);
            return;
        }
        return _originalAlert.apply(this, arguments);
    };
    
    console.log('NUCLEAR ALERT BLOCKER INSTALLED IN ROOM-STATUS.JS');
})();

// Room status enum
const RoomStatus = {
    AVAILABLE: 'available',
    OCCUPIED: 'occupied',
    CLEANING: 'cleaning',
    MAINTENANCE: 'maintenance'
};

// Function to initialize room data if it doesn't exist yet
function initializeRoomData() {
    if (!localStorage.getItem('roomData')) {
        // Create default room data
        const roomData = {
            // Standard rooms (301-330)
            standard: Array.from({ length: 30 }, (_, i) => ({
                roomNumber: `${i+301}`,
                type: 'standard',
                status: RoomStatus.AVAILABLE,
                statusUpdatedAt: new Date().toISOString(),
                updatedBy: 'system'
            })),
            // Deluxe rooms (401-430)
            deluxe: Array.from({ length: 30 }, (_, i) => ({
                roomNumber: `${i+401}`,
                type: 'deluxe',
                status: RoomStatus.AVAILABLE,
                statusUpdatedAt: new Date().toISOString(),
                updatedBy: 'system'
            })),
            // Suite rooms (501-530)
            suite: Array.from({ length: 30 }, (_, i) => ({
                roomNumber: `${i+501}`,
                type: 'suite',
                status: RoomStatus.AVAILABLE,
                statusUpdatedAt: new Date().toISOString(),
                updatedBy: 'system'
            }))
        };
        
        // Save to local storage
        localStorage.setItem('roomData', JSON.stringify(roomData));
        console.log('Room data initialized with all rooms available');
    }
}

// Function to set some initial room statuses for demo purposes
function setInitialRoomStatuses() {
    // Set some standard rooms as occupied
    updateRoomStatus('305', RoomStatus.OCCUPIED);
    updateRoomStatus('310', RoomStatus.OCCUPIED);
    updateRoomStatus('315', RoomStatus.OCCUPIED);
    updateRoomStatus('320', RoomStatus.OCCUPIED);
    
    // Set some standard rooms as cleaning
    updateRoomStatus('307', RoomStatus.CLEANING);
    updateRoomStatus('312', RoomStatus.CLEANING);
    updateRoomStatus('325', RoomStatus.CLEANING);
    
    // Set some standard rooms as maintenance
    // Removed 303 from maintenance to fix the issue
    updateRoomStatus('330', RoomStatus.MAINTENANCE);
    
    // Set some deluxe rooms as occupied
    updateRoomStatus('405', RoomStatus.OCCUPIED);
    updateRoomStatus('410', RoomStatus.OCCUPIED);
    updateRoomStatus('415', RoomStatus.OCCUPIED);
    
    // Set some deluxe rooms as cleaning
    updateRoomStatus('420', RoomStatus.CLEANING);
    updateRoomStatus('425', RoomStatus.CLEANING);
    
    // Set some deluxe rooms as maintenance
    updateRoomStatus('430', RoomStatus.MAINTENANCE);
    
    // Set some suite rooms as occupied
    updateRoomStatus('505', RoomStatus.OCCUPIED);
    updateRoomStatus('510', RoomStatus.OCCUPIED);
    
    // Set some suite rooms as cleaning
    updateRoomStatus('515', RoomStatus.CLEANING);
    
    // Set some suite rooms as maintenance
    updateRoomStatus('520', RoomStatus.MAINTENANCE);
    
    console.log('Initial room statuses set for demonstration');
}

// Helper function to determine room type from room number
function getRoomTypeFromNumber(roomNumber) {
    const roomNum = parseInt(roomNumber, 10);
    if (roomNum >= 301 && roomNum <= 330) return 'standard';
    if (roomNum >= 401 && roomNum <= 430) return 'deluxe';
    if (roomNum >= 501 && roomNum <= 530) return 'suite';
    return null;
}

// Function to get all rooms
function getAllRooms() {
    initializeRoomData();
    const roomData = JSON.parse(localStorage.getItem('roomData'));
    
    // Combine all room types into a single array
    return [
        ...roomData.standard,
        ...roomData.deluxe,
        ...roomData.suite
    ];
}

// Function to get rooms by type
function getRoomsByType(type) {
    initializeRoomData();
    const roomData = JSON.parse(localStorage.getItem('roomData'));
    
    return roomData[type] || [];
}

// Function to get a specific room by number
function getRoomByNumber(roomNumber) {
    const allRooms = getAllRooms();
    return allRooms.find(room => room.roomNumber === roomNumber) || null;
}

// Function to update room status
function updateRoomStatus(roomNumber, newStatus, updatedBy = 'staff') {
    // Make sure room data is initialized
    initializeRoomData();
    
    // Get the latest data from localStorage
    const roomData = JSON.parse(localStorage.getItem('roomData'));
    
    // Determine the room type from the room number
    const roomType = getRoomTypeFromNumber(roomNumber);
    if (!roomType) {
        console.error(`Invalid room number: ${roomNumber}`);
        return false;
    }
    
    // Find the room index in the appropriate array
    const roomIndex = roomData[roomType].findIndex(room => room.roomNumber === roomNumber);
    if (roomIndex === -1) {
        console.error(`Room ${roomNumber} not found in ${roomType} rooms`);
        return false;
    }
    
    // Update the room status
    roomData[roomType][roomIndex].status = newStatus;
    roomData[roomType][roomIndex].statusUpdatedAt = new Date().toISOString();
    roomData[roomType][roomIndex].updatedBy = updatedBy;
    
    // Save updated data back to local storage
    localStorage.setItem('roomData', JSON.stringify(roomData));
    
    // Dispatch a custom event to notify other pages about the status change
    const statusChangeEvent = new CustomEvent('roomStatusChanged', {
        detail: {
            roomNumber: roomNumber,
            roomType: roomType,
            newStatus: newStatus,
            updatedAt: roomData[roomType][roomIndex].statusUpdatedAt
        }
    });
    window.dispatchEvent(statusChangeEvent);
    
    console.log(`Room ${roomNumber} status updated to ${newStatus}`);
    return true;
}

// Function to check if a room is available for booking
function isRoomAvailable(roomNumber) {
    const room = getRoomByNumber(roomNumber);
    return room && room.status === RoomStatus.AVAILABLE;
}

// Function to get all rooms with a specific status
function getRoomsByStatus(status) {
    const allRooms = getAllRooms();
    return allRooms.filter(room => room.status === status);
}

// Function to get room status by room number
function getRoomStatus(roomNumber) {
    const room = getRoomByNumber(roomNumber);
    return room ? room.status : null;
}

// Function to reset all room statuses (for testing)
function resetAllRoomStatuses(source = 'manual') {
    // Check for global silent flag
    if (window.isSilentReset === true) {
        console.log('Silent room reset detected via global flag - suppressing events');
        localStorage.removeItem('roomData');
        initializeRoomData();
        return true;
    }
    
    // Check if source indicates a silent operation
    if (source === 'silent' || source === 'initialization') {
        console.log('Silent room reset requested via parameter - suppressing events');
        localStorage.removeItem('roomData');
        initializeRoomData();
        return true;
    }
    
    localStorage.removeItem('roomData');
    initializeRoomData();
    
    // NUCLEAR OPTION: NEVER dispatch this event by default to avoid alerts
    // Only dispatch the event if this is a manual reset from the admin panel
    if (source === 'admin_panel') {
        // Dispatch a global event to notify all pages that all rooms have been reset
        const resetEvent = new CustomEvent('allRoomStatusesReset', {
            detail: {
                timestamp: new Date().toISOString(),
                source: source
            }
        });
        window.dispatchEvent(resetEvent);
    }
    
    console.log('All room statuses have been reset to Available');
    return true;
}

// Initialize room data when script is loaded
initializeRoomData();

// Reset all room statuses to match database
resetAllRoomStatuses('initialization');

// Ensure rooms 301, 302, and 303 are explicitly set to available
updateRoomStatus('301', RoomStatus.AVAILABLE);
updateRoomStatus('302', RoomStatus.AVAILABLE);
updateRoomStatus('303', RoomStatus.AVAILABLE);

// Listen for database changes from server
document.addEventListener('DOMContentLoaded', function() {
    // Check if we need to sync with database (this would be set by server-side code)
    if (window.syncRoomStatusWithDatabase) {
        console.log('Syncing room status with database...');
        // This would typically be an AJAX call to get the latest room statuses
        // For now, we'll just reset to match what's in the database
        resetAllRoomStatuses('initialization'); // Use initialization source to prevent alerts
    }
});

// Add a listener for the page visibility to refresh room data when tab becomes visible again
document.addEventListener('visibilitychange', function() {
    if (document.visibilityState === 'visible') {
        console.log('Page became visible, refreshing room data...');
        // Reload room data from localStorage (which might have been updated by other tabs)
        const roomData = JSON.parse(localStorage.getItem('roomData'));
        if (roomData) {
            // Notify the UI that room data might have changed
            const refreshEvent = new CustomEvent('roomDataRefreshed', {
                detail: { timestamp: new Date().toISOString() }
            });
            window.dispatchEvent(refreshEvent);
        }
    }
});

// Export functions for use in other scripts
window.RoomStatus = RoomStatus;
window.RoomStatusManager = {
    getAllRooms,
    getRoomsByType,
    getRoomByNumber,
    updateRoomStatus,
    isRoomAvailable,
    getRoomsByStatus,
    resetAllRoomStatuses,
    getRoomStatus
}; 

// Add a console log for testing in browser
console.log('Room status manager loaded successfully');