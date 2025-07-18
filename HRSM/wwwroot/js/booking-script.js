// NUCLEAR OPTION: Completely replace the browser's alert function with a version that does nothing
// This is placed at the very top of the file to ensure it runs before anything else
(function() {
    console.log('INSTALLING NUCLEAR ALERT BLOCKER');
    
    // Store original methods
    var _originalAlert = window.alert;
    var _originalDispatchEvent = EventTarget.prototype.dispatchEvent;
    var _originalAddEventListener = EventTarget.prototype.addEventListener;
    
    // Override alert to be a no-op function for certain messages
    window.alert = function(message) {
        if (typeof message === 'string' && 
            (message.includes('room availability') || 
             message.includes('Room availability') || 
             message.includes('rooms are now available'))) {
            console.log('🚫 NUCLEAR BLOCKER: Prevented alert:', message);
            return;
        }
        return _originalAlert.apply(this, arguments);
    };
    
    // Override dispatchEvent to filter out room reset events
    EventTarget.prototype.dispatchEvent = function(event) {
        if (event && event.type === 'allRoomStatusesReset') {
            console.log('🚫 NUCLEAR BLOCKER: Prevented event dispatch:', event.type);
            return true; // Pretend the event was successfully dispatched
        }
        return _originalDispatchEvent.apply(this, arguments);
    };
    
    // Override addEventListener to intercept room reset event listeners
    EventTarget.prototype.addEventListener = function(type, listener, options) {
        if (type === 'allRoomStatusesReset') {
            console.log('🚫 NUCLEAR BLOCKER: Prevented event listener:', type);
            // Replace the listener with a empty function
            listener = function() { 
                console.log('🚫 NUCLEAR BLOCKER: Dummy listener called');
            };
        }
        return _originalAddEventListener.call(this, type, listener, options);
    };
    
    // Also try to monkey-patch any native alert dialog implementations
    if (window.HTMLDialogElement) {
        var _originalShowModal = HTMLDialogElement.prototype.showModal;
        HTMLDialogElement.prototype.showModal = function() {
            if (this.textContent && 
                (this.textContent.includes('room availability') || 
                 this.textContent.includes('Room availability') || 
                 this.textContent.includes('rooms are now available'))) {
                console.log('🚫 NUCLEAR BLOCKER: Prevented dialog:', this.textContent);
                return;
            }
            return _originalShowModal.apply(this, arguments);
        };
    }
    
    console.log('NUCLEAR ALERT BLOCKER INSTALLED');
})();

// Booking System JavaScript - Complete Working Version
let currentStep = 1;
let selectedPaymentMethod = null;
let currentDate = new Date();
let checkInDate = null;
let checkOutDate = null;
let currentMainImage = '';
let selectedRoomNumber = null;
let selectedBookingType = '';

// Add new variables for checkout calendar display only
let checkinDate = new Date();
let checkoutDate = new Date();
checkoutDate.setDate(checkoutDate.getDate() + 1);

// Flag to prevent alerts during silent resets
window.isSilentReset = false;

// AGGRESSIVE APPROACH: Completely override window.alert to block all room availability messages
// Store the original alert function
const originalAlert = window.alert;
// Override with our filtered version
window.alert = function(message) {
    // Skip alerts about room availability
    if (message && typeof message === 'string' && 
        (message.includes('room availability') || 
         message.includes('Room availability') || 
         message.includes('rooms are now available'))) {
        console.log('BLOCKED ALERT:', message);
        return; // Don't show the alert
    }
    // Allow other alerts to pass through
    return originalAlert.apply(window, arguments);
};

// Room data map
const roomDataMap = {
    'standard': {
        name: 'Standard Room',
        price: 1806,
        description: 'Comfortable and cozy standard room with essential amenities. Perfect for solo travelers or couples on a budget.',
        mainImage: '/images/bookingpic-standard.png'
    },
    'deluxe': {
        name: 'Deluxe Room',
        price: 3588,
        description: 'Spacious deluxe room with premium amenities and a city view. Ideal for couples or small families seeking comfort and style.',
        mainImage: '/images/bookingpic-deluxe.png'
    },
    'suite': {
        name: 'Suite Room',
        price: 15000,
        description: 'Luxurious suite with separate living area, premium amenities, and panoramic views. Perfect for those seeking the ultimate comfort and luxury experience.',
        mainImage: '/images/bookingpic-suite.png'
    }
};

document.addEventListener('DOMContentLoaded', function() {
    console.log("DOM loaded, initializing booking system...");
    
    // SUPER AGGRESSIVE APPROACH: Detect and remove alert dialogs through DOM manipulation
    // Create a MutationObserver to watch for newly added alert dialog nodes
    const observer = new MutationObserver(function(mutations) {
        // Skip observer processing if we're in silent mode
        if (window.isSilentReset === true || window.isTransitioning === true) {
            return;
        }
        
        let alertsFound = 0;
        
        mutations.forEach(function(mutation) {
            if (mutation.addedNodes && alertsFound < 5) { // Limit processing to prevent performance issues
                mutation.addedNodes.forEach(function(node) {
                    // Check if it's a dialog element or something that might contain text
                    if (node.nodeType === 1) { // Element node
                        // If it's an alert or dialog
                        if (node.tagName === 'DIALOG' || 
                            (node.className && typeof node.className === 'string' && 
                             (node.className.includes('alert') || node.className.includes('dialog')))) {
                            // Check if it contains text about room availability
                            const text = node.textContent || node.innerText;
                            if (text && (text.includes('room availability') || 
                                         text.includes('Room availability') || 
                                         text.includes('rooms are now available'))) {
                                console.log('Auto-dismissed dialog:', text);
                                // Try several methods to dismiss it
                                if (node.close) node.close();
                                if (node.dismiss) node.dismiss();
                                if (node.remove) node.remove();
                                alertsFound++;
                            }
                        }
                        
                        // Look for buttons that might dismiss alerts
                        if (alertsFound < 5) { // Limit button scanning too
                            const buttons = node.querySelectorAll('button');
                            buttons.forEach(button => {
                                if (button.textContent === 'OK' || 
                                    button.textContent === 'Close' || 
                                    button.textContent === 'Dismiss') {
                                    console.log('Auto-clicking dismiss button');
                                    button.click();
                                    alertsFound++;
                                }
                            });
                        }
                    }
                });
            }
        });
    });
    
    // Store observer globally so we can disconnect/reconnect it
    window.formObserver = observer;
    
    // Start observing the document with the configured parameters
    observer.observe(document.body, { childList: true, subtree: true });
    
    // Replace continuous interval with a more efficient approach
    // Set up an interval to auto-dismiss any browser alerts, but make it less frequent
    const autoClickInterval = setInterval(() => {
        // Skip if in silent mode
        if (window.isSilentReset === true || window.isTransitioning === true) {
            return;
        }
        
        // Try to find any alert dialog button and click it
        const alertButtons = document.querySelectorAll('button[type="button"], input[type="button"]');
        let clicked = false;
        
        for (const button of alertButtons) {
            if (button.textContent === 'OK' || 
                button.value === 'OK' || 
                button.textContent === 'Close' || 
                button.value === 'Close') {
                console.log('Auto-clicking alert button:', button);
                button.click();
                clicked = true;
                break;
            }
        }
        
        // If we didn't find any buttons to click, we can slow down the interval
        if (!clicked && window.alertCheckSlowdown !== true) {
            clearInterval(autoClickInterval);
            window.alertCheckSlowdown = true;
            // Set a new interval that checks less frequently
            setInterval(() => {
                if (window.isSilentReset === true || window.isTransitioning === true) {
                    return;
                }
                
                const buttons = document.querySelectorAll('button, input[type="button"]');
                for (const button of buttons) {
                    if (button.textContent === 'OK' || 
                        button.value === 'OK' || 
                        button.textContent === 'Close' || 
                        button.value === 'Close') {
                        button.click();
                        break;
                    }
                }
            }, 2000); // Check every 2 seconds instead
        }
    }, 500); // Check every 500ms initially
    
    // Initialize global state flags
    window.isSilentReset = false;
    window.isTransitioning = false;
    window.isResettingRooms = false;
    
    // Reset all room statuses to ensure synchronization with database
    if (window.RoomStatusManager) {
        window.RoomStatusManager.resetAllRoomStatuses('initialization');
        
        // Explicitly ensure rooms 301, 302, and 303 are available
        window.RoomStatusManager.updateRoomStatus('301', window.RoomStatus.AVAILABLE);
        window.RoomStatusManager.updateRoomStatus('302', window.RoomStatus.AVAILABLE);
        window.RoomStatusManager.updateRoomStatus('303', window.RoomStatus.AVAILABLE);
        
        console.log('Room statuses reset and critical rooms set to available');
    }
    
    // Initialize room display and selection
    updateRoomDisplay('standard');
    updateRoomSelection('standard');
    
    // Initialize stepper
    updateStepper(1);
    
    // Initialize both calendars
    generateCalendar(checkinDate.getFullYear(), checkinDate.getMonth(), 'checkin');
    generateCalendar(checkoutDate.getFullYear(), checkoutDate.getMonth(), 'checkout');
    highlightDates();
    
    // Setup payment methods
    setupPaymentMethods();
    
    // Setup terms agreement
    const termsCheckbox = document.getElementById('v5-terms-agreement');
    const acceptButton = document.getElementById('v5-terms-accept-btn');
    if (termsCheckbox && acceptButton) {
        termsCheckbox.addEventListener('change', function() {
            acceptButton.disabled = !this.checked;
        });
        
        acceptButton.addEventListener('click', function() {
            if (termsCheckbox.checked) {
                finalizeBooking();
            } else {
                alert('Please agree to the terms and conditions');
            }
        });
    }
    
    // Setup payment modals
    setupPaymentModals();
    
    // Listen for room status changes from other pages (like RoomManagement.cshtml)
    window.addEventListener('roomStatusChanged', function(event) {
        console.log('Room status change detected:', event.detail);
        
        // Skip if we're in a transition
        if (window.isTransitioning) return;
        
        // If we're on the room selection step, update the room selection
        if (currentStep === 2 || currentStep === 4) {
            // Get the current selected room type from the dropdown
            const roomTypeSelect = document.querySelector('.v5-booking-select');
            const currentRoomType = roomTypeSelect ? roomTypeSelect.value : 'standard';
            
            // Update the room selection to reflect the new status
            updateRoomSelection(currentRoomType);
            
            // If the user had selected a room that is now unavailable, clear the selection
            if (selectedRoomNumber === event.detail.roomNumber && event.detail.newStatus !== window.RoomStatus.AVAILABLE) {
                selectedRoomNumber = null;
                alert(`Room ${event.detail.roomNumber} is no longer available and has been unselected.`);
            }
        }
    });
    
    console.log("Booking system initialized successfully");
});

function updateRoomDisplay(roomType) {
    console.log("Updating room display for:", roomType);
    
    const room = roomDataMap[roomType];
    if (!room) {
        console.error("Room data not found for:", roomType);
        return;
    }
    
    const roomDisplay = document.getElementById('v5-room-display');
    if (!roomDisplay) {
        console.error("Room display element not found");
        return;
    }
    
    // Set the current main image
    currentMainImage = room.mainImage;
    
    roomDisplay.innerHTML = `
        <div class="v5-booking-room-image">
            <img src="${room.mainImage}" alt="${room.name}" id="v5-booking-room-image">
        </div>
        <div class="v5-booking-room-details">
            <h3 class="v5-booking-room-title">${room.name}</h3>
            <p class="v5-booking-room-price"><strong>₱${room.price.toLocaleString()} per night</strong></p>
            <div class="v5-booking-stars">★★★★★</div>
            <hr class="v5-booking-divider">
            <p class="v5-booking-room-description">${room.description}</p>
        </div>
    `;

    // Update confirmation page thumbnails if we're on step 6
    if (currentStep === 6) {
        updateConfirmationThumbnails(roomType);
    }
    
    console.log("Room display updated successfully");
}

function updateConfirmationThumbnails(roomType) {
    const room = roomDataMap[roomType];
    if (!room) return;

    const thumbnailsContainer = document.querySelector('.v5-room-thumbnails');
    if (!thumbnailsContainer) return;

    // Set main image
    const mainImage = document.getElementById('v5-confirm-main-image');
    if (mainImage) {
        mainImage.src = room.mainImage;
    }

    // Only show the main image as a thumbnail
    thumbnailsContainer.innerHTML = `
        <img src="${room.mainImage}" alt="${room.name} Main View" class="main-view active">
    `;
}

function switchConfirmationImage(src) {
    const mainImage = document.getElementById('v5-confirm-main-image');
    if (mainImage) {
        mainImage.src = src;
    }

    // Update thumbnail active states
    const thumbnails = document.querySelectorAll('.v5-room-thumbnails img');
    thumbnails.forEach(thumb => {
        if (thumb.src === src) {
            thumb.classList.add('active');
        } else {
            thumb.classList.remove('active');
        }
    });
}

async function generateRoomButtons(roomType) {
    const roomButtonsContainer = document.getElementById('v5-room-selection');
    roomButtonsContainer.innerHTML = '<div class="loading-indicator"><i class="fas fa-spinner fa-spin"></i> Loading available rooms...</div>';
    
    try {
        // Fetch the latest room statuses directly from the API instead of using local data
        const response = await fetch('/api/Room/ByType/' + roomType);
        
        if (!response.ok) {
            throw new Error('Failed to fetch room data from API');
        }
        
        const result = await response.json();
        
        if (!result.success) {
            throw new Error(result.message || 'Error loading room data');
        }
        
        const rooms = result.data;
        roomButtonsContainer.innerHTML = '';
        
        // Sort rooms by room number
        rooms.sort((a, b) => a.roomNumber.localeCompare(b.roomNumber));
        
        // Define status styles - match colors from RoomManagement.cshtml
        const statusStyles = {
            'available': { class: 'status-available', color: '#28a745', background: 'rgba(40, 167, 69, 0.1)' },
            'occupied': { class: 'status-occupied', color: '#dc3545', background: 'rgba(220, 53, 69, 0.1)' },
            'cleaning': { class: 'status-cleaning', color: '#ffc107', background: 'rgba(255, 193, 7, 0.1)' },
            'maintenance': { class: 'status-maintenance', color: '#17a2b8', background: 'rgba(23, 162, 184, 0.1)' }
        };
        
        // Create a style tag for our status styles if it doesn't exist
        let styleTag = document.getElementById('room-status-styles');
        if (!styleTag) {
            styleTag = document.createElement('style');
            styleTag.id = 'room-status-styles';
            document.head.appendChild(styleTag);
            
            // Add CSS rules for room status indicators
            styleTag.textContent = `
                /* Reset any custom button styling to ensure original height */
                .v5-room-button {
                    min-height: unset;
                    height: auto;
                    display: inline-block;
                    flex-direction: unset;
                    justify-content: unset;
                    align-items: unset;
                    padding: 0.375rem 0.75rem;
                }
                .v5-room-button .room-status {
                    display: block;
                    margin-top: 4px;
                    padding: 2px 6px;
                    border-radius: 4px;
                    font-size: 0.8rem;
                    font-weight: 600;
                }
                .v5-room-button .status-available {
                    background-color: ${statusStyles.available.background};
                    color: ${statusStyles.available.color};
                }
                .v5-room-button .status-occupied {
                    background-color: ${statusStyles.occupied.background};
                    color: ${statusStyles.occupied.color};
                }
                .v5-room-button .status-cleaning {
                    background-color: ${statusStyles.cleaning.background};
                    color: ${statusStyles.cleaning.color};
                }
                .v5-room-button .status-maintenance {
                    background-color: ${statusStyles.maintenance.background};
                    color: ${statusStyles.maintenance.color};
                }
                .v5-room-button.disabled {
                    opacity: 0.8;
                    cursor: not-allowed;
                    border-color: #ccc;
                }
                /* Also style the room button border based on status */
                .v5-room-button.disabled[data-status="occupied"] {
                    border-left: 4px solid ${statusStyles.occupied.color};
                }
                .v5-room-button.disabled[data-status="cleaning"] {
                    border-left: 4px solid ${statusStyles.cleaning.color};
                }
                .v5-room-button.disabled[data-status="maintenance"] {
                    border-left: 4px solid ${statusStyles.maintenance.color};
                }
            `;
        }
        
        for (const room of rooms) {
            // Check if the room is available from the API data
            const isAvailable = room.status === 'available';
            const status = room.status || 'unavailable';
            
            const roomButton = document.createElement('button');
            roomButton.className = `v5-room-button ${selectedRoomNumber === room.roomNumber ? 'selected' : ''}`;
            roomButton.setAttribute('data-room-number', room.roomNumber);
            roomButton.setAttribute('data-room-type', roomType);
            roomButton.setAttribute('data-status', status);
            roomButton.type = 'button';
            roomButton.textContent = `Room ${room.roomNumber}`;
            
            if (!isAvailable) {
                roomButton.disabled = true;
                roomButton.classList.add('disabled');
                
                // Add status indicator with appropriate styling
                const statusLabel = document.createElement('span');
                statusLabel.className = `room-status ${statusStyles[status].class}`;
                statusLabel.textContent = capitalizeFirstLetter(status);
                roomButton.appendChild(statusLabel);
            } else {
                roomButton.addEventListener('click', () => {
                    selectRoom(room.roomNumber);
                });
            }
            
            roomButtonsContainer.appendChild(roomButton);
        }
        
        // If the previously selected room is now unavailable, clear the selection
        if (selectedRoomNumber) {
            const selectedRoomStillAvailable = rooms.find(r => 
                r.roomNumber === selectedRoomNumber && r.status === 'available');
            
            if (!selectedRoomStillAvailable) {
                selectedRoomNumber = null;
                console.log('Previously selected room is no longer available');
            }
        }
    } catch (error) {
        console.error('Error generating room buttons:', error);
        roomButtonsContainer.innerHTML = `
            <div class="error-message">
                <i class="fas fa-exclamation-circle"></i>
                Error loading rooms. Please try again.
            </div>
        `;
    }
}

// Helper function to capitalize the first letter of a string
function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

// This function performs an ultra-silent, direct room reset without any events
function silentRoomReset() {
    console.log('PERFORMING ULTRA-SILENT ROOM RESET');
    
    // Don't run this function repeatedly if it's already running
    if (window.isResettingRooms) {
        console.log('Room reset already in progress, skipping duplicate call');
        return;
    }
    
    window.isResettingRooms = true;
    
    // Ensure we don't trigger any events or alerts during this process
    window.isSilentReset = true;
    
    // Save original alert function
    const originalAlert = window.alert;
    // Temporarily disable alerts
    window.alert = function() {
        console.log('SILENT RESET: Alert suppressed -', arguments[0]);
    };
    
    try {
        // Performance optimization: Check if we already have room data first
        const existingData = localStorage.getItem('roomData');
        if (existingData) {
            // Modify the existing data instead of recreating it from scratch
            const parsedData = JSON.parse(existingData);
            
            // Only update room statuses to available where needed
            Object.keys(parsedData).forEach(roomType => {
                parsedData[roomType].forEach(room => {
                    // Only change status if needed to reduce unnecessary updates
                    if (room.status !== RoomStatus.AVAILABLE) {
                        room.status = RoomStatus.AVAILABLE;
                        room.statusUpdatedAt = new Date().toISOString();
                        room.updatedBy = 'system';
                    }
                });
            });
            
            // Save back to localStorage
            localStorage.setItem('roomData', JSON.stringify(parsedData));
            console.log('ULTRA-SILENT ROOM RESET COMPLETED (optimized)');
        } else {
            // If no data exists, create it from scratch
            const standard = Array.from({ length: 30 }, (_, i) => ({
                roomNumber: `${i+301}`,
                type: 'standard',
                status: RoomStatus.AVAILABLE,
                statusUpdatedAt: new Date().toISOString(),
                updatedBy: 'system'
            }));
            
            const deluxe = Array.from({ length: 30 }, (_, i) => ({
                roomNumber: `${i+401}`,
                type: 'deluxe',
                status: RoomStatus.AVAILABLE,
                statusUpdatedAt: new Date().toISOString(),
                updatedBy: 'system'
            }));
            
            const suite = Array.from({ length: 30 }, (_, i) => ({
                roomNumber: `${i+501}`,
                type: 'suite',
                status: RoomStatus.AVAILABLE,
                statusUpdatedAt: new Date().toISOString(),
                updatedBy: 'system'
            }));
            
            const roomData = { standard, deluxe, suite };
            localStorage.setItem('roomData', JSON.stringify(roomData));
            console.log('ULTRA-SILENT ROOM RESET COMPLETED (full reset)');
        }
    } catch (error) {
        console.error('Error during ultra-silent room reset:', error);
    } finally {
        // Restore the original alert function
        window.alert = originalAlert;
        
        // Reset the silent flags after a delay
        setTimeout(() => {
            window.isSilentReset = false;
            window.isResettingRooms = false;
            console.log("Room reset completed and flags cleared");
        }, 500);
    }
}

async function updateRoomSelection(roomType) {
    try {
        // Update room selection title
        const roomSelectionTitle = document.getElementById('roomSelectionTitle');
        if (roomSelectionTitle) {
            roomSelectionTitle.textContent = `Select a ${capitalizeFirstLetter(roomType)} Room`;
        }
        
        // Generate room buttons
        await generateRoomButtons(roomType);
        
        // Update room images based on type
        updateRoomDisplay(roomType);
        
        // Update confirmation thumbnails
        updateConfirmationThumbnails(roomType);
    } catch (error) {
        console.error('Error updating room selection:', error);
    }
}

async function changeRoomImage(select) {
    console.log("Changing room image for:", select.value);
    
    // Set global flag to indicate we're in a silent operation
    window.isSilentReset = true;
    
    // NUCLEAR APPROACH: disable ALL alerts while this function runs
    const originalWindowAlert = window.alert;
    window.alert = function(message) { 
        console.log("🛑 ALL ALERTS BLOCKED during room change:", message);
        return;
    };
    
    try {
        const roomType = select.value;
        const roomData = roomDataMap[roomType];
        
        if (!roomData) {
            console.error("Room data not found for:", roomType);
            return;
        }
        
        // Reset selected room number when changing room type
        selectedRoomNumber = null;
        
        // Update room display
        updateRoomDisplay(roomType);
        
        // DIRECT DOM MANIPULATION: Generate room buttons without triggering any events
        const roomSelectionContainer = document.getElementById('v5-room-selection');
        if (roomSelectionContainer) {
            // Ultra-silent room reset that doesn't use any APIs
            silentRoomReset();
            
            // Wait a moment for the reset to complete
            await new Promise(resolve => setTimeout(resolve, 100));
            
            // Create buttons for this specific room type directly
            let roomData = JSON.parse(localStorage.getItem('roomData')) || {};
            let rooms = roomData[roomType] || [];
            
            // Set specific room statuses based on requirements
            if (window.RoomStatusManager) {
                // Set them directly without going through the manager API to avoid events
                if (roomType === 'standard') {
                    if (rooms[0]) rooms[0].status = 'occupied'; // Room 301
                    if (rooms[1]) rooms[1].status = 'maintenance'; // Room 302
                    if (rooms[2]) rooms[2].status = 'cleaning'; // Room 303
                }
                // Save the updated data back to localStorage
                roomData[roomType] = rooms;
                localStorage.setItem('roomData', JSON.stringify(roomData));
            }
            
            // Generate HTML directly
            let buttonsHTML = '';
            rooms.forEach(room => {
                const isAvailable = room.status === 'available';
                let statusLabel = '';
                
                if (room.status === 'occupied') {
                    statusLabel = '<span class="room-status occupied">Occupied</span>';
                } else if (room.status === 'cleaning') {
                    statusLabel = '<span class="room-status cleaning">Cleaning</span>';
                } else if (room.status === 'maintenance') {
                    statusLabel = '<span class="room-status maintenance">Maintenance</span>';
                }
                
                buttonsHTML += `
                    <button type="button" 
                            class="v5-room-button ${selectedRoomNumber === room.roomNumber ? 'selected' : ''}" 
                            data-room="${room.roomNumber}"
                            data-status="${room.status}"
                            ${!isAvailable ? 'disabled' : ''}>
                        Room ${room.roomNumber}
                        ${!isAvailable ? statusLabel : ''}
                    </button>
                `;
            });
            
            // Set the HTML directly without using updateRoomSelection
            roomSelectionContainer.innerHTML = buttonsHTML;
            
            // Add click event listeners to the room buttons
            const roomButtons = roomSelectionContainer.querySelectorAll('.v5-room-button:not([disabled])');
            roomButtons.forEach(button => {
                button.addEventListener('click', () => {
                    const roomNumber = button.getAttribute('data-room');
                    selectRoom(roomNumber);
                });
            });
        }
        
        console.log("Room image and selection changed successfully");
    } catch (error) {
        console.error("Error changing room:", error);
    } finally {
        // Restore the alert function after a delay to ensure any events that might trigger alerts have finished
        setTimeout(() => {
            window.alert = originalWindowAlert;
            window.isSilentReset = false;
            console.log("Alert function restored");
        }, 2000);
    }
}

function selectRoom(roomNumber) {
    console.log("Selecting room:", roomNumber);
    
    // Update selected room number
    selectedRoomNumber = roomNumber;
    
    // Update room buttons
    const roomButtons = document.querySelectorAll('.v5-room-button');
    let selectedRoomType = null;
    
    roomButtons.forEach(button => {
        if (button.getAttribute('data-room-number') === roomNumber) {
            button.classList.add('selected');
            // Get the room type of the selected room
            selectedRoomType = button.getAttribute('data-room-type');
        } else {
            button.classList.remove('selected');
        }
    });
    
    // Update the room type dropdown if a room with different type was selected
    if (selectedRoomType) {
        const roomTypeSelect = document.querySelector('.v5-booking-select');
        if (roomTypeSelect && roomTypeSelect.value !== selectedRoomType) {
            roomTypeSelect.value = selectedRoomType;
            // Update room display with the new type
            updateRoomDisplay(selectedRoomType);
        }
    }
    
    console.log("Room selected successfully:", selectedRoomNumber);
}

function nextForm(formNumber) {
    // Prevent multiple rapid calls to this function
    if (window.isTransitioning) {
        console.log("Already transitioning, please wait...");
        return;
    }
    
    window.isTransitioning = true;
    window.scrollTo({ top: 0, behavior: 'smooth' });
    
    if (!validateCurrentForm(currentStep)) {
        window.isTransitioning = false;
        return;
    }
    
    // Performance optimization: Disable MutationObserver during transitions
    if (window.formObserver) {
        window.formObserver.disconnect();
        console.log("Temporarily disabled MutationObserver for performance");
    }
    
    // Capture booking type when moving from step 1 to 2
    if (currentStep === 1) {
        const button = event.target;
        selectedBookingType = button.textContent.trim();
        
        // Set date restrictions based on booking type
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        if (selectedBookingType === 'Immediate Booking') {
            // For immediate booking, allow check-in within the next 24 hours
            const tomorrow = new Date(today);
            tomorrow.setDate(tomorrow.getDate() + 1);
            
            // Store the date restrictions
            window.minCheckinDate = today;
            window.maxCheckinDate = tomorrow;
            window.minCheckoutDate = tomorrow;
            window.maxCheckoutDate = new Date(tomorrow.getTime() + (30 * 24 * 60 * 60 * 1000)); // +30 days
        } else if (selectedBookingType === 'Advanced Booking') {
            // For advanced booking, allow check-in from 2 days up to 90 days in advance
            const dayAfterTomorrow = new Date(today);
            dayAfterTomorrow.setDate(dayAfterTomorrow.getDate() + 2);
            
            const ninetyDaysFromNow = new Date(today);
            ninetyDaysFromNow.setDate(ninetyDaysFromNow.getDate() + 90);
            
            // Store the date restrictions
            window.minCheckinDate = dayAfterTomorrow;
            window.maxCheckinDate = ninetyDaysFromNow;
            window.minCheckoutDate = dayAfterTomorrow;
            window.maxCheckoutDate = new Date(ninetyDaysFromNow.getTime() + (30 * 24 * 60 * 60 * 1000));
        }

        // Clear any previously selected dates
        checkinDate = null;
        checkoutDate = null;
        document.getElementById('v5-check-in-date').textContent = 'Not selected';
        document.getElementById('v5-check-out-date').textContent = 'Not selected';
        
        // Regenerate calendars with new restrictions
        generateCalendar(new Date().getFullYear(), new Date().getMonth(), 'checkin');
        generateCalendar(new Date().getFullYear(), new Date().getMonth(), 'checkout');
    }
    
    // Set a flag to prevent alert dialogs during transition
    window.isSilentReset = true;
    
    // Update current step immediately to reflect where we're going
    currentStep = formNumber;
    
    // Hide all forms first
    document.querySelectorAll('.v5-booking-form').forEach(form => {
        form.classList.remove('active');
    });
    
    // Performance improvement: Delay showing the new form slightly
    setTimeout(() => {
        const selectedForm = document.getElementById('v5-form' + formNumber);
        if (selectedForm) {
            selectedForm.classList.add('active');
            
            if (formNumber === 2) {
                // Performance optimization: Use a timeout to delay room selection initialization
                console.log("Moving to step 2, initializing room selection after delay...");
                setTimeout(() => {
                    try {
                        // Update room display with minimal processing
                        const roomTypeSelect = document.querySelector('.v5-booking-select');
                        if (roomTypeSelect) {
                            const roomType = roomTypeSelect.value || 'standard';
                            updateRoomDisplay(roomType);
                            
                            // Use a separate timeout for the more intensive room selection update
                            setTimeout(() => {
                                updateRoomSelection(roomType);
                                console.log("Room selection completed");
                                
                                // Allow time to render before re-enabling observer
                                setTimeout(() => {
                                    // Re-enable observer
                                    if (window.formObserver) {
                                        window.formObserver.observe(document.body, { childList: true, subtree: true });
                                        console.log("Re-enabled MutationObserver");
                                    }
                                    window.isSilentReset = false;
                                    window.isTransitioning = false;
                                }, 500);
                            }, 200);
                        } else {
                            window.isSilentReset = false;
                            window.isTransitioning = false;
                        }
                    } catch (error) {
                        console.error("Error during room selection initialization:", error);
                        window.isSilentReset = false;
                        window.isTransitioning = false;
                    }
                }, 100);
            } else if (formNumber === 3) {
                // Initialize calendars when moving to step 3
                console.log("Moving to step 3, initializing calendars...");
                setTimeout(() => {
                    try {
                        // Ensure we have valid date objects for the calendars, but don't display them yet
                        if (!checkinDate || isNaN(checkinDate.getTime())) {
                            checkinDate = new Date();
                            console.log("Created new check-in date (internal only)");
                        }
                        
                        if (!checkoutDate || isNaN(checkoutDate.getTime())) {
                            checkoutDate = new Date();
                            checkoutDate.setDate(checkinDate.getDate() + 1);
                            console.log("Created new check-out date (internal only)");
                        }
                        
                        // Check if dates were previously selected (don't auto-populate)
                        const checkinDisplay = document.getElementById('v5-check-in-date');
                        const checkoutDisplay = document.getElementById('v5-check-out-date');
                        
                        // Reset text displays to "Not selected" if they haven't been explicitly chosen
                        if (!checkInDate || isNaN(checkInDate.getTime())) {
                            if (checkinDisplay) checkinDisplay.textContent = 'Not selected';
                        } else {
                            // Only show if explicitly selected
                            if (checkinDisplay) checkinDisplay.textContent = checkInDate.toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' });
                        }
                        
                        if (!checkOutDate || isNaN(checkOutDate.getTime())) {
                            if (checkoutDisplay) checkoutDisplay.textContent = 'Not selected';
                        } else {
                            // Only show if explicitly selected
                            if (checkoutDisplay) checkoutDisplay.textContent = checkOutDate.toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' });
                        }
                        
                        // Generate the calendars
                        generateCalendar(checkinDate.getFullYear(), checkinDate.getMonth(), 'checkin');
                        generateCalendar(checkoutDate.getFullYear(), checkoutDate.getMonth(), 'checkout');
                        highlightDates();
                        
                        // Re-enable observer and clear flags
                        if (window.formObserver) {
                            window.formObserver.observe(document.body, { childList: true, subtree: true });
                        }
                        window.isSilentReset = false;
                        window.isTransitioning = false;
                    } catch (error) {
                        console.error("Error initializing calendars:", error);
                        window.isSilentReset = false;
                        window.isTransitioning = false;
                    }
                }, 100);
            } else if (formNumber === 6) {
                // Generate confirmation summary when moving to step 6
                console.log("Moving to step 6, generating confirmation summary...");
                setTimeout(() => {
                    generateConfirmationSummary();
                    
                    // Re-enable observer and clear flags
                    if (window.formObserver) {
                        window.formObserver.observe(document.body, { childList: true, subtree: true });
                    }
                    window.isSilentReset = false;
                    window.isTransitioning = false;
                }, 100);
            } else {
                // For other steps, just re-enable observer and clear flags
                if (window.formObserver) {
                    window.formObserver.observe(document.body, { childList: true, subtree: true });
                }
                window.isSilentReset = false;
                window.isTransitioning = false;
            }
        } else {
            // If form not found, clear flags
            window.isSilentReset = false;
            window.isTransitioning = false;
        }
        
        updateStepper(formNumber);
    }, 50);
    
    // When moving to step 4 (room selection), sync with API
    if (formNumber === 4) {
        // Force refresh room data when entering the room selection step
        setTimeout(() => {
            try {
                // Get the current selected room type
                const roomTypeSelect = document.querySelector('.v5-booking-select');
                if (roomTypeSelect) {
                    const roomType = roomTypeSelect.value || 'standard';
                    // Update the room selection with fresh data
                    updateRoomSelection(roomType);
                }
            } catch (error) {
                console.error("Error refreshing room data:", error);
            }
        }, 100);
    }
}

function prevForm(formNumber) {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    
    currentStep = formNumber;
    document.querySelectorAll('.v5-booking-form').forEach(form => {
        form.classList.remove('active');
    });
    
    const selectedForm = document.getElementById('v5-form' + formNumber);
    if (selectedForm) {
        selectedForm.classList.add('active');
        if (formNumber === 6) {
            console.log("Returning to step 6, regenerating confirmation summary...");
            // Add a small delay to ensure the DOM is updated before generating the summary
            setTimeout(() => {
                generateConfirmationSummary();
            }, 100);
        }
    }
    
    updateStepper(formNumber);
}

function validateName(name) {
    // Name should be at least 2 words, each at least 2 characters long
    // Only letters, spaces, and hyphens allowed
    const namePattern = /^[A-Za-z]+([-\s][A-Za-z]+)+$/;
    return namePattern.test(name) && name.length >= 5;
}

function validatePhoneNumber(phone) {
    // Philippine phone number format: +63 or 0 followed by 10 digits
    const phonePattern = /^(\+63|0)[0-9]{10}$/;
    return phonePattern.test(phone.replace(/\s/g, '')); // Remove spaces before testing
}

function validateEmail(email) {
    // Basic email validation
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailPattern.test(email);
}

function validateCurrentForm(step) {
    console.log("Validating form for step:", step);
    
    // No validation needed for step 1 (booking type)
    if (step === 1) return true;
    
    // Validate room selection for step 2
    if (step === 2) {
        const roomSelect = document.querySelector('.v5-booking-select');
        if (!roomSelect) {
            console.error("Room select element not found!");
            return false;
        }
        
        const roomType = roomSelect.value;
        if (!roomType) {
            alert("Please select a room type.");
            return false;
        }
        
        // Room selection is now done at step 4, so we don't need to validate selectedRoomNumber here
        return true;
    }
    
    // Validate dates for step 3
    if (step === 3) {
        try {
            const checkinDateText = document.getElementById('v5-check-in-date').textContent;
            const checkoutDateText = document.getElementById('v5-check-out-date').textContent;
            
            // Check if dates are selected
            if (checkinDateText === 'Not selected' || checkoutDateText === 'Not selected') {
                // Don't auto-populate dates if they haven't been explicitly selected
                alert("Please select both check-in and check-out dates.");
                return false;
            }

            // Parse the dates using the displayed format
            let checkinDateObj, checkoutDateObj;
            
            try {
                checkinDateObj = new Date(checkinDateText);
                checkoutDateObj = new Date(checkoutDateText);
                
                // Verify the dates are valid
                if (isNaN(checkinDateObj.getTime()) || isNaN(checkoutDateObj.getTime())) {
                    throw new Error("Invalid date format");
                }
                
                // Reset time parts to midnight for accurate date comparison
                checkinDateObj.setHours(0, 0, 0, 0);
                checkoutDateObj.setHours(0, 0, 0, 0);
            } catch (e) {
                console.error("Error parsing dates:", e);
                alert("There was an error with the selected dates. Please select your dates again.");
                return false;
            }

            const today = new Date();
            today.setHours(0, 0, 0, 0);

            if (checkinDateObj < today) {
                alert("Check-in date cannot be in the past.");
                return false;
            }

            if (checkoutDateObj <= checkinDateObj) {
                alert("Check-out date must be after check-in date.");
                return false;
            }
            
            // Update the global date variables to ensure consistency
            checkInDate = checkinDateObj;
            checkOutDate = checkoutDateObj;
                
            return true;
        } catch (error) {
            console.error("Error validating dates:", error);
            alert("There was an error validating your dates. Please try again.");
            return false;
        }
    }
    
    // Validate guest information and room selection for step 4
    if (step === 4) {
        const guestName = document.querySelector('.v5-input-field[placeholder="John Dela Cruz"]').value;
        const guestContact = document.querySelector('.v5-input-field[placeholder="0912 345 6789"]').value;
        const guestEmail = document.querySelector('.v5-input-field[placeholder="your@email.com"]').value;
        
        if (!validateName(guestName)) {
            alert("Please enter a valid full name (First and Last name, letters only).");
            return false;
        }

        if (!validatePhoneNumber(guestContact)) {
            alert("Please enter a valid Philippine phone number (e.g., +639123456789 or 09123456789).");
            return false;
        }
        
        if (!validateEmail(guestEmail)) {
            alert("Please enter a valid email address.");
            return false;
        }

        if (!selectedRoomNumber) {
            alert("Please select a room number.");
            return false;
        }

        return true;
    }
    
    // Validate payment method for step 5
    if (step === 5) {
        const selectedPayment = document.querySelector('.v5-booking-payment-card.selected');
        
        if (!selectedPayment) {
            alert("Please select a payment method.");
            return false;
        }
        
        return true;
    }
    
    // No validation needed for step 6 (confirmation)
    if (step === 6) return true;
    
    // No validation needed for step 7 (terms)
    if (step === 7) return true;
    
    return true;
}

function updateStepper(currentStep) {
    console.log("Updating stepper to step:", currentStep);
    
    const steps = document.querySelectorAll('.v5-booking-step');
    
    steps.forEach((step, index) => {
        const stepNumber = index + 1;
        
        if (stepNumber < currentStep) {
            // Completed steps
            step.classList.add('completed');
            step.classList.remove('active');
        } else if (stepNumber === currentStep) {
            // Current step
            step.classList.add('active');
            step.classList.remove('completed');
        } else {
            // Future steps
            step.classList.remove('active', 'completed');
        }
    });
}

function getIconClass(stepNumber) {
    const icons = [
        'fas fa-calendar-alt',
        'fas fa-bed',
        'fas fa-calendar-check',
        'fas fa-user',
        'fas fa-credit-card',
        'fas fa-clipboard-check',
        'fas fa-file-alt'
    ];
    return icons[stepNumber - 1] || 'fas fa-question-circle';
}

function generateCalendar(year, month, calendarType) {
    console.log(`Generating ${calendarType} calendar for:`, year, month);
    
    const calendarDates = document.getElementById(`${calendarType}-dates`);
    if (!calendarDates) {
        console.error(`${calendarType} calendar dates container not found`);
        return;
    }
    
    // Clear existing dates
    calendarDates.innerHTML = '';
    
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const daysInMonth = lastDay.getDate();
    const startingDay = firstDay.getDay();
    
    // Update month and year display
    const monthYearDisplay = document.getElementById(`${calendarType}-month-year`);
    if (monthYearDisplay) {
        const monthName = firstDay.toLocaleString('default', { month: 'long' });
        monthYearDisplay.textContent = `${monthName.toUpperCase()} ${year}`;
    }
    
    // Add empty cells for days before the first day of the month
    for (let i = 0; i < startingDay; i++) {
        const emptyCell = document.createElement('div');
        emptyCell.classList.add('disabled');
        calendarDates.appendChild(emptyCell);
    }
    
    // Add calendar dates
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    for (let date = 1; date <= daysInMonth; date++) {
        const dateCell = document.createElement('div');
        dateCell.textContent = date;
        
        const currentDate = new Date(year, month, date);
        currentDate.setHours(0, 0, 0, 0);
        
        if (currentDate < today) {
            dateCell.classList.add('disabled');
        } else {
            dateCell.addEventListener('click', () => selectDate(currentDate, calendarType));
            
            // Check if date is selected or in range
            if (checkInDate && currentDate.getTime() === checkInDate.getTime()) {
                dateCell.classList.add('selected');
            } else if (checkOutDate && currentDate.getTime() === checkOutDate.getTime()) {
                dateCell.classList.add('selected');
            } else if (checkInDate && checkOutDate && 
                      currentDate > checkInDate && currentDate < checkOutDate) {
                dateCell.classList.add('in-range');
            }
        }
        
        calendarDates.appendChild(dateCell);
    }
    
    console.log(`${calendarType} calendar generated successfully`);
}

function selectDate(selectedDate, calendarType) {
    console.log(`Selecting date for ${calendarType}:`, selectedDate);
    
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    const selected = new Date(selectedDate);
    selected.setHours(0, 0, 0, 0);
    
    // Initialize min/max dates if not set
    if (!window.minCheckinDate) {
        window.minCheckinDate = today;
    }
    
    if (!window.maxCheckinDate) {
        const maxDate = new Date(today);
        maxDate.setDate(maxDate.getDate() + 90); // Default to 90 days in advance
        window.maxCheckinDate = maxDate;
    }
    
    if (calendarType === 'checkin') {
        if (window.minCheckinDate && window.maxCheckinDate &&
            (selected < window.minCheckinDate || selected > window.maxCheckinDate)) {
            if (selectedBookingType === 'Immediate Booking') {
                alert('For immediate booking, check-in must be within 24 hours from now.');
            } else {
                alert('For advanced booking, check-in must be between 2 and 90 days in advance.');
            }
            return;
        }
        
        // Explicitly set the checkInDate variable when user selects a date
        checkInDate = selected;
        
        // Update minimum checkout date to be the day after selected check-in
        window.minCheckoutDate = new Date(selected);
        window.minCheckoutDate.setDate(window.minCheckoutDate.getDate() + 1);
        
        // If existing checkout date is before new minimum, suggest a new one but don't auto-select
        if (checkOutDate && checkOutDate <= checkInDate) {
            checkOutDate = null;
            document.getElementById('v5-check-out-date').textContent = 'Not selected';
            alert("Your check-out date must be after your new check-in date. Please select a new check-out date.");
        }
    } else {
        if (!checkInDate) {
            alert('Please select a check-in date first.');
            return;
        }
        
        if (selected <= checkInDate) {
            alert('Check-out date must be after check-in date.');
            return;
        }
        
        const maxStay = new Date(checkInDate);
        maxStay.setDate(maxStay.getDate() + 30); // Maximum 30-day stay
        
        if (selected > maxStay) {
            alert('Maximum stay duration is 30 days.');
            return;
        }
        
        // Explicitly set the checkOutDate variable when user selects a date
        checkOutDate = selected;
    }
    
    // Update the display when explicitly selected by user
    if (calendarType === 'checkin' && checkInDate) {
        document.getElementById('v5-check-in-date').textContent = 
            checkInDate.toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' });
    } else if (calendarType === 'checkout' && checkOutDate) {
        document.getElementById('v5-check-out-date').textContent = 
            checkOutDate.toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' });
    }
    
    // Regenerate both calendars to show updated highlighting
    generateCalendar(checkinDate.getFullYear(), checkinDate.getMonth(), 'checkin');
    generateCalendar(checkoutDate.getFullYear(), checkoutDate.getMonth(), 'checkout');
    
    // Highlight the selected dates
    highlightDates();
}

function formatDate(dateString) {
    console.log("Formatting date:", dateString);
    
    if (!dateString) {
        console.error("Date string is missing!");
        return '';
    }
    
    const date = new Date(dateString);
    
    // Validate date
    if (isNaN(date.getTime())) {
        console.error("Invalid date format!");
        return '';
    }
    
    // Format date as "Month Day, Year"
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    const formattedDate = date.toLocaleDateString('en-US', options);
    console.log("Formatted date:", formattedDate);
    
    return formattedDate;
}

function highlightDates() {
    console.log('Highlighting dates...');
    
    // Get all date cells from both calendars
    const checkinDates = document.querySelectorAll('#checkin-dates div:not(.disabled)');
    const checkoutDates = document.querySelectorAll('#checkout-dates div:not(.disabled)');
    
    // Remove existing highlights
    checkinDates.forEach(cell => {
        cell.classList.remove('selected', 'in-range');
    });
    checkoutDates.forEach(cell => {
        cell.classList.remove('selected', 'in-range');
    });
    
    // Add new highlights
    if (checkInDate || checkOutDate) {
        const highlightCell = (cell, date) => {
            const cellDate = new Date(date.getFullYear(), date.getMonth(), parseInt(cell.textContent));
            cellDate.setHours(0, 0, 0, 0);
            
            if (checkInDate && cellDate.getTime() === checkInDate.getTime()) {
                cell.classList.add('selected');
            } else if (checkOutDate && cellDate.getTime() === checkOutDate.getTime()) {
                cell.classList.add('selected');
            } else if (checkInDate && checkOutDate && 
                      cellDate > checkInDate && cellDate < checkOutDate) {
                cell.classList.add('in-range');
            }
        };
        
        const currentDate = new Date();
        checkinDates.forEach(cell => highlightCell(cell, currentDate));
        checkoutDates.forEach(cell => highlightCell(cell, currentDate));
    }
}

function clearDates() {
    // Set default dates (today and tomorrow)
    checkinDate = new Date();
    checkoutDate = new Date();
    checkoutDate.setDate(checkinDate.getDate() + 1);
    
    // Reset the global variables for validation
    checkInDate = null;
    checkOutDate = null;
    
    // Reset displayed text
    document.getElementById('v5-check-in-date').textContent = 'Not selected';
    document.getElementById('v5-check-out-date').textContent = 'Not selected';
    
    // Regenerate calendars to clear highlighting
    generateCalendar(checkinDate.getFullYear(), checkinDate.getMonth(), 'checkin');
    generateCalendar(checkoutDate.getFullYear(), checkoutDate.getMonth(), 'checkout');
    
    console.log("Dates cleared and calendars reset");
}

function changeMonth(offset, calendarType) {
    if (calendarType === 'checkin') {
        checkinDate.setMonth(checkinDate.getMonth() + offset);
        generateCalendar(checkinDate.getFullYear(), checkinDate.getMonth(), 'checkin');
    } else {
        checkoutDate.setMonth(checkoutDate.getMonth() + offset);
        generateCalendar(checkoutDate.getFullYear(), checkoutDate.getMonth(), 'checkout');
    }
    highlightDates();
}

function setupPaymentMethods() {
    console.log("Setting up payment methods...");
    
    const paymentMethods = document.querySelectorAll('.v5-booking-payment-card');
    
    if (!paymentMethods || paymentMethods.length === 0) {
        console.error("Payment method elements not found");
        return;
    }
    
    paymentMethods.forEach(method => {
        method.addEventListener('click', () => {
            console.log("Payment method clicked:", method.getAttribute('data-payment'));
            
            // Remove selected class from all payment methods
            paymentMethods.forEach(m => m.classList.remove('selected'));
            
            // Add selected class to the clicked payment method
            method.classList.add('selected');
            
            // Store the selected payment method
            selectedPaymentMethod = method.getAttribute('data-payment');
            
            // Show corresponding modal based on payment method
            const paymentType = method.getAttribute('data-payment');
            let paymentName = '';
            let modalId = '';
            
            switch (paymentType) {
                case 'visa':
                    paymentName = 'VISA';
                    modalId = 'v5-visa-modal';
                    break;
                case 'mastercard':
                    paymentName = 'MASTERCARD';
                    modalId = 'v5-mastercard-modal';
                    break;
                case 'gcash':
                    paymentName = 'GCASH';
                    modalId = 'v5-gcash-modal';
                    break;
                case 'maya':
                    paymentName = 'MAYA';
                    modalId = 'v5-maya-modal';
                    break;
                case 'pay-at-hotel':
                    paymentName = 'Pay at Hotel';
                    nextForm(6); // Skip payment modal for pay at hotel
                    return;
            }
            
            // Show the modal if it's not pay-at-hotel
            if (modalId) {
                const modal = document.getElementById(modalId);
                if (modal) {
                    modal.style.display = 'flex';
                }
            }
            
            // If we're already on step 6, update the confirmation summary
            if (currentStep === 6) {
                const paymentElement = document.getElementById('v5-confirm-payment');
                if (paymentElement) {
                    paymentElement.textContent = paymentName;
                    console.log("Updated payment method in confirmation summary:", paymentName);
                }
            }
            
            console.log("Payment method selected:", paymentName);
        });
    });
    
    console.log("Payment methods setup completed");
}

function setupPaymentModals() {
    console.log("Setting up payment modals...");
    
    // Get all payment modals
    const modals = document.querySelectorAll('.v5-booking-modal');
    const closeButtons = document.querySelectorAll('.v5-modal-close');
    
    // Add click event to close buttons
    closeButtons.forEach(button => {
        button.addEventListener('click', () => {
            const modal = button.closest('.v5-booking-modal');
            if (modal) {
                modal.style.display = 'none';
            }
        });
    });
    
    // Close modal when clicking outside of it
    window.addEventListener('click', (event) => {
        modals.forEach(modal => {
            if (event.target === modal) {
                modal.style.display = 'none';
            }
        });
    });

    // Function to show success message and proceed
    function showPaymentSuccess(paymentType) {
        const successMessage = `Payment successfully processed via ${paymentType}!`;
        alert(successMessage);
        document.getElementById(`v5-${paymentType.toLowerCase()}-modal`).style.display = 'none';
        nextForm(6);
    }
    
    // Setup form submissions with validations
    const visaForm = document.getElementById('v5-visa-form');
    const mayaForm = document.getElementById('v5-maya-form');
    const gcashForm = document.getElementById('v5-gcash-form');
    const mastercardForm = document.getElementById('v5-mastercard-form');
    
    // Card number validation
    function validateCardNumber(number) {
        return /^[0-9]{16}$/.test(number.replace(/\s/g, ''));
    }
    
    // Expiry date validation (MM/YY format)
    function validateExpiryDate(expiry) {
        if (!/^(0[1-9]|1[0-2])\/([0-9]{2})$/.test(expiry)) return false;
        
        const [month, year] = expiry.split('/');
        const expDate = new Date(2000 + parseInt(year), parseInt(month) - 1);
        const today = new Date();
        
        return expDate > today;
    }
    
    // CVV validation
    function validateCVV(cvv) {
        return /^[0-9]{3,4}$/.test(cvv);
    }
    
    // Phone number validation
    function validatePhoneNumber(phone) {
        return /^(09|\+639)\d{9}$/.test(phone.replace(/\s/g, ''));
    }
    
    // OTP validation
    function validateOTP(otp) {
        return /^[0-9]{6}$/.test(otp);
    }
    
    if (visaForm) {
        visaForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const cardNumber = e.target.querySelector('input[placeholder="1234 5678 9012 3456"]').value;
            const expiry = e.target.querySelector('input[placeholder="MM/YY"]').value;
            const cvv = e.target.querySelector('input[placeholder="•••"]').value;
            const name = e.target.querySelector('input[placeholder="John Doe"]').value;
            
            if (!validateCardNumber(cardNumber)) {
                alert('Please enter a valid 16-digit card number');
                return;
            }
            if (!validateExpiryDate(expiry)) {
                alert('Please enter a valid expiry date (MM/YY) that is in the future');
                return;
            }
            if (!validateCVV(cvv)) {
                alert('Please enter a valid CVV (3-4 digits)');
                return;
            }
            if (!name.trim()) {
                alert('Please enter the cardholder name');
                return;
            }
            
            showPaymentSuccess('VISA');
        });
    }
    
    if (mastercardForm) {
        mastercardForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const cardNumber = e.target.querySelector('input[placeholder="1234 5678 9012 3456"]').value;
            const expiry = e.target.querySelector('input[placeholder="MM/YY"]').value;
            const cvv = e.target.querySelector('input[placeholder="•••"]').value;
            const name = e.target.querySelector('input[placeholder="John Doe"]').value;
            
            if (!validateCardNumber(cardNumber)) {
                alert('Please enter a valid 16-digit card number');
                return;
            }
            if (!validateExpiryDate(expiry)) {
                alert('Please enter a valid expiry date (MM/YY) that is in the future');
                return;
            }
            if (!validateCVV(cvv)) {
                alert('Please enter a valid CVV (3-4 digits)');
                return;
            }
            if (!name.trim()) {
                alert('Please enter the cardholder name');
                return;
            }
            
            showPaymentSuccess('Mastercard');
        });
    }
    
    // Function to handle OTP-based payments (GCash and Maya)
    function setupOTPForm(formId, modalId, paymentType) {
        const form = document.getElementById(formId);
        if (form) {
            form.addEventListener('submit', (e) => {
                e.preventDefault();
                const phone = e.target.querySelector('input[placeholder="0912 345 6789"]').value;
                const otp = e.target.querySelector('input[placeholder="Enter 6-digit OTP"]').value;
                
                if (!validatePhoneNumber(phone)) {
                    alert('Please enter a valid Philippine mobile number (e.g., 09123456789 or +639123456789)');
                    return;
                }
                if (!validateOTP(otp)) {
                    alert('Please enter a valid 6-digit OTP');
                    return;
                }
                
                showPaymentSuccess(paymentType);
            });
            
            // Add OTP resend functionality
            const resendLink = form.querySelector('.v5-otp-resend a');
            if (resendLink) {
                resendLink.addEventListener('click', (e) => {
                    e.preventDefault();
                    const phone = form.querySelector('input[placeholder="0912 345 6789"]').value;
                    
                    if (!validatePhoneNumber(phone)) {
                        alert('Please enter a valid phone number before requesting OTP');
                        return;
                    }
                    
                    alert('New OTP has been sent to your mobile number');
                });
            }
        }
    }
    
    // Setup GCash and Maya forms
    setupOTPForm('v5-gcash-form', 'v5-gcash-modal', 'GCash');
    setupOTPForm('v5-maya-form', 'v5-maya-modal', 'Maya');
    
    console.log("Payment modals setup completed");
}

function generateConfirmationSummary() {
    console.log("Generating confirmation summary...");
    
    try {
    // Get room type and data
    const roomSelect = document.querySelector('.v5-booking-select');
    const roomType = roomSelect ? roomSelect.value : '';
    const roomData = roomDataMap[roomType];
    
        if (!roomData) {
            console.error("Room data not found");
            return;
        }
        
        // Helper function to safely update element text content
        const safeSetTextContent = (elementId, value) => {
            const element = document.getElementById(elementId);
            if (element) {
                element.textContent = value;
            } else {
                console.warn(`Element with id ${elementId} not found`);
            }
        };
        
        // Update room details
        safeSetTextContent('v5-confirm-room-type', `${roomData.name} (Room ${selectedRoomNumber})`);
        safeSetTextContent('v5-confirm-booking-type', selectedBookingType || 'Not specified');
        safeSetTextContent('v5-confirm-room-rate', formatCurrency(roomData.price));
    
    // Update room image and description
    const mainImage = document.getElementById('v5-confirm-main-image');
        if (mainImage) {
            mainImage.src = roomData.mainImage;
            console.log("Updated main image:", roomData.mainImage);
        }
        
        const description = document.getElementById('v5-confirm-description');
        if (description) {
        description.textContent = roomData.description;
        console.log("Updated description:", roomData.description);
        }
        
        // Update dates and calculate nights
        const checkinDateText = document.getElementById('v5-check-in-date')?.textContent || 'Not selected';
        const checkoutDateText = document.getElementById('v5-check-out-date')?.textContent || 'Not selected';
        
        safeSetTextContent('v5-confirm-checkin', checkinDateText);
        safeSetTextContent('v5-confirm-checkout', checkoutDateText);
        
        // Calculate nights if dates are selected
        let nights = 0;
        if (checkinDateText !== 'Not selected' && checkoutDateText !== 'Not selected') {
    const checkinDate = new Date(checkinDateText);
    const checkoutDate = new Date(checkoutDateText);
            nights = Math.ceil((checkoutDate - checkinDate) / (1000 * 60 * 60 * 24));
            safeSetTextContent('v5-confirm-nights', nights.toString());
        } else {
            safeSetTextContent('v5-confirm-nights', '0');
        }
        
        // Get and update guest information
        const guestName = document.querySelector('.v5-input-field[placeholder="John Dela Cruz"]')?.value || '';
        const guestContact = document.querySelector('.v5-input-field[placeholder="0912 345 6789"]')?.value || '';
        const guestEmail = document.querySelector('.v5-input-field[placeholder="your@email.com"]')?.value || '';
        
        safeSetTextContent('v5-confirm-guest-name', guestName || 'Not provided');
        safeSetTextContent('v5-confirm-guest-contact', guestContact || 'Not provided');
        safeSetTextContent('v5-confirm-guest-email', guestEmail || 'Not provided');
        
        // Get and update guest count and additional services
        const adultsInput = document.querySelector('.v5-guest-card:nth-child(2) .v5-number-field[value="1"]');
        const childrenInput = document.querySelector('.v5-guest-card:nth-child(2) .v5-number-field[value="0"]');
        
        safeSetTextContent('v5-confirm-adults', adultsInput?.value || '0');
        safeSetTextContent('v5-confirm-children', childrenInput?.value || '0');
        
        const extraBedsInput = document.querySelector('.v5-guest-card:nth-child(5) .v5-number-field[min="0"]:first-of-type');
        const extraRoomsInput = document.getElementById('extra-rooms-input');
        const extraBeds = parseInt(extraBedsInput?.value || '0');
        const extraRooms = parseInt(extraRoomsInput?.value || '0');
        
        safeSetTextContent('v5-confirm-extra-beds', extraBeds);
        safeSetTextContent('v5-confirm-extra-rooms', extraRooms);
        
        // Get and update checkbox services
        const spaCheckbox = document.getElementById('spa-service');
        const roomServiceCheckbox = document.getElementById('room-service');
        const airportTransferCheckbox = document.getElementById('airport-transfer');
        
        safeSetTextContent('v5-confirm-spa', spaCheckbox?.checked ? 'Yes' : 'No');
        safeSetTextContent('v5-confirm-room-service', roomServiceCheckbox?.checked ? 'Yes' : 'No');
        safeSetTextContent('v5-confirm-transfer', airportTransferCheckbox?.checked ? 'Yes' : 'No');
        
        // Get and update special requests
        const specialRequests = document.querySelector('.v5-textarea-field')?.value || '';
        safeSetTextContent('v5-confirm-notes', specialRequests || 'None');
        
        // Get and update payment method
        const selectedPaymentCard = document.querySelector('.v5-booking-payment-card.selected');
        let paymentMethod = 'Not selected';
        if (selectedPaymentCard) {
            const paymentType = selectedPaymentCard.getAttribute('data-payment');
            switch (paymentType) {
                case 'visa': paymentMethod = 'VISA'; break;
                case 'mastercard': paymentMethod = 'Mastercard'; break;
                case 'gcash': paymentMethod = 'GCash'; break;
                case 'maya': paymentMethod = 'Maya'; break;
                case 'pay-at-hotel': paymentMethod = 'Pay at Hotel'; break;
            }
        }
        safeSetTextContent('v5-confirm-payment', paymentMethod);
        
        // Calculate and update payment breakdown
        let totalAmount = 0;
        
        // Room cost calculation
        const roomCost = roomData.price * nights;
        safeSetTextContent('v5-breakdown-room-rate', formatCurrency(roomCost));
        safeSetTextContent('v5-breakdown-room-details', `${formatCurrency(roomData.price)} × ${nights} night${nights !== 1 ? 's' : ''}`);
        totalAmount += roomCost;
        
        // Extra beds calculation
        const extraBedsCost = extraBeds * 1000 * nights;
        safeSetTextContent('v5-breakdown-beds', formatCurrency(extraBedsCost));
        if (extraBeds > 0) {
            safeSetTextContent('v5-breakdown-beds-details', `${extraBeds} bed${extraBeds !== 1 ? 's' : ''} × ₱1,000 × ${nights} night${nights !== 1 ? 's' : ''}`);
        } else {
            safeSetTextContent('v5-breakdown-beds-details', 'No extra beds');
        }
        totalAmount += extraBedsCost;
        
        // Extra rooms calculation
        const extraRoomsCost = extraRooms * roomData.price * nights;
        safeSetTextContent('v5-breakdown-rooms', formatCurrency(extraRoomsCost));
        if (extraRooms > 0) {
            safeSetTextContent('v5-breakdown-rooms-details', `${extraRooms} room${extraRooms !== 1 ? 's' : ''} × ${formatCurrency(roomData.price)} × ${nights} night${nights !== 1 ? 's' : ''}`);
        } else {
            safeSetTextContent('v5-breakdown-rooms-details', 'No extra rooms');
        }
        totalAmount += extraRoomsCost;
        
        // Additional services calculation
        const servicesContainer = document.getElementById('v5-breakdown-services');
        if (servicesContainer) {
            let servicesHTML = '';
            let servicesCost = 0;
            
            if (spaCheckbox?.checked) {
                servicesHTML += `<div class="v5-breakdown-service-item">
                    <span>Spa Service</span>
                    <span>${formatCurrency(1000)}</span>
                </div>`;
                servicesCost += 1000;
            }
            
            if (roomServiceCheckbox?.checked) {
                servicesHTML += `<div class="v5-breakdown-service-item">
                    <span>Room Service</span>
                    <span>${formatCurrency(800)}</span>
                </div>`;
                servicesCost += 800;
            }
            
            if (airportTransferCheckbox?.checked) {
                servicesHTML += `<div class="v5-breakdown-service-item">
                    <span>Airport Transfer</span>
                    <span>${formatCurrency(1200)}</span>
                </div>`;
                servicesCost += 1200;
            }
            
            servicesContainer.innerHTML = servicesHTML || '<span class="v5-breakdown-details">No additional services selected</span>';
            totalAmount += servicesCost;
        }
        
        // Update total amount
        safeSetTextContent('v5-confirm-total', formatCurrency(totalAmount));
        safeSetTextContent('v5-breakdown-total', formatCurrency(totalAmount));
        
        console.log("Confirmation summary generated successfully");
        
    } catch (error) {
        console.error('Error generating confirmation summary:', error);
    }
}

async function submitBooking() {
    // Move to terms and conditions page first
    nextForm(7);
}

// Add new function for final submission after terms acceptance
async function finalizeBooking() {
    const confirmBtn = document.getElementById('v5-terms-accept-btn');
    if (!confirmBtn) return;
    
    try {
        confirmBtn.disabled = true;
        confirmBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processing...';
        
        // Get all the booking details with null checks
        const roomSelect = document.querySelector('.v5-booking-select');
        const roomType = roomSelect?.value || '';
        const roomData = roomDataMap[roomType];
        
        if (!roomData) {
            throw new Error('Invalid room type selected');
        }
        
        const checkinDateText = document.getElementById('v5-check-in-date')?.textContent || '';
        const checkoutDateText = document.getElementById('v5-check-out-date')?.textContent || '';
        
        if (!checkinDateText || !checkoutDateText || 
            checkinDateText === 'Not selected' || checkoutDateText === 'Not selected') {
            throw new Error('Please select valid check-in and check-out dates');
        }
        
        // Get guest information with null checks
        const guestName = document.querySelector('.v5-input-field[placeholder="John Dela Cruz"]')?.value?.trim() || '';
        const guestContact = document.querySelector('.v5-input-field[placeholder="0912 345 6789"]')?.value?.trim() || '';
        const guestEmail = document.querySelector('.v5-input-field[placeholder="your@email.com"]')?.value?.trim() || '';
        
        if (!guestName || !guestContact || !guestEmail) {
            throw new Error('Please fill in all guest information');
        }
        
        // Get guest counts with null checks
        const adultsInput = document.querySelector('.v5-guest-card:nth-child(2) .v5-number-field[value="1"]');
        const childrenInput = document.querySelector('.v5-guest-card:nth-child(2) .v5-number-field[value="0"]');
        const adults = parseInt(adultsInput?.value || '1');
        const children = parseInt(childrenInput?.value || '0');
        
        // Get additional services with null checks
        const extraBedsInput = document.querySelector('.v5-guest-card:nth-child(5) .v5-number-field[min="0"]:first-of-type');
        const extraRoomsInput = document.getElementById('extra-rooms-input');
        const extraBeds = parseInt(extraBedsInput?.value || '0');
        const extraRooms = parseInt(extraRoomsInput?.value || '0');
        
        // Get checkbox services with null checks
        const spaCheckbox = document.getElementById('spa-service');
        const roomServiceCheckbox = document.getElementById('room-service');
        const airportTransferCheckbox = document.getElementById('airport-transfer');
        
        const spaService = spaCheckbox?.checked || false;
        const roomService = roomServiceCheckbox?.checked || false;
        const airportTransfer = airportTransferCheckbox?.checked || false;
        
        const specialRequests = document.querySelector('.v5-textarea-field')?.value?.trim() || '';
        const paymentMethod = document.getElementById('v5-confirm-payment')?.textContent || '';
        
        if (paymentMethod === 'Not selected') {
            throw new Error('Please select a payment method');
        }

        // Format payment method to match database expectations
        let formattedPaymentMethod = paymentMethod;
        switch (paymentMethod.toLowerCase()) {
            case 'visa':
                formattedPaymentMethod = 'VISA';
                break;
            case 'mastercard':
                formattedPaymentMethod = 'Mastercard';
                break;
            case 'gcash':
                formattedPaymentMethod = 'GCash';
                break;
            case 'maya':
                formattedPaymentMethod = 'Maya';
                break;
            case 'pay at hotel':
                formattedPaymentMethod = 'Pay at Hotel';
                break;
        }
        
        // Calculate costs
        const nights = Math.ceil((new Date(checkoutDateText) - new Date(checkinDateText)) / (1000 * 60 * 60 * 24));
        const roomRate = roomData.price;
        const roomCost = nights * roomRate;
        const extraBedCost = extraBeds * 1000 * nights;
        const extraRoomCost = extraRooms * roomRate * nights;
        const spaCost = spaService ? 1000 : 0;
        const roomServiceCost = roomService ? 800 : 0;
        const transferCost = airportTransfer ? 1200 : 0;
        const totalAmount = roomCost + extraBedCost + extraRoomCost + spaCost + roomServiceCost + transferCost;
        
        // Create booking data object
        const bookingData = {
            roomType: roomType.charAt(0).toUpperCase() + roomType.slice(1),
            roomNumber: selectedRoomNumber,
            bookingType: selectedBookingType || 'Immediate Booking',
            checkInDate: checkinDateText,
            checkOutDate: checkoutDateText,
            guestName,
            guestContact: guestContact.replace(/\s/g, ''),
            guestEmail,
            adults,
            children,
            extraBeds,
            extraRooms,
            spaService,
            roomService,
            airportTransfer,
            specialRequests,
            paymentMethod: formattedPaymentMethod,
            totalAmount
        };
        
        console.log("Preparing to submit booking with data:", JSON.stringify(bookingData, null, 2));
        
        // Send booking data to server with proper error handling
        const response = await fetch('/api/booking', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(bookingData)
        });
        
        console.log("Booking response status:", response.status);
        const data = await response.json();
        console.log("Booking response data:", data);
        
        if (!response.ok) {
            // Check for specific error about existing reservation
            if (data.reservationNumber) {
                alert(`${data.message}\n\nYour existing reservation number is: ${data.reservationNumber}`);
                window.location.href = '/Reservation/Index';
                return;
            }
            
            // Handle other errors
            let errorMessage = 'Booking submission failed';
            if (data.message) {
                errorMessage = data.message;
            }
            if (data.errors) {
                errorMessage += '\n' + data.errors.join('\n');
            }
            throw new Error(errorMessage);
        }
        
        if (data.success) {
            alert(`Booking Confirmed!\n\nReservation #: ${data.reservationNumber}\n\nThank you for choosing our hotel.`);
            window.location.href = '/Reservation/Index';
        } else {
            throw new Error(data.message || 'Booking failed');
        }
        
    } catch (error) {
        console.error('Booking error details:', {
            name: error.name,
            message: error.message,
            stack: error.stack
        });
        let errorMessage = error.message || 'An unexpected error occurred. Please try again.';
        
        if (!navigator.onLine) {
            errorMessage = 'Please check your internet connection and try again.';
        }
        
        alert('Booking failed: ' + errorMessage);
    } finally {
        if (confirmBtn) {
            confirmBtn.disabled = false;
            confirmBtn.innerHTML = 'Accept';
        }
    }
}

function adjustNumber(input, change, min = 0) {
    let value = parseInt(input.value) + change;
    input.value = Math.max(min, value);
}

function formatCurrency(amount) {
    console.log("Formatting currency:", amount);
    
    if (typeof amount !== 'number' || isNaN(amount)) {
        console.error("Invalid amount!");
        return '₱0.00';
    }
    
    // Format amount as Philippine Peso
    const formattedAmount = new Intl.NumberFormat('en-PH', {
        style: 'currency',
        currency: 'PHP',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(amount);
    
    console.log("Formatted amount:", formattedAmount);
    return formattedAmount;
}

async function downloadReceipt() {
    // Get the receipt template
    const receiptTemplate = document.getElementById('receipt-template');
    if (!receiptTemplate) {
        console.error('Receipt template not found');
        return;
    }

    // Clone the template
    const receipt = receiptTemplate.cloneNode(true);
    receipt.style.display = 'block';

    // Update receipt number and date
    const receiptNumber = document.querySelector('.v7-receipt-number');
    const receiptDate = document.querySelector('.v7-receipt-date');
    if (receiptNumber) {
        receiptNumber.textContent = `Receipt #: ${bookingData.bookingId || generateReceiptNumber()}`;
    }
    if (receiptDate) {
        receiptDate.textContent = `Date: ${new Date().toLocaleDateString()}`;
    }

    // Update guest information
    const guestName = document.querySelector('.v7-receipt-guest-name');
    const guestEmail = document.querySelector('.v7-receipt-guest-email');
    const guestPhone = document.querySelector('.v7-receipt-guest-phone');
    if (guestName) guestName.textContent = bookingData.guestName || '';
    if (guestEmail) guestEmail.textContent = bookingData.guestEmail || '';
    if (guestPhone) guestPhone.textContent = bookingData.guestPhone || '';

    // Update booking details
    const roomType = document.querySelector('.v7-receipt-room-type');
    const checkIn = document.querySelector('.v7-receipt-check-in');
    const checkOut = document.querySelector('.v7-receipt-check-out');
    const nights = document.querySelector('.v7-receipt-nights');
    if (roomType) roomType.textContent = bookingData.roomType || '';
    if (checkIn) checkIn.textContent = formatDate(bookingData.checkInDate);
    if (checkOut) checkOut.textContent = formatDate(bookingData.checkOutDate);
    if (nights) {
        const nightsCount = calculateNights(bookingData.checkInDate, bookingData.checkOutDate);
        nights.textContent = `${nightsCount} night${nightsCount > 1 ? 's' : ''}`;
    }

    // Update charges table
    updateChargesTable();

    // Update payment information
    const paymentMethod = document.querySelector('.v7-receipt-payment-method');
    const paymentStatus = document.querySelector('.v7-receipt-payment-status');
    if (paymentMethod) paymentMethod.textContent = formatPaymentMethod(bookingData.paymentMethod);
    if (paymentStatus) paymentStatus.textContent = bookingData.paymentStatus || 'Paid';

    // Generate PDF options
    const opt = {
        margin: [10, 10],
        filename: `receipt_${bookingData.bookingId || 'new'}.pdf`,
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { 
            scale: 2,
            useCORS: true,
            logging: false
        },
        jsPDF: { 
            unit: 'mm',
            format: 'a4',
            orientation: 'portrait'
        }
    };

    try {
        // Generate and download PDF
        const pdf = await html2pdf().from(receipt).set(opt).save();
        
        // Clean up
        receipt.remove();
    } catch (error) {
        console.error('Error generating PDF:', error);
        alert('There was an error generating your receipt. Please try again.');
    }
}

function generateReceiptNumber() {
    const timestamp = new Date().getTime();
    const random = Math.floor(Math.random() * 1000);
    return `INV-${timestamp}-${random}`;
}

function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
}

function calculateNights(checkIn, checkOut) {
    if (!checkIn || !checkOut) return 0;
    const start = new Date(checkIn);
    const end = new Date(checkOut);
    const diffTime = Math.abs(end - start);
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
}

function updateChargesTable() {
    const tbody = document.querySelector('.v7-receipt-charges tbody');
    const tfootTotal = document.querySelector('.v7-receipt-total');
    if (!tbody || !tfootTotal) return;

    tbody.innerHTML = ''; // Clear existing rows
    let total = 0;

    // Add room charge
    const nightsCount = calculateNights(bookingData.checkInDate, bookingData.checkOutDate);
    const roomRate = getRoomRate(bookingData.roomType);
    const roomTotal = roomRate * nightsCount;
    total += roomTotal;
    
    addChargeRow(tbody, 'Room Charge', `₱${roomRate.toLocaleString()}`, nightsCount, `₱${roomTotal.toLocaleString()}`);

    // Add extra bed charges if any
    if (bookingData.extraBeds > 0) {
        const extraBedRate = 800; // Extra bed rate per night
        const extraBedTotal = extraBedRate * bookingData.extraBeds * nightsCount;
        total += extraBedTotal;
        addChargeRow(tbody, 'Extra Bed', `₱${extraBedRate.toLocaleString()}`, 
            `${bookingData.extraBeds} × ${nightsCount} nights`, 
            `₱${extraBedTotal.toLocaleString()}`);
    }

    // Add additional services if any
    if (bookingData.services) {
        for (const service of bookingData.services) {
            const serviceTotal = service.price * service.quantity;
            total += serviceTotal;
            addChargeRow(tbody, service.name, `₱${service.price.toLocaleString()}`, 
                service.quantity, `₱${serviceTotal.toLocaleString()}`);
        }
    }

    // Update total
    if (tfootTotal) {
        tfootTotal.textContent = `₱${total.toLocaleString()}`;
    }
}

function addChargeRow(tbody, description, rate, quantity, amount) {
    const tr = document.createElement('tr');
    tr.innerHTML = `
        <td>${description}</td>
        <td>${rate}</td>
        <td>${quantity}</td>
        <td>${amount}</td>
    `;
    tbody.appendChild(tr);
}

function getRoomRate(roomType) {
    const rates = {
        'Standard': 2500,
        'Deluxe': 3500,
        'Suite': 5000
    };
    return rates[roomType] || 0;
}

function formatPaymentMethod(method) {
    if (!method) return '';
    const formats = {
        'visa': 'VISA',
        'mastercard': 'Mastercard',
        'gcash': 'GCash',
        'maya': 'Maya',
        'pay at hotel': 'Pay at Hotel'
    };
    return formats[method.toLowerCase()] || method;
}

// Add styles for unavailable rooms
const style = document.createElement('style');
style.textContent = `
    .v5-room-button:disabled {
        opacity: 0.5;
        cursor: not-allowed;
        position: relative;
    }
    
    .room-status {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        background: rgba(0, 0, 0, 0.7);
        color: white;
        padding: 2px 6px;
        border-radius: 4px;
        font-size: 0.8em;
    }
`;
document.head.appendChild(style);

// Add styles for room buttons
const roomButtonStyles = document.createElement('style');
roomButtonStyles.textContent = `
    .v5-room-button {
        background-color: #f8f8f8;
        border: 1px solid #ddd;
        border-radius: 6px;
        padding: 10px 15px;
        font-size: 14px;
        cursor: pointer;
        transition: all 0.3s ease;
        position: relative;
        text-align: center;
    }
    
    .v5-room-button:hover {
        background-color: #eaf6ff;
        border-color: #a0d4ff;
    }
    
    .v5-room-button.selected {
        background-color: #290909;
        color: white;
        border-color: #290909;
    }
    
    .v5-room-button.disabled {
        opacity: 0.6;
        cursor: not-allowed;
        background-color: #f0f0f0;
    }
    
    .v5-room-button .room-status {
        position: absolute;
        bottom: -8px;
        left: 50%;
        transform: translateX(-50%);
        background-color: #ff5252;
        color: white;
        padding: 2px 8px;
        border-radius: 10px;
        font-size: 10px;
        white-space: nowrap;
    }
`;
document.head.appendChild(roomButtonStyles);
