// Room data definition
const ROOM_DATA = {
    'standard': {
        name: 'Standard Room',
        price: 1806,
        rooms: [
            { number: '301', status: 'available' },
            { number: '302', status: 'available' },
            { number: '303', status: 'available' },
            { number: '304', status: 'available' },
            { number: '305', status: 'available' },
            { number: '306', status: 'available' },
            { number: '307', status: 'available' },
            { number: '308', status: 'available' },
            { number: '309', status: 'available' },
            { number: '310', status: 'available' },
            { number: '311', status: 'available' },
            { number: '312', status: 'available' },
            { number: '313', status: 'available' },
            { number: '314', status: 'available' },
            { number: '315', status: 'available' },
            { number: '316', status: 'available' },
            { number: '317', status: 'available' },
            { number: '318', status: 'available' },
            { number: '319', status: 'available' },
            { number: '320', status: 'available' },
            { number: '321', status: 'available' },
            { number: '322', status: 'available' },
            { number: '323', status: 'available' },
            { number: '324', status: 'available' },
            { number: '325', status: 'available' },
            { number: '326', status: 'available' },
            { number: '327', status: 'available' },
            { number: '328', status: 'available' },
            { number: '329', status: 'available' },
            { number: '330', status: 'available' }
        ]
    },
    'deluxe': {
        name: 'Deluxe Room',
        price: 3588,
        rooms: [
            { number: '401', status: 'available' },
            { number: '402', status: 'available' },
            { number: '403', status: 'available' },
            { number: '404', status: 'available' },
            { number: '405', status: 'available' },
            { number: '406', status: 'available' },
            { number: '407', status: 'available' },
            { number: '408', status: 'available' },
            { number: '409', status: 'available' },
            { number: '410', status: 'available' },
            { number: '411', status: 'available' },
            { number: '412', status: 'available' },
            { number: '413', status: 'available' },
            { number: '414', status: 'available' },
            { number: '415', status: 'available' },
            { number: '416', status: 'available' },
            { number: '417', status: 'available' },
            { number: '418', status: 'available' },
            { number: '419', status: 'available' },
            { number: '420', status: 'available' },
            { number: '421', status: 'available' },
            { number: '422', status: 'available' },
            { number: '423', status: 'available' },
            { number: '424', status: 'available' },
            { number: '425', status: 'available' },
            { number: '426', status: 'available' },
            { number: '427', status: 'available' },
            { number: '428', status: 'available' },
            { number: '429', status: 'available' },
            { number: '430', status: 'available' }
        ]
    },
    'suite': {
        name: 'Suite Room',
        price: 15000,
        rooms: [
            { number: '501', status: 'available' },
            { number: '502', status: 'available' },
            { number: '503', status: 'available' },
            { number: '504', status: 'available' },
            { number: '505', status: 'available' },
            { number: '506', status: 'available' },
            { number: '507', status: 'available' },
            { number: '508', status: 'available' },
            { number: '509', status: 'available' },
            { number: '510', status: 'available' },
            { number: '511', status: 'available' },
            { number: '512', status: 'available' },
            { number: '513', status: 'available' },
            { number: '514', status: 'available' },
            { number: '515', status: 'available' },
            { number: '516', status: 'available' },
            { number: '517', status: 'available' },
            { number: '518', status: 'available' },
            { number: '519', status: 'available' },
            { number: '520', status: 'available' },
            { number: '521', status: 'available' },
            { number: '522', status: 'available' },
            { number: '523', status: 'available' },
            { number: '524', status: 'available' },
            { number: '525', status: 'available' },
            { number: '526', status: 'available' },
            { number: '527', status: 'available' },
            { number: '528', status: 'available' },
            { number: '529', status: 'available' },
            { number: '530', status: 'available' }
        ]
    }
};

// Define room status constants
if (typeof window !== 'undefined') {
    window.RoomStatus = {
        AVAILABLE: 'available',
        OCCUPIED: 'occupied',
        MAINTENANCE: 'maintenance',
        CLEANING: 'cleaning'
    };
    
    // Create RoomStatusManager for managing room statuses
    window.RoomStatusManager = {
        // Initialize room data if not already initialized
        initializeRoomData: function() {
            if (!localStorage.getItem('roomData')) {
                localStorage.setItem('roomData', JSON.stringify(ROOM_DATA));
            }
        },
        
        // Get all rooms
        getAllRooms: function() {
            this.initializeRoomData();
            const roomData = JSON.parse(localStorage.getItem('roomData'));
            let allRooms = [];
            
            for (const type in roomData) {
                allRooms = allRooms.concat(roomData[type]);
            }
            
            return allRooms;
        },
        
        // Update a room's status
        updateRoomStatus: function(roomNumber, status) {
            this.initializeRoomData();
            const roomData = JSON.parse(localStorage.getItem('roomData'));
            let updated = false;
            
            for (const type in roomData) {
                for (let i = 0; i < roomData[type].length; i++) {
                    if (roomData[type][i].number === roomNumber) {
                        roomData[type][i].status = status;
                        roomData[type][i].statusUpdatedAt = new Date().toISOString();
                        updated = true;
                        break;
                    }
                }
                if (updated) break;
            }
            
            if (updated) {
                localStorage.setItem('roomData', JSON.stringify(roomData));
                
                // Trigger event to notify of status change
                const event = new CustomEvent('roomStatusChanged', {
                    detail: {
                        roomNumber: roomNumber,
                        newStatus: status
                    }
                });
                window.dispatchEvent(event);
            }
            
            return updated;
        },
        
        // Reset all room statuses
        resetAllRoomStatuses: function(reason = 'manual') {
            this.initializeRoomData();
            const roomData = JSON.parse(localStorage.getItem('roomData'));
            
            for (const type in roomData) {
                for (let i = 0; i < roomData[type].length; i++) {
                    roomData[type][i].status = window.RoomStatus.AVAILABLE;
                    roomData[type][i].statusUpdatedAt = new Date().toISOString();
                }
            }
            
            localStorage.setItem('roomData', JSON.stringify(roomData));
            
            // Set specific rooms as needed for demo
            if (reason === 'initialization') {
                this.updateRoomStatus('301', window.RoomStatus.OCCUPIED);
                this.updateRoomStatus('302', window.RoomStatus.MAINTENANCE);
                this.updateRoomStatus('303', window.RoomStatus.CLEANING);
            }
            
            // Trigger event to notify of reset
            const event = new CustomEvent('allRoomStatusesReset', {
                detail: { reason: reason }
            });
            window.dispatchEvent(event);
        },
        
        // Synchronize with the API to get the latest room statuses
        syncWithAPI: async function() {
            try {
                console.log("Syncing room statuses with API...");
                const response = await fetch('/api/Room');
                
                if (!response.ok) {
                    throw new Error('Failed to fetch room statuses from API');
                }
                
                const result = await response.json();
                
                if (result.success && result.data) {
                    // Initialize room data if not already initialized
                    this.initializeRoomData();
                    const roomData = JSON.parse(localStorage.getItem('roomData'));
                    let updatedCount = 0;
                    
                    // Update room statuses based on API data
                    result.data.forEach(apiRoom => {
                        for (const type in roomData) {
                            for (let i = 0; i < roomData[type].length; i++) {
                                if (roomData[type][i].number === apiRoom.roomNumber) {
                                    const oldStatus = roomData[type][i].status;
                                    
                                    // Only update if status is different
                                    if (oldStatus !== apiRoom.status) {
                                        roomData[type][i].status = apiRoom.status;
                                        roomData[type][i].statusUpdatedAt = apiRoom.statusUpdatedAt || new Date().toISOString();
                                        updatedCount++;
                                        
                                        // Trigger event for this specific room change
                                        const event = new CustomEvent('roomStatusChanged', {
                                            detail: {
                                                roomNumber: apiRoom.roomNumber,
                                                newStatus: apiRoom.status,
                                                oldStatus: oldStatus
                                            }
                                        });
                                        window.dispatchEvent(event);
                                    }
                                    break;
                                }
                            }
                        }
                    });
                    
                    // Save updated data to localStorage
                    localStorage.setItem('roomData', JSON.stringify(roomData));
                    console.log(`Synced with API: updated ${updatedCount} room statuses`);
                    
                    if (updatedCount > 0) {
                        // Trigger a general update event
                        const event = new CustomEvent('roomStatusesSynced', {
                            detail: { updatedCount: updatedCount }
                        });
                        window.dispatchEvent(event);
                    }
                    
                    return true;
                } else {
                    console.error("API returned an error or no data:", result);
                    return false;
                }
            } catch (error) {
                console.error("Error syncing with API:", error);
                return false;
            }
        }
    };
    
    // Initialize room data when the script loads
    window.RoomStatusManager.initializeRoomData();

    // Attempt to sync with the API when the page loads
    document.addEventListener('DOMContentLoaded', function() {
        // Sync with a slight delay to ensure everything is ready
        setTimeout(() => window.RoomStatusManager.syncWithAPI(), 500);
    });
} 