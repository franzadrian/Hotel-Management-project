function toggleSidebar() {
    let sidebar = document.getElementById("sidebar");
    let headerRight = document.querySelector(".header-right");

    if (sidebar && headerRight) {
        sidebar.classList.toggle("active");

        // Hide notification & sidebar button when sidebar is open
        if (sidebar.classList.contains("active")) {
            headerRight.style.display = "none";
        } else {
            headerRight.style.display = "flex";
        }
    }
}

// Load notifications when page loads
document.addEventListener('DOMContentLoaded', function() {
    loadNotifications();
    
    // Add sidebar toggle button to header-right if it doesn't exist
    const headerRight = document.querySelector('.header-right');
    const sidebar = document.getElementById('sidebar');
    
    if (headerRight && sidebar && !document.querySelector('.sidebar-btn')) {
        const sidebarBtn = document.createElement('button');
        sidebarBtn.className = 'sidebar-btn';
        sidebarBtn.onclick = toggleSidebar;
        
        const icon = document.createElement('i');
        icon.className = 'fas fa-bars';
        
        sidebarBtn.appendChild(icon);
        headerRight.appendChild(sidebarBtn);
    }
});

// Function to load notifications from the server
function loadNotifications() {
    // Check if notification elements exist before proceeding
    const notificationBox = document.getElementById('notificationBox');
    const notificationToggle = document.getElementById('notificationToggle');
    
    // If notification elements don't exist on the page, don't attempt to load notifications
    if (!notificationBox || !notificationToggle) {
        console.log('Notification elements not found on page, skipping notification loading');
        return;
    }
    
    fetch('/api/Notification/GetNotifications')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                displayNotifications(data.notifications);
                // Only show unread count in badge
                updateNotificationBadge(data.totalUnread || 0);
            } else {
                console.error('Error loading notifications:', data.message);
            }
        })
        .catch(error => {
            console.error('Error fetching notifications:', error);
        });
}

// Function to display notifications in the notification box
function displayNotifications(notifications) {
    const notificationBox = document.getElementById('notificationBox');
    if (!notificationBox) return;
    
    const notificationList = notificationBox.querySelector('.notification-list');
    const emptyNotification = notificationBox.querySelector('.empty-notification');
    const markAllReadContainer = notificationBox.querySelector('.mark-all-read-container');
    
    if (!notificationList) return;
    
    notificationList.innerHTML = ''; // Clear existing notifications
    
    if (!notifications || notifications.length === 0) {
        // Show empty notification message, hide list and mark all read button
        if (emptyNotification) emptyNotification.style.display = 'flex';
        if (markAllReadContainer) markAllReadContainer.style.display = 'none';
        notificationList.style.display = 'none';
        return;
    }
    
    // Hide empty notification message, show list and mark all read button if there are unread notifications
    if (emptyNotification) emptyNotification.style.display = 'none';
    notificationList.style.display = 'block';
    
    // Check if there are any unread notifications
    const hasUnread = notifications.some(notification => !notification.isRead);
    
    // Show/hide mark all read button based on unread notifications
    if (markAllReadContainer) {
        markAllReadContainer.style.display = hasUnread ? 'flex' : 'none';
    }
    
    notifications.forEach(notification => {
        const li = document.createElement('li');
        li.className = notification.isRead ? 'notification-item' : 'notification-item unread';
        li.dataset.id = notification.id;
        
        // Set inline hover styles using a special data attribute
        li.dataset.hoverColor = notification.isRead ? '#fcf9f5' : '#ffffff';
        
        // Add custom hover effect with JavaScript
        li.addEventListener('mouseenter', function() {
            this.style.backgroundColor = this.dataset.hoverColor;
            this.style.boxShadow = 'inset 0 0 0 1px rgba(41, 9, 9, 0.1)';
        });
        
        li.addEventListener('mouseleave', function() {
            this.style.backgroundColor = notification.isRead ? '#d4a373' : '#e0b684';
            this.style.boxShadow = 'none';
        });
        
        const icon = document.createElement('i');
        icon.className = notification.icon || 'fas fa-bell';
        
        const content = document.createElement('div');
        content.className = 'notification-content';
        
        const message = document.createElement('p');
        message.textContent = notification.message;
        
        const timeAgo = document.createElement('small');
        const timeIcon = document.createElement('i');
        timeIcon.className = 'fas fa-clock';
        timeAgo.appendChild(timeIcon);
        
        const timeText = document.createTextNode(' ' + getTimeAgo(new Date(notification.createdAt)));
        timeAgo.appendChild(timeText);
        
        content.appendChild(message);
        content.appendChild(timeAgo);
        
        li.appendChild(icon);
        li.appendChild(content);
        
        // Ensure consistent padding for both read and unread notifications
        // We'll handle this in CSS instead of inline styles
        
        // Add click handler for marking as read and navigation
        if (notification.relatedUrl) {
            li.style.cursor = 'pointer';
            li.addEventListener('click', function() {
                // Mark as read
                if (!notification.isRead) {
                    markNotificationAsRead(notification.id);
                }
                // Navigate to the related URL
                window.location.href = notification.relatedUrl;
            });
        }
        
        notificationList.appendChild(li);
    });
}

// Function to update notification badge
function updateNotificationBadge(count) {
    const notificationBtn = document.getElementById('notificationToggle');
    if (!notificationBtn) return;
    
    // Remove existing badge if any
    const existingBadge = notificationBtn.querySelector('.notification-badge');
    if (existingBadge) {
        existingBadge.remove();
    }
    
    // Add badge if there are notifications
    if (count > 0) {
        const badge = document.createElement('span');
        badge.className = 'notification-badge';
        badge.textContent = count;
        notificationBtn.appendChild(badge);
    }
}

// Helper function to format time ago
function getTimeAgo(date) {
    const now = new Date();
    const diffMs = now - date;
    const diffSec = Math.floor(diffMs / 1000);
    const diffMin = Math.floor(diffSec / 60);
    const diffHour = Math.floor(diffMin / 60);
    const diffDay = Math.floor(diffHour / 24);
    
    if (diffSec < 60) {
        return 'Just now';
    } else if (diffMin < 60) {
        return `${diffMin} minute${diffMin > 1 ? 's' : ''} ago`;
    } else if (diffHour < 24) {
        return `${diffHour} hour${diffHour > 1 ? 's' : ''} ago`;
    } else {
        return `${diffDay} day${diffDay > 1 ? 's' : ''} ago`;
    }
}

// Add event listeners safely with null checks
document.addEventListener('DOMContentLoaded', function() {
    // Notification toggle
    const notificationToggle = document.getElementById("notificationToggle");
    const notificationBox = document.getElementById("notificationBox");
    
    if (notificationToggle && notificationBox) {
        notificationToggle.addEventListener("click", function (e) {
            e.stopPropagation();
            notificationBox.classList.toggle("active");
            
            // If opening notifications, refresh the list
            if (notificationBox.classList.contains("active")) {
                loadNotifications();
            }
        });
    }
    
    // Mark all notifications as read
    const markAllReadBtn = document.querySelector('.mark-all-read');
    if (markAllReadBtn) {
        markAllReadBtn.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            // Check if there are any unread notifications
            const unreadItems = document.querySelectorAll('.notification-item.unread');
            if (unreadItems.length === 0) {
                // No unread notifications, show brief message
                const originalText = markAllReadBtn.innerHTML;
                markAllReadBtn.innerHTML = '<i class="fas fa-check"></i> All read';
                setTimeout(() => {
                    markAllReadBtn.innerHTML = originalText;
                }, 1500);
                return;
            }
            
            // Show loading state
            const originalText = markAllReadBtn.innerHTML;
            markAllReadBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Marking...';
            markAllReadBtn.disabled = true;
            
            fetch('/api/Notification/MarkAllAsRead', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    // Update UI - remove unread class from all notifications
                    unreadItems.forEach(item => {
                        item.classList.remove('unread');
                    });
                    
                    // Show success state
                    markAllReadBtn.innerHTML = '<i class="fas fa-check"></i> Done';
                    markAllReadBtn.disabled = false;
                    
                    // Reset badge
                    updateNotificationBadge(0);
                    
                    // Return to original text after a delay
                    setTimeout(() => {
                        markAllReadBtn.innerHTML = originalText;
                    }, 1500);
                    
                    console.log('All notifications marked as read');
                }
            })
            .catch(error => {
                console.error('Error marking all notifications as read:', error);
                // Reset button on error
                markAllReadBtn.innerHTML = originalText;
                markAllReadBtn.disabled = false;
            });
        });
    }
    
    // User profile dropdown
    const userProfileToggle = document.getElementById('userProfileToggle');
    const userProfileDropdown = document.getElementById('userProfileDropdown');
    
    if (userProfileToggle && userProfileDropdown) {
        userProfileToggle.addEventListener('click', function() {
            userProfileDropdown.classList.toggle('active');
        });
    }
    
    // View account link
    const viewAccount = document.getElementById('viewAccount');
    if (viewAccount) {
        viewAccount.addEventListener('click', function(event) {
            event.preventDefault(); // Prevent default link behavior
            window.location.href = '/Account/UserInfo';
        });
    }
    
    // Close dropdowns when clicking outside
    document.addEventListener('click', function(event) {
        // Close user profile dropdown
        if (userProfileToggle && userProfileDropdown) {
            if (event.target !== userProfileToggle && !userProfileToggle.contains(event.target) &&
                !userProfileDropdown.contains(event.target)) {
                userProfileDropdown.classList.remove('active');
            }
        }
        
        // Close notification dropdown
        if (notificationToggle && notificationBox) {
            if (event.target !== notificationToggle && !notificationToggle.contains(event.target) && 
                !notificationBox.contains(event.target)) {
                notificationBox.classList.remove('active');
            }
        }
    });
});

// Function to mark a notification as read
function markNotificationAsRead(notificationId) {
    fetch(`/api/Notification/${notificationId}/MarkAsRead`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Failed to mark notification as read');
        }
        return response.json();
    })
    .then(data => {
        if (data.success) {
            // Update UI to mark notification as read
            const notificationItem = document.querySelector(`li[data-id="${notificationId}"]`);
            if (notificationItem) {
                notificationItem.classList.remove('unread');
            }
            
            // Update notification badge count
            const badge = document.querySelector('.notification-badge');
            if (badge) {
                const count = parseInt(badge.textContent);
                if (count > 1) {
                    badge.textContent = count - 1;
                } else {
                    badge.style.display = 'none';
                }
            }
            
            // Check if there are any unread notifications left
            const unreadItems = document.querySelectorAll('.notification-item.unread');
            if (unreadItems.length === 0) {
                // Hide the "Mark All Read" button if no unread notifications remain
                const markAllReadContainer = document.querySelector('.mark-all-read-container');
                if (markAllReadContainer) {
                    markAllReadContainer.style.display = 'none';
                }
            }
        }
    })
    .catch(error => {
        console.error('Error marking notification as read:', error);
    });
}